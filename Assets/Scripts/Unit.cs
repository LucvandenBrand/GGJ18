using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour {

    Transform myTransform;
    Rigidbody2D rigidbody;
    Vector3 lastPosition;
    [SerializeField]
    float speed = 40;

    bool isInfected = false;
    int score = 0;
    public string name = "";

    public class InfectionEvent : UnityEvent<Unit> {}
    public InfectionEvent infectionEvent = new InfectionEvent();

    Vector2 virtualJoystick = new Vector2(0, 0);
    

    // Use this for initialization
    void Start () {
        myTransform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        lastPosition = myTransform.position;
		
        myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
    }
	
    // Update is called once per frame
    void FixedUpdate () {
        if(virtualJoystick.x != 0 && virtualJoystick.y != 0) {
            myTransform.LookAt(new Vector3(myTransform.position.x + virtualJoystick.x, myTransform.position.y + virtualJoystick.y, 0), new Vector3(0,0,-1));
        }
        addForce(virtualJoystick.x, virtualJoystick.y);

        if(!this.isInfected){
            this.score++;
        }
    }

    public void addVirtualForce(float x_axis, float y_axis) {
        virtualJoystick = new Vector2(x_axis, y_axis);
    }

    public void addForce(float x_axis, float y_axis){
        Vector3 movement = new Vector3(x_axis, y_axis, 0);

        if (movement.magnitude > 1.0)
        {
        movement.Normalize();
        }
        // myTransform.Translate(movement.normalized * speed * Time.deltaTime );
        rigidbody.AddForce(movement * speed);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        
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
