using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
	public Board board;
	public C4Agent P1;
	public C4Agent P2;

	//training mode enables control by the academy (through the step function)
	[HideInInspector]
	public bool trainingMode = false;

	//reward amount on win or loss (loss is negative)
	int reward = 50;
	int forfeitReward = -500;

	bool gameOver = false;
	bool p1Turn = true;
	bool p1TurnLastTick = false;

	void Awake() {
		ResetGame();
	}

	void Update() {
		if (!trainingMode) {
			if (!gameOver && p1Turn != p1TurnLastTick) {
				(p1Turn ? P1 : P2).PlayTurn(p1Turn);
				p1TurnLastTick = p1Turn;
			} else if (gameOver) {
				if (Input.GetKeyDown("return")) {
					ResetGame();
				}
			}
		}
	}

	//on step, make the agent who's turn it is play
	public void Step() {
		if (gameOver) {
			ResetGame();
		}
		(p1Turn ? P1 : P2).PlayTurn(p1Turn);
		if (p1Turn == p1TurnLastTick) {
			Debug.LogError("Steps are out of order!");
		}
		p1TurnLastTick = p1Turn;
	}

	//End the turn
	//Codes:
	//0 = turn done
	//1 = error
	//2 = forfeit
	public void EndTurn(int statusCode) {
		//ends the current player's turn
		if (statusCode == 0) {
			//check for winner
			int winner = board.CheckForWin();
			if (winner != 0) {
				//Debug.Log("Player " + (winner == 1 ? "1" : "2") + " won!");
				P1.SetReward(winner * reward);
				P2.SetReward(-winner * reward);
				SetGameOver();
			} else if (board.IsFull()) {
				//check for draw
				SetGameOver();
				//Debug.Log("Draw!");
			} else {
				p1Turn = !p1Turn;
			}
		} else if (statusCode == 1) {
			Debug.LogError("Player " + (p1Turn ? "1" : "2") + " experienced an error.");
			SetGameOver();
		} else if (statusCode == 2) {
			Debug.Log("Player " + (p1Turn ? "1" : "2") + " forfeited.");
			(p1Turn ? P1 : P2).SetReward(forfeitReward);
			SetGameOver();
		}
	}

	public void SetGameOver() {
		gameOver = true;
	}

	//Reset the board and agents
	public void ResetGame() {
		P1.Done();
		P2.Done();
		p1Turn = true;
		p1TurnLastTick = !p1Turn;
		board.ResetBoard();
		gameOver = false;
	}
}
