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
        Unit highScoreUnit = players[0];

        foreach (var player in players)
        {
            Debug.Log("rayscore" + player.name + " " + player.rayScore);
            if (player.rayScore > highScoreUnit.rayScore)
                highScoreUnit = player;
        }
        foreach (var player in players)
            player.resetRayScore();
        StartCoroutine(toggleConeEffect(highScoreUnit, players));
        ++highScoreUnit.score;
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

    private void ResetScore() {
        List<Unit> players = new List<Unit>(networkController.players.Values);
        foreach (var player in players) {
            player.score = 0;
        }
    }

	// private void Update() {
    //     if(Input.GetKeyDown(KeyCode.Space)) {
    //         Debug.LogWarning("WINNAAR "+DetermanScore().name +" "+ DetermanScore().score);
    //     }
	// }
}
