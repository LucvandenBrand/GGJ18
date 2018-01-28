using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeMaker : MonoBehaviour {
	public GameObject cone;
	private Rigidbody rb;

	public Material[] voronoiColors;
	private static int voronoiCounter = 0;
	private static int voronoiBackCounter = 0;

	// Use this for initialization
	public void makeCone(bool countBack=false) {
		GameObject locCone = Instantiate(cone);

		locCone.layer = LayerMask.NameToLayer ("ConesLayer");
		if (countBack){
			locCone.GetComponent<Renderer>().material = voronoiColors[voronoiColors.Length - 1 - voronoiBackCounter];
			voronoiBackCounter = (voronoiBackCounter + 1) % voronoiColors.Length;
		} else {
			locCone.GetComponent<Renderer>().material = voronoiColors[voronoiCounter];
			voronoiCounter = (voronoiCounter + 1) % voronoiColors.Length;
		}
		locCone.transform.position = transform.position;
		locCone.transform.parent = transform;
	}
}
