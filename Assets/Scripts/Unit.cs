using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	Transform myTransform;
	Rigidbody2D rigidbody;
	[SerializeField]
	float speed = 10;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		// myTransform.Translate(movement.normalized * speed * Time.deltaTime );
		rigidbody.AddForce(movement * speed);
	}
}
