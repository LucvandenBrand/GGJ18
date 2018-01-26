using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ShiftMaterialFloat : MonoBehaviour {

    [SerializeField]
    private string[] paramNames;
    [SerializeField]
    private float[] paramSpeeds;

    private MeshRenderer meshRenderer;
    private float[] curValues;

	// Use this for initialization
	void Start () {
        curValues = new float[paramNames.Length];
        for (int i = 0; i < curValues.Length; i++)
            curValues[i] = 0;
        meshRenderer = GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i=0; i<paramNames.Length; i++)
        {
            float translation = Time.deltaTime * paramSpeeds[i];
            curValues[i] += translation;
            meshRenderer.material.SetFloat(paramNames[i], curValues[i]);
        }
	}
}
