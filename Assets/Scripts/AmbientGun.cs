using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public float fireRate;
	private float nextFire;


	private void Start() {
		
	}

	void Update () {
		if (GetComponentInParent<Unit>().isInfected && Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Shoot();
		}
	}
	
	private void Shoot() {
		Instantiate(bullet, Vector3(0, 0, 0), Random.rotation);
	}
}
