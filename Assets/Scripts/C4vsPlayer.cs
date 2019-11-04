using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

//Temporary agent that for human vs agent play.
public class C4vsPlayer : Agent
{
	public bool humanStarts = true;
	public Board board;

	bool humansTurn;
	bool gameOver;

	void Start() {
		ResetGame();
	}

	// Update is called once per frame
	void Update()
    {
		if (!gameOver) {
			if (humansTurn) {
				if (Input.GetKeyDown("1")) {
					board.Play(0, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("2")) {
					board.Play(1, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("3")) {
					board.Play(2, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("4")) {
					board.Play(3, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("5")) {
					board.Play(4, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("6")) {
					board.Play(5, humanStarts);
					NextTurn();
				} else if (Input.GetKeyDown("7")) {
					board.Play(6, humanStarts);
					NextTurn();
				}
			} else {
				RequestDecision();
			}
		} else {
			if (Input.GetKeyDown("return")) {
				ResetGame();
			}
		}
		if (Input.GetKeyDown("p")) {
			board.PrintBoard();
		}
    }

	void NextTurn() {
		//check for winner
		int winner = board.CheckForWin();
		if (winner != 0) {
			Debug.Log("Player " + (winner == 1 ? "1" : "2") + " won!");
			gameOver = true;
		} else if (board.IsFull()) {
			//check for draw
			Debug.Log("Draw!");
			gameOver = true;
		}

		humansTurn = !humansTurn;
	}

	void ResetGame() {
		humansTurn = humanStarts;
		gameOver = false;
		board.ResetBoard();
	}

	public override void InitializeAgent() {

	}

	public override void AgentReset() {

	}

	public override void CollectObservations() {
		AddVectorObs(board.GetBoardState(!humanStarts));
	}

	public override void AgentAction(float[] vectorAction, string textAction) {
		int colToPlay = Mathf.FloorToInt(vectorAction[0]);
		if (!board.Play(colToPlay, !humanStarts)) {
			//if the agent tries to play an invalid move, its a forfeit
			Debug.Log("Agent forfeits. You win.");
			gameOver = true;
		}
		NextTurn();
	}
}
