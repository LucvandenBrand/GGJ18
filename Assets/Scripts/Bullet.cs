using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

	Rigidbody2D myRigidbody;
	[SerializeField]
	float fireSpeed = 5;

	private Vector3 oldVelocity;
	private Transform myTransform;



	void Awake() {
		myRigidbody = gameObject.GetComponent<Rigidbody2D>();
		myTransform = transform;
	}

	void Start () {
		myRigidbody.velocity = Vector3.forward * fireSpeed;
         
		// freeze the rotation so it doesnt go spinning after a collision
		myRigidbody.freezeRotation = true;
		 
		myRigidbody.AddRelativeForce(new Vector2(0, 1) * fireSpeed);

    Destroy(this.gameObject, 10);
	}

	void FixedUpdate () { // Store the velocity every physics update this may be multiple times per frame
		oldVelocity = myRigidbody.velocity;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "Shield") {
			print("JHEEEEEEEEEEEEE");
// 			Destroy(this.gameObject, 0.001f);
			ContactPoint2D contact = collision.contacts[0];
			
			Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);        
			
			myRigidbody.velocity = reflectedVelocity;
			
			Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
			myTransform.rotation = rotation * myTransform.rotation;
		} 
		if (collision.gameObject.tag == "Edges") { // todo: tag parent instead of 4 edge objects
			Destroy(this.gameObject, 0.001f);
// 			SetActive(false); // destroy probably doesn't work...
		}
		if (collision.gameObject.tag == "Player") {
			Unit unit = collision.gameObject.GetComponent<Unit>();
			if (!unit.isInfected){
				Destroy(this.gameObject, 0.001f);
				unit.Infect();
			}
		}

	}
}
