﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	Animation animation;
	public GameObject playerPrefab;
    public int numPlayers;
    public GameObject bullet;

	List<Unit> units = new List<Unit>();


    private void Start() {
        for (int i = 0; i < numPlayers; i++)
        {
// 			float distance = 1000f;
//             Vector3 randomPos = new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), 0);
// 		    GameObject unitObject = Instantiate(playerPrefab, randomPos, Quaternion.identity) as GameObject;
// 		    units.Add(unitObject.GetComponent<Unit>());
            //give infection to first player
//             if (i == 0)
//             {
//                 Gun gun = unitObject.AddComponent<RodeHond>();
//             }
        }
		
	}

	private void Update() {

	}
}
