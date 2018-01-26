using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShiftMaterialFloat : MonoBehaviour {

    [SerializeField]
    private string[] paramNames;
    [SerializeField]
    private float[] paramSpeeds;

    // The renderer to get the material from.
    private MeshRenderer meshRenderer;

    // Keeping track of all the param values.
    private float[] curValues;

	// At start, initialize the array to 0.
	void Start () {
        curValues = new float[paramNames.Length];
        for (int i = 0; i < curValues.Length; i++)
            curValues[i] = 0;
        meshRenderer = GetComponent<MeshRenderer>();
    }
	
	// Every frame update the params based on their speed.
	void Update () {
        for (int i=0; i<paramNames.Length; i++)
        {
            float translation = Time.deltaTime * paramSpeeds[i];
            curValues[i] += translation;
            meshRenderer.material.SetFloat(paramNames[i], curValues[i]);
        }
	}
}
