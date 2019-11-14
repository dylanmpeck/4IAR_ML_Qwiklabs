using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

//A standard Connect 4 Agent
public abstract class C4Agent : Agent
{
	public bool IsPlayer1;
	public GameMaster gameMaster;

	//Require a PlayTurn method for all connect4 agents
	public abstract void PlayTurn();
}
