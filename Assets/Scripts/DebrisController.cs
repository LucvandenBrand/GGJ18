using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisController : MonoBehaviour {

	public GameObject[] cells;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

//	GameObject findClosestCell () 
//	{
//		cells = GameObject.FindGameObjectsWithTag("Cells");
//		GameObject selectedCell;
//		float closestDistance = float.MaxValue;
//		foreach(GameObject cell in cells ) {
//			var pos = cell.transform.position;
//			float distance = Mathf.Sqrt (Mathf.Pow(pos.x - transform.position.x, 2) + Mathf.Pow(pos.y - transform.position.y, 2));
//			if (closestDistance > distance) {
//				closestDistance = distance;
//				selectedCell = cell;
//			}
//		}
//		return closestDistance;
//	}
}
