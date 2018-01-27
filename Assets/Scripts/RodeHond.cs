using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodeHond : Gun {

	// Use this for initialization
	void Start () {
        shootPower = 20;
        fireRate = 0.2f;
        triggerName = "Fire1";
        shootPosition = transform;
        bullet = (GameObject)Resources.Load("RodeHondBullet", typeof(GameObject));
        //To set the sphere to red
        transform.GetChild(0).GetComponent<Renderer>().material = (Material)Resources.Load("RodeHondMaterial", typeof(Material));
    }
}
