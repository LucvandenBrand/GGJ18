using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	[SerializeField] Transform shootPosition;
	[SerializeField] GameObject bullet;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Instantiate(bullet, shootPosition.position, Quaternion.identity);
		}
	}
}
