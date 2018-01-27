﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour {

    Transform myTransform;
    Rigidbody2D rigidbody;
    Vector3 lastPosition;
    [SerializeField]
    public float speed;
    public float rotationSpeed;

    public bool isInfected = false;
    int score = 0;
    public string name = "";

    public class InfectionEvent : UnityEvent<Unit> {}
    public InfectionEvent infectionEvent = new InfectionEvent();

    public Vector2 virtualJoystick = new Vector2(0, 0);
    
    
    // Use this for initialization
    void Start () {
        isInfected = Random.value > 0.5;
        myTransform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        lastPosition = myTransform.position;
		
        //myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
    }
	
    // Update is called once per frame
    void FixedUpdate () {
        if(virtualJoystick.x != 0 && virtualJoystick.y != 0) {
            //myTransform.LookAt(new Vector3(myTransform.position.x + virtualJoystick.x, myTransform.position.y + virtualJoystick.y, 0), new Vector3(0,0,-1));
        }
        addForce(virtualJoystick.x, virtualJoystick.y);

        if(!this.isInfected){
            this.score++;
        }
    }
    

    public void addVirtualForce(float x_axis, float y_axis) {
        virtualJoystick = new Vector2(x_axis, y_axis);
    }

    public void addForce(float x_axis, float y_axis) {
        Vector3 movement = new Vector3(x_axis, y_axis, 0);

        if (movement.magnitude > 1.0)
        {
            movement.Normalize();
        }
        rigidbody.AddForce(movement * speed);

        if (movement.magnitude > 0.01f)
        {
            float rot_z = Mathf.Atan2(y_axis, x_axis) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
    }

   void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Bullet") {
// 			Debug.LogError("JHEEEEEEEEEEEEE");
			
			
			
			if (!isInfected){
				Destroy(other.gameObject);
				isInfected = true;
				// todo: notify something
			}
		}
    }

    void OnTriggerEnter2D(Collider2D other) {
    }

    public void Infect(){
        if(this.isInfected){
            return;
        }
        Debug.Log("Player is infected!");
        this.isInfected = true;
        if (this.infectionEvent != null) {
            this.infectionEvent.Invoke(this);
        }
        // TODO Play nice animation
    }
}
