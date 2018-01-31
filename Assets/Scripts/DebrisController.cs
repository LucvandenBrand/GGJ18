using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisController : MonoBehaviour {

//	public GameObject[] cells;
	// Use this for initialization

	public float DebreeInterval = 15.0f;
	public float tChange = 10.0f;
	public GameObject debris;

	void Start () {
		
	}

	void Update () {
		tChange -= Time.deltaTime;

		if (tChange <= 0) {
			tChange = Random.Range (DebreeInterval, DebreeInterval + 5.0f);
			if (GetComponent<GameTimer>().isRunning()){
				float randomX = Random.Range (-8.0f, 8.0f);
				float randomY = Random.Range (-4.5f, 4.5f);
				GameObject currDebris = Instantiate (debris, new Vector3(randomX, randomY, 10.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
				currDebris.GetComponent<DebrisSimulator> ().dc = this;
			}
		}
	}

	public void HitPlayer(float x, float y)
	{
		GameObject hittedPlayer = this.findClosestPlayer (x, y);

		if(hittedPlayer != null) {
			hittedPlayer.GetComponent<Unit>().SuperPower ();
		}

	}

	public GameObject findClosestPlayer (float x, float y) 
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		float closestDistance = float.MaxValue;
		GameObject selectedPlayer = null;
		foreach(GameObject player in players ) {
			Vector3 pos = player.transform.position;
			float distance = Mathf.Sqrt (Mathf.Pow(pos.x - x, 2) + Mathf.Pow(pos.y - y, 2));
			if (closestDistance > distance) {
				closestDistance = distance;
				selectedPlayer = player;
			}
		}
		return selectedPlayer;
	}
}
