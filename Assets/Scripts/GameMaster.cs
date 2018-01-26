using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	Animation animation;
	GameObject playerPrefab;

	List<Unit> units = new List<Unit>();


	private void Start() {
		Vector3 randomPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
		GameObject unitObject = Instantiate(playerPrefab, randomPos, Quaternion.identity) as GameObject;
		units.Add(unitObject.GetComponent<Unit>());		
	}

	private void Update() {

	}
}
