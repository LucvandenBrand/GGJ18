using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    Transform myTransform;
    Rigidbody2D rigidbody;
    Vector3 lastPosition;
    [SerializeField]
    float speed = 40;

    Vector2 virtualJoystick = new Vector2(0, 0);
    

    // Use this for initialization
    void Start () {
        myTransform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        lastPosition = myTransform.position;
		
        myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
    }
	
    // Update is called once per frame
    void Update () {
        // addForce(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        addForce(virtualJoystick.x, virtualJoystick.y);

        if (Vector3.Distance(myTransform.position, lastPosition) > 0.03){
            myTransform.LookAt(new Vector3(lastPosition.x, lastPosition.y, 0), new Vector3(0,0,-1));
        }
        lastPosition = myTransform.position;
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
}
