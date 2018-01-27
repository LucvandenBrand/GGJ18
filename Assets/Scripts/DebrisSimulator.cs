using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisSimulator : MonoBehaviour {

	public DebrisController dc;
	public float speed;
	public float stop;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.localScale = transform.localScale * speed;
		if (transform.localScale.x < stop) {
			dc.HitPlayer(transform.position.x, transform.position.y);
			Destroy (gameObject);

		}
	}
}
