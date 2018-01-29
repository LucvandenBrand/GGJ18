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
	Material SuperPowerMaterial;
	[SerializeField]
	Material InnerMaterial;

	[SerializeField]
	public float speed;
	public float rotationSpeed;

	public bool isInfected = false;
	public int score = 0;
	public string name = "";
	public bool hasDisconnected = false;
	public bool paralyze = false;
	public bool superpower = false;

	private float interval = 10.0f;
	private float sChange = 0.0f;
	private float pChange = 0.0f;

	public class InfectionEvent : UnityEvent<Unit> {}
	public InfectionEvent infectionEvent = new InfectionEvent();

	public Vector2 virtualJoystick = new Vector2(0, 0);

    [SerializeField]
    private GameObject playerCollisionParticleSystem;


    [SerializeField]
    private AudioClip playerCollisionSound;
    private AudioSource audiosource;
    public int rayScore;


    [System.Serializable]
    public class Yell {
        public string text;
        public AudioClip scoreSound;
    }

    [SerializeField]
    public Yell[] scoreSounds;

	
	// Use this for initialization
	void Start () {
		
		myTransform = gameObject.transform;
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
		lastPosition = myTransform.position;

		audiosource = GetComponent<AudioSource>();
		
		setScale();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(virtualJoystick.x != 0 && virtualJoystick.y != 0) {
			//myTransform.LookAt(new Vector3(myTransform.position.x + virtualJoystick.x, myTransform.position.y + virtualJoystick.y, 0), new Vector3(0,0,-1));
		}
		addForce(virtualJoystick.x, virtualJoystick.y);
	}

	public void Update() {
		sChange -= Time.deltaTime;
		pChange -= Time.deltaTime;
		if (pChange <= 0 && paralyze) {
			paralyze = false;
			gameObject.transform.GetChild (1).transform.gameObject.SetActive (true);
		}
		if (sChange <= 0 && superpower) {
			gameObject.transform.GetChild(0).GetComponent<MeshRenderer> ().material = InnerMaterial;
			superpower = false;
		}
	}	
	public void updateScore() {
		score += 1;
		setScale();

    maybePlayScoreSound();
	}

    private void maybePlayScoreSound(){
        if(Random.Range(0, 3) < 1) {
            AudioClip randomClip = scoreSounds[Random.Range(0, scoreSounds.Length)].scoreSound;
            Debug.Log(randomClip);
            audiosource.pitch = 1;
            audiosource.PlayOneShot(randomClip, 0.5f);
        }
    }
	
	void setScale(){
		float scale = .7f + .3f * System.Math.Min((float)score, 10.0f);
		myTransform.localScale = new Vector3(scale, scale, scale); // MUST BE SMALLER THAN 5!!
		rigidbody.mass = scale;
	}
	
	public void updateRayScore() {
		rayScore += 1;
	}
	public void resetRayScore()
	{
		rayScore = 0;
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

			if (superpower) {
				coll.gameObject.GetComponent<Unit>().Paralyze ();
			}
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

	public void SuperPower() {
		gameObject.transform.GetChild(0).GetComponent<MeshRenderer> ().material = SuperPowerMaterial;
		superpower = true;
		sChange = interval;
	}

	public void Paralyze() {
		if (!superpower) {
			paralyze = true;
			pChange = interval;
			gameObject.transform.GetChild (1).transform.gameObject.SetActive (false);
		}
	}

//	public IEnumerable Paralyze(){
//		yield return true;
//	}

	public void ResetPlayer(){
		this.isInfected = false;
		this.paralyze = false;
		this.superpower = false;
		this.score = 0;
        this.transform.position = new Vector3(10, UnityEngine.Random.Range(-1f, 1f), 5);
        setScale();
	}
}
