using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeGenerator : MonoBehaviour {
    public GameObject cone;
	private Rigidbody rb;

    public Material[] voronoiColors;
    private static int voronoiCounter = 0;

	// Use this for initialization
	void Start () {
        GameObject locCone = Instantiate(cone);

		locCone.layer = LayerMask.NameToLayer ("ConesLayer");
        locCone.GetComponent<Renderer>().material = voronoiColors[voronoiCounter++];
        locCone.transform.position = transform.position;
        locCone.transform.parent = transform;
    }
}
