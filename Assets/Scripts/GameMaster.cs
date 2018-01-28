using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	// List<Unit> units = new List<Unit>();
	[SerializeField] public LayerMask layerMask;
	[SerializeField] public float rayLength = 100;

	[SerializeField] int width, height;

	[SerializeField] NetworkController networkController;

	public Unit DetermanScore() {
		for (int x=0; x <= width; x++) {
			for (int y=0; y <= height; y++) {
				Vector3 test = new Vector3((1 / (float)width) * x, (1 / (float)height) * y, 0);

				RaycastHit hit;

				Ray ray = Camera.main.ViewportPointToRay(test);
				Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow, 500f);	

				if(Physics.Raycast(ray, out hit, rayLength, layerMask)) {
					if (hit.transform.gameObject != null) {
						hit.transform.gameObject.GetComponentInParent<Unit>().updateRayScore();
					} else {
						Debug.LogError("RAAR");
					}
				}
			}
		}

		List<Unit> players = new List<Unit>(networkController.players.Values);
		Unit highScoreUnit = null;

		foreach (var player in players)
		{
			Debug.Log("rayscore" + player.name + " " + player.rayScore);
			if (highScoreUnit == null || player.rayScore > highScoreUnit.rayScore)
				highScoreUnit = player;
		}
		foreach (var player in players)
			player.resetRayScore();
		if (highScoreUnit != null){
			StartCoroutine(toggleConeEffect(highScoreUnit, players));
			highScoreUnit.updateScore();
		}
		return highScoreUnit;
	}

	IEnumerator toggleConeEffect(Unit highScoreUnit, List<Unit> players){
		foreach (var player in players)
			if(player != highScoreUnit) {
				player.transform.GetChild(1).gameObject.SetActive(false);
			}
		yield return new WaitForSeconds(1);
		
		foreach (var player in players)
			if(player != highScoreUnit) {
				player.transform.GetChild(1).gameObject.SetActive(true);
			}
	}
	
	public void ResetPlayers() {
		List<Unit> players = new List<Unit>(networkController.players.Values);
		foreach (var player in players) {
			player.ResetPlayer();
            player.transform.position = new Vector3(10, UnityEngine.Random.Range(-1f, 1f), 5);
        }
	}

}
