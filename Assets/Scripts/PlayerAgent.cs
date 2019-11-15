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
            int col = getMouseOverColumn();
            if (col == -1)
                gameMaster.board.setTokenToMousePos(IsPlayer1, getMouseWorldPosition());
            else
                gameMaster.board.setTokenAboveColumn(IsPlayer1, col);

            if (Input.GetMouseButtonDown(0))
            {
                if (gameMaster.board.Play(col, IsPlayer1))
                {
                    EndTurn();
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

    int getMouseOverColumn()
    {
        Ray mouseRay;
        RaycastHit mouseHit;

        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out mouseHit))
        {
            if (mouseHit.collider.name.Contains("columnCollider"))
            {
                return (mouseHit.collider.name[14] - '0');
            }
            else
            {
                return (-1);
            }
        }
        return (-1);
    }

    Vector3 getMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
