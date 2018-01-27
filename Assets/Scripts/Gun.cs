using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public Transform shootPosition;
	public GameObject bullet;
    public string triggerName;
    public float shootPower;
    public float fireRate;
    private float nextFire;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis(triggerName) > 0.50 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            DoShot();
        }
    }

    void DoShot()
    {
        GameObject curBullet = (GameObject)Instantiate(bullet, transform.position + (transform.up), transform.rotation);
        //curBullet.transform.rotation = transform.rotation;
        Rigidbody rb = curBullet.GetComponent<Rigidbody>();
        rb.AddRelativeForce(transform.up * shootPower);
    }
}
