using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    Transform myTransform;
    Rigidbody2D rigidbody;
    Vector3 lastPosition;
    [SerializeField]
    public float speed;
    public float rotationSpeed;

    public Vector2 virtualJoystick = new Vector2(0, 0);
    
    
    // Use this for initialization
    void Start () {
        myTransform = gameObject.transform;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        lastPosition = myTransform.position;
		
        //myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
    }
	
    // Update is called once per frame
    void FixedUpdate () {
        // addForce(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(virtualJoystick.x != 0 && virtualJoystick.y != 0) {
            //myTransform.LookAt(new Vector3(myTransform.position.x + virtualJoystick.x, myTransform.position.y + virtualJoystick.y, 0), new Vector3(0,0,-1));
        }
        addForce(virtualJoystick.x, virtualJoystick.y);

    //     if (Vector3.Distance(myTransform.position, lastPosition) > 0.03){
    //         myTransform.LookAt(new Vector3(lastPosition.x, lastPosition.y, 0), new Vector3(0,0,-1));
    //     }
    //     lastPosition = myTransform.position;
    // }
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
        rigidbody.AddForce(movement * speed * Time.deltaTime);

        if (movement.magnitude > 0.01f)
        {
            float rot_z = Mathf.Atan2(y_axis, x_axis) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
    }
}
