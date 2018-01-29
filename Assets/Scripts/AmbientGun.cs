using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientGun : MonoBehaviour {

	public float fireRate;
	private float nextFire;
	public GameObject bullet;


	private void Start() {
		nextFire = Time.time;
	}

	void Update () {
// 		print("AMBIENT GUN IS WORKING");
		if (Time.time > nextFire)
		{
			nextFire = Time.time + fireRate;
			Shoot();
		}
	}
	
	private void Shoot() {
		print("BOOOOM");
		Instantiate(bullet, new Vector3(0, 0, 5), Random.rotation);
	}
}
