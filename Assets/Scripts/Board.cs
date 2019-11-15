using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Connect 4 Board class. All the board state and win logic is here.
public class Board : MonoBehaviour
{
	int width = 7;
	int height = 6;

	//All graphical prefabs needed to build the board procedurally
	public GameObject gridCellPrefab;
	public GameObject p1TokenPrefab;
	public GameObject p2TokenPrefab;

	//If NoGUI is true, the graphics for the board will not be initialized (Saves memory from gameObjects being created)
	public bool noGUI = false;

	//A list of all the player token gameObjects being used
	List<GameObject> tokens = new List<GameObject>();

	//Token object pools to minimize creating and deleting memory on each play and board clear
	List<GameObject> p1TokenPool = new List<GameObject>();
	List<GameObject> p2TokenPool = new List<GameObject>();

    //Positions above each respective column where dropping piece will start.
    List<Vector2> placementPositions = new List<Vector2>();

	//Has the board been initialized (Some ml-agents behavior happen on awake)
	bool initialized = false;
	//Player 1 == 1, Player 2 == -1, no token == 0
	int[,] boardState;

    GameObject currentToken;
    bool currentTokenActivated;

    public bool successfullyPlaced;

	//Initialize on start
	void Start() {
		InitializeBoard();
	}

	//Generates the board's graphics and initializes the board state to 0
	void InitializeBoard() {
		if (!initialized) {
			boardState = new int[height, width];
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (!noGUI) {
						GameObject cell = Instantiate(gridCellPrefab, new Vector2(transform.position.x + x, transform.position.y - y), Quaternion.identity);
                        if (y == height / 2) createColumnTriggersForMouse(cell, x);
                        cell.transform.parent = transform;
					}
					boardState[y, x] = 0;
				}
			}
			//Initalize token pool
			if (!noGUI) {
				GameObject t;
				//TODO do cleanly
				for (int i = 0; i < (width * height / 2) + 1; i++) {
					t = Instantiate(p1TokenPrefab, Vector3.zero, Quaternion.identity);
					t.transform.parent = transform;
					t.SetActive(false);
					p1TokenPool.Add(t);

					t = Instantiate(p2TokenPrefab, Vector3.zero, Quaternion.identity);
					t.transform.parent = transform;
					t.SetActive(false);
					p2TokenPool.Add(t);
				}
				initialized = true;
			}

