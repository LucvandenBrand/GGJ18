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
        Instantiate(bullet, shootPosition.position, transform.rotation);
        //curBullet.transform.rotation = transform.rotation;
        // Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        // rb.AddRelativeForce(transform.up * shootPower);
    }
}
