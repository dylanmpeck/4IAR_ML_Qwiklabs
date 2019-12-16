using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : C4Agent
{
	bool isPlayer1;
	//Play a move. 
	//RequestDecision is an ML-agents method which calls CollectObservation(), runs it through the agent's brain, then calls AgentAction() based on the brain's output
	public override void PlayTurn(bool p1) {
		isPlayer1 = p1;
		RequestDecision();
	}

	//Get the board state and add it to our vector observation
	public override void CollectObservations() {
		AddVectorObs(gameMaster.board.GetBoardState(isPlayer1));
	}

	//Play a move based on our action vector. 
	public override void AgentAction(float[] vectorAction, string textAction) {
		int colToPlay = Mathf.FloorToInt(vectorAction[0]);
		if (!gameMaster.board.Play(colToPlay, isPlayer1)) {
			//if the agent tries to play an invalid move, end turn with forfeit code
			gameMaster.EndTurn(2);
		} else {
			//We need to check for gameover here as agent action is called as an async method
			gameMaster.EndTurn(0);
		}
	}
}
