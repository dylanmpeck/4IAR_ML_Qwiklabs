using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : C4Agent
{
	bool playing = false;
    bool IsPlayer1;

    public override void PlayTurn(bool p1) {
        IsPlayer1 = p1;
		playing = true;
	}

	void Update() {
		if (playing) {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            int col = GetMouseOverColumn(mouseWorldPos);
            if (col == -1)
            {
                gameMaster.board.SetTokenToMousePos(IsPlayer1, mouseWorldPos);
            }
            else
            {
                gameMaster.board.SetTokenAboveColumn(IsPlayer1, col);
                if (Input.GetMouseButtonDown(0))
                {
                    if (gameMaster.board.Play(col, IsPlayer1))
                    {
                        EndTurn();
                    }
                }
            }

            

           // if (gameMaster.board.successfullyPlaced)
             //   EndTurn();
			//do a thing
		}
		//gameMaster.board.Play(IsPlayer1, col);
		//call gameMaster.EndTurn(0) on turn end;
		//playing = false;
	}

    void EndTurn()
    {
        playing = false;
        gameMaster.board.successfullyPlaced = false;
        gameMaster.EndTurn(0);
    }

    int GetMouseOverColumn(Vector3 mouseWorldPos)
    {
        mouseWorldPos.z = -1;
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector3.forward);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "ColumnCollider")
            {
                return (hit.collider.name[3] - '0');
            }
            else
            {
                return (-1);
            }
        }
        return (-1);
    }

    Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
