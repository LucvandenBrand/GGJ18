using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletHitReceiver : MonoBehaviour {

	void OnCollision(Collision col)
    {
        Debug.Log("COLLISION with:" + col.gameObject.name);
        //Improve later if we need more/iterative deseases
        if (col.gameObject.name == "RodeHondBullet")
        {
            gameObject.AddComponent<RodeHond>();
        }
    }
}
