using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAI : MonoBehaviour {

	private Rigidbody rb;
	public int thrust;
	public int turnpower;
	public int friction;
	public int maxSpeed = 5;

	private float rotateChange  = 0.0f;
	private float speedChange  = 0.0f;

	private float randomRotation = 0.0f;
	private float randomSpeed = 0.0f;
	private float normlizeDistance = 2;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	void Update () {

		if(rb.velocity.magnitude > maxSpeed){
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}

		if (Time.time >= rotateChange){
			randomRotation = Random.Range(-1.0f,1.0f);
			rotateChange = Time.time + Random.Range(0.5f,1.5f);
		}

		if (Time.time >= speedChange){
			randomSpeed = Random.Range(0.0f,1.0f);
			speedChange = Time.time + Random.Range(0.5f,2.5f);
		}

		rb.AddRelativeForce(transform.up * thrust * speedChange);
		rb.drag = friction;

		float rotator = (transform.rotation.w - (AngleTo (Vector3.zero, transform.position)/ 90)) * -1;

		float normalized = Mathf.Pow(Vector3.Distance (Vector3.zero, transform.position), 2) / 10;
//		Debug.Log (normalized);
		float randomForce = ((turnpower * randomRotation) / normalized);
		float centerForce = (rotator * normalized);
		Debug.Log (normalized + " " + randomForce + " " + centerForce);
		transform.Rotate (Vector3.forward * centerForce * randomForce);
	}

	private float AngleTo(Vector2 pos, Vector2 target)
	{
		Vector2 diference = target - pos;
		float sign = (target.y < pos.y) ? -1.0f : 1.0f;
		return Vector2.Angle(Vector2.right, diference) * sign;
	}
}
