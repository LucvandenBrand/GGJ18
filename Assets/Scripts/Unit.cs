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
	public float speed;
	public float rotationSpeed;

	public bool isInfected = false;
	public int score = 0;
	public string name = "";
	public bool hasDisconnected = false;
	
	public class InfectionEvent : UnityEvent<Unit> {}
	public InfectionEvent infectionEvent = new InfectionEvent();

	public Vector2 virtualJoystick = new Vector2(0, 0);

    [SerializeField]
    private GameObject playerCollisionParticleSystem;


    [SerializeField]
    private AudioClip playerCollisionSound;
    private AudioSource audiosource;
    public int rayScore;
	
	// Use this for initialization
	void Start () {
		
		myTransform = gameObject.transform;
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
		lastPosition = myTransform.position;

		audiosource = GetComponent<AudioSource>();
// 		if (Random.value > 0.5){
// 			Infect();
// 		}
		//myTransform.LookAt(new Vector3(0, -1, 0), new Vector3(0,0,-1));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(virtualJoystick.x != 0 && virtualJoystick.y != 0) {
			//myTransform.LookAt(new Vector3(myTransform.position.x + virtualJoystick.x, myTransform.position.y + virtualJoystick.y, 0), new Vector3(0,0,-1));
		}
		addForce(virtualJoystick.x, virtualJoystick.y);
  }
  
  public void updateScore() {
	score += 1;
  }
  
  public void updateRayScore() {
        rayScore += 1;
  }

<<<<<<< HEAD
	if(!this.isInfected){
		this.score++;
		float scale = .5f + .8f * System.Math.Min((float)score, 10.0f);
		myTransform.localScale = new Vector3(scale, scale, scale); // MUST BE SMALLER THAN 5!!
	}
}
=======
    public void resetRayScore()
    {
        rayScore = 0;
    }
>>>>>>> ed38244633e25b5c6f281b9d1c2fb240542a2ae6

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

	void OnTriggerEnter2D(Collider2D other) {
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            GameObject particles = Instantiate(playerCollisionParticleSystem, Camera.main.transform);
            particles.transform.position = coll.transform.position;

            float lowPitchRange = .75F;
            float highPitchRange = 1.5F;
            float velToVol = .01F;
            float hitVol = coll.relativeVelocity.magnitude * velToVol;
            audiosource.pitch = Random.Range (lowPitchRange,highPitchRange);
            audiosource.PlayOneShot(playerCollisionSound, hitVol);
        }
    }
	

	public void Infect(){
		if(this.isInfected){
			return;
		}
		Debug.Log("Player is infected!");
		this.isInfected = true;
		
		// deactivate the shield
		// TODO watch out for the hardcoded order
		myTransform.GetChild(1).gameObject.SetActive(false);
		if (this.infectionEvent != null) {
			this.infectionEvent.Invoke(this);
		}
		// TODO Play nice animation
	}

	public void ResetPlayer(){
		this.isInfected = false;
		this.score = 0;
		myTransform.GetChild(1).gameObject.SetActive(true); // show shield.
		Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 5);
		this.transform.position = randomPos;
	}
}