            //Initialize placement positions for each column.
            if (!noGUI)
                initializePlacementPositions();
		}
	}

	//Clear the board state and remove all tokens.
	//TODO, reuse tokens from a gameObject pool to reduce extra GC work
	public void ResetBoard() {
		InitializeBoard();
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				boardState[y, x] = 0;
			}
		}
		if (!noGUI) {
			foreach (GameObject token in tokens) {
				token.SetActive(false);
			}
			tokens.Clear();
		}
	}

    void initializePlacementPositions()
    {
        for (int x = 0; x < width; x++)
        {
            placementPositions.Add(new Vector2(transform.position.x + x, transform.position.y + 1));
        }
    }

	//Place a token into a given column.
	//Return false if the column is not valid, or if the column is full (any invalid move)
    public bool Play(int col, bool p1) {
        if (col < 0 || col >= width)
        {
            Debug.LogError("Not a valid column!");
            return false;
        }

        for (int y = 0; y < height; y++)
        {
            if (boardState[y, col] == 0)
            {
                boardState[y, col] = p1 ? 1 : -1;
                if (!noGUI)
                {
                    //GameObject token = Instantiate(p1 ? p1TokenPrefab : p2TokenPrefab, new Vector2(transform.position.x + col, transform.position.y - ((height - 1) - y)), Quaternion.identity);
                    if (!currentTokenActivated) activateToken(p1);
                    setTokenAboveColumn(p1, col);
                    StartCoroutine(dropPiece(currentToken, y, col));
                    //token.transform.parent = transform;
                    //tokens.Add(token);
                }
                return true;
            }
        }
        //Debug.LogError("Column " + col.ToString() + " is full!");
        return false;
    }

    IEnumerator dropPiece(GameObject token, int y, int col)
    {
        Vector3 startPos = token.transform.position;
        Vector3 endPos = startPos;
        endPos.y -= height - y;

        GameObject placedPiece = Instantiate(token, startPos, Quaternion.identity);
        placedPiece.SetActive(true);
        placedPiece.transform.parent = transform;
        tokens.Add(placedPiece);

        token.SetActive(false);

        float vel = 0.0f;

        while (Mathf.Approximately(placedPiece.transform.position.y, endPos.y) == false)
        {
            vel += -42f * Time.deltaTime;
            placedPiece.transform.Translate(new Vector3(0, vel * Time.deltaTime, 0));
            if (placedPiece.transform.position.y < endPos.y)
            {
                placedPiece.transform.position = endPos;
            }
            yield return null;
        }

        currentTokenActivated = false;
        successfullyPlaced = true;
    }

    //Get an available token from the token pool
    public GameObject GetTokenFromPool(List<GameObject> pool) {
		foreach (GameObject go in pool) {
			if (!go.activeSelf) {
				return go;
			}
		}
		return null;
	}

	//Checks for a connect 4 in the board state (can be optimized)
	//Returns the logical player number (1 or -1) if there is a win. Returns 0 if no one has won
	public int CheckForWin() {
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				int t = boardState[y, x];
				if (t != 0) {
					if (CheckH(x, y, t) || CheckV(x, y, t) || CheckDU(x, y, t) || CheckDD(x, y, t)) {
						return (t);
					}
				}
			}
		}
		return 0;
	}

	//Check for a horizontal connect 4 at pos (x, y) with player number p
	bool CheckH(int x, int y, int p) {
		if (x <= width - 4) {
			if (boardState[y, x + 1] == p && boardState[y, x + 2] == p && boardState[y, x + 3] == p) {
				return true;
			}
		}
		return false;
	}

	//Check for a vertical connect 4 at pos (x, y) with player number p
	bool CheckV(int x, int y, int p) {
		if (y <= height - 4) {
			if (boardState[y + 1, x] == p && boardState[y + 2, x] == p && boardState[y + 3, x] == p) {
				return true;
			}
		}
		return false;
	}

	//Check for a diagonal upwards connect 4 at pos (x, y) with player number p
	bool CheckDU(int x, int y, int p) {
		if (x <= width - 4 && y <= height - 4) {
			if (boardState[y + 1, x + 1] == p && boardState[y + 2, x + 2] == p && boardState[y + 3, x + 3] == p) {
				return true;
			}
		}
		return false;
	}

	//Check for a diagonal downwards connect 4 at pos (x, y) with player number p
	bool CheckDD(int x, int y, int p) {
		if (x <= width - 4 && y >= 3) {
			if (boardState[y - 1, x + 1] == p && boardState[y - 2, x + 2] == p && boardState[y - 3, x + 3] == p) {
				return true;
			}
		}
		return false;
	}

	//Returns true if the board is full (this usually means there's a draw)
	public bool IsFull() {
		for (int y = height - 1; y >= 0; y--) {
			for (int x = 0; x < width; x++) {
				if (boardState[y, x] == 0) {
					return false;
				}
			}
		}
		return true;
	}

	//Print the logical board state to console
	public void PrintBoard() {
		string p = "";
		for (int y = height - 1; y >= 0; y--) {
			for (int x = 0; x < width; x++) {
				p += string.Format("{0, 2}, ", boardState[y, x]);
			}
			p += System.Environment.NewLine;
		}
		Debug.Log(p);
	}

	//Generate a float array representation of the board for ML-agents observation
	//It currently represents each cell as two values either one or zero based on which player is occupying the cell.
	//Ex: [1, 0] if the agent's token is there, [0, 1] if the enemie's token is in the cell, [0, 0] if the token is free.
	//Preliminary testing with a single value of +1, -1, or 0 based on cell state seems to indicate very little issue training at first, but reduced training performance long term.
	//Would be ideal to not have to create a new board state every step to use as an observation.
	public float[] GetBoardState(bool p1) {
		float[] bs = new float[width * height * 2];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				bs[(y * width + x) * 2] = 0;
				bs[(y * width + x) * 2 + 1] = 0;
				if (boardState[y, x] == 1) {
					if (p1) {
						bs[(y * width + x) * 2] = 1f;
					} else {
						bs[(y * width + x) * 2 + 1] = 1f;
					}
				} else if (boardState[y, x] == -1) {
					if (p1) {
						bs[(y * width + x) * 2 + 1] = 1f;
					} else {
						bs[(y * width + x) * 2] = 1f;
					}
				}
			}
		}

		return bs;
	}

	public bool SetBoardState(int[,] bs) {
		if (bs.GetLength(0) == height && bs.GetLength(1) == width) {
			boardState = bs;
			return true;
		}
		return false;
	}

    //Creates box triggers over each column in order to check which column the mouse is hovered over when placing
    void createColumnTriggersForMouse(GameObject cell, int col)
    {
        GameObject columnCollider = new GameObject("columnCollider" + col);
        Vector3 centerOfColumn = cell.transform.position;
        centerOfColumn.x += 0.5f;
        columnCollider.transform.position = centerOfColumn;
        columnCollider.transform.rotation = Quaternion.identity;
        columnCollider.transform.parent = transform;
        BoxCollider2D bc = columnCollider.AddComponent<BoxCollider2D>();
        bc.isTrigger = true;
        bc.size = new Vector2(bc.size.x, height);
    }

    void activateToken(bool p1)
    {
        currentToken = GetTokenFromPool(p1 ? p1TokenPool : p2TokenPool);
        currentTokenActivated = true;
    }

    public void setTokenAboveColumn(bool p1, int col)
    {
        if (!currentTokenActivated)
            activateToken(p1);

        currentToken.transform.position = placementPositions[col];
    }

    public void setTokenToMousePos(bool p1, Vector3 position)
    {
        if (!currentTokenActivated)
            activateToken(p1);

        currentToken.transform.position = position;
    }
}

