using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A training module for adversarial self-play
public class C4AdversarialGame : MonoBehaviour
{
	//Objects needed for the game
	public Board board;
	public C4Agent P1;
	public C4Agent P2;

	//reward amount on win or loss (loss is negative)
	int reward = 50;

	bool p1Turn = false;

	//on step, make the agent who's turn it is play
	public void Step() {
		p1Turn = !p1Turn;
		if (p1Turn) {
			P1.PlayTurn();
		} else {
			P2.PlayTurn();
		}
	}

	//Check to see if either agent has won, or if there has been a draw
	public void CheckForGameOver() {

		//check for winner
		int winner = board.CheckForWin();
		if (winner != 0) {
			//Debug.Log("Player " + (winner == 1 ? "1" : "2") + " won!");
			P1.SetReward(winner * reward);
			P2.SetReward(-winner * reward);
			ResetGame();
		}
		else if (board.IsFull()) {
			//check for draw
			ResetGame();
			//Debug.Log("Draw!");
		}
	}

	//Reset the board and agents
	public void ResetGame() {
		P1.Done();
		P2.Done();
		p1Turn = false;
		board.ResetBoard();
	}
}
