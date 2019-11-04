using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

//A standard Connect 4 Agent
public class C4Agent : Agent
{
	public bool IsPlayer1;
	public C4AdversarialGame gameMaster;

	//Initializes the agent, we don't need to do anything, but this is a required override.
	public override void InitializeAgent() {

	}

	//Resets the agent, we don't need to do anything, but this is a required override.
	public override void AgentReset() {

	}

	//Play a move. 
	//RequestDecision is an ML-agents method which calls CollectObservation(), runs it through the agent's brain, then calls AgentAction() based on the brain's output
	public void PlayTurn() {
		RequestDecision();
	}

	//Get the board state and add it to our vector observation
	public override void CollectObservations() {
		AddVectorObs(gameMaster.board.GetBoardState(IsPlayer1));
	}

	//Play a move based on our action vector. 
	public override void AgentAction(float[] vectorAction, string textAction) {
		int colToPlay = Mathf.FloorToInt(vectorAction[0]);
		if (!gameMaster.board.Play(colToPlay, IsPlayer1)) {
			//if the agent tries to play an invalid move, double lose 'cause bad.
			SetReward(-500f);
			gameMaster.ResetGame();
		} else {
			//We need to check for gameover here as agent action is called as an async method
			gameMaster.CheckForGameOver();
		}
	}
}
