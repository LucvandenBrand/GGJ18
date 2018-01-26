using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	Transform myTransform;
	Rigidbody2D rigidbody;
	Vector3 lastPosition;
	[SerializeField]
	float speed = 10;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
		lastPosition = myTransform.position;
		
		myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
		// myTransform.Translate(movement.normalized * speed * Time.deltaTime );
		rigidbody.AddForce(movement * speed);
		if (Vector3.Distance(myTransform.position, lastPosition) > 0.01){
			myTransform.LookAt(new Vector3(lastPosition.x, lastPosition.y, 0), new Vector3(0,0,-1));
		}
		lastPosition = myTransform.position;
	}
}
