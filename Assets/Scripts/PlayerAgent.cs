using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : C4Agent
{
	bool playing = false;

	public override void PlayTurn() {
		playing = true;
	}

	void Update() {
		if (playing) {
			//do a thing
		}
		//gameMaster.board.Play(IsPlayer1, col);
		//call gameMaster.EndTurn(0) on turn end;
		//playing = false;
	}

}
