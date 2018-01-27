using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour {

	private Rigidbody rb;
	public int thrust;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update () {

		float horizontalAxis = Input.GetAxis ("Horizontal");
		float verticalAxis = Input.GetAxis ("Vertical");
		Vector3 vec = new Vector3 (horizontalAxis, verticalAxis, 0.0f);

//		Debug.Log (AngleTo(Vector3.zero, transform.position));
		rb.AddRelativeForce(vec * thrust);
	}


}
