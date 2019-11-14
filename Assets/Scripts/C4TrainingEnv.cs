using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

//An adversarial self-play training environment for connect 4.
public class C4TrainingEnv : Academy
{
	//prefab for a training module (contains the game controller, board, and agents)
	public GameObject trainingModulePrefab;
	//Defines the size of the grid of training modules
	public Vector2 moduleGridSize;
	//Defines the x and y offsets for each module
	public Vector2 moduleOffsets;

	//The list of all modules
	List<GameMaster> trainingModules;

	//Create the grid of training modules on start
	void Start()
    {
		trainingModules = new List<GameMaster>();
		for (int y = 0; y < moduleGridSize.y; y++) {
			for (int x = 0; x < moduleGridSize.x; x++) {
				GameObject module = Instantiate(trainingModulePrefab, new Vector2(x * moduleOffsets.x, -y * moduleOffsets.y), Quaternion.identity);
				module.transform.parent = transform;
				trainingModules.Add(module.GetComponent<GameMaster>());
			}
		}
	}

	//Reset all the modules on academy reset
	public override void AcademyReset() {
		foreach (GameMaster module in trainingModules) {
			module.ResetGame();
		}
	}

	//Step in all the training modules on academy step
	public override void AcademyStep() {
		foreach (GameMaster module in trainingModules) {
			module.Step();
		}
	}
}
