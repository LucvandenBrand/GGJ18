using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coneGenerator : MonoBehaviour {
    public GameObject cone;
    public Material mat;
	// Use this for initialization
	void Start () {

        GameObject locCone = Instantiate(cone);
		locCone.layer = LayerMask.NameToLayer ("ConesLayer");
        locCone.GetComponent<Renderer>().material = mat;
        locCone.transform.position = transform.position;
        locCone.transform.parent = transform;
	}

}
