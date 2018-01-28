using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortCellSphere : MonoBehaviour {
    MeshFilter meshfilter;
    float phase;

	// Use this for initialization
	void Start () {
      phase = Random.Range(0f, 24f);
      meshfilter = GetComponent<MeshFilter>();

      // Mesh mesh = GetComponent<MeshFilter>().mesh;
      // Vector3[] vertices = mesh.vertices;
      // Vector3[] normals = mesh.normals;
      // int i = 0;
      // while (i < vertices.Length) {
      //     vertices[i] += 0.02f * normals[i] * Random.Range(-1f, 1f);
      //     ++i;
      // }
      // mesh.vertices = vertices;
	}
	
	// Update is called once per frame
	void Update () {
      Mesh mesh = GetComponent<MeshFilter>().mesh;
      Vector3[] vertices = mesh.vertices;
      Vector3[] normals = mesh.normals;
      int i = 0;
      while (i < vertices.Length) {
          vertices[i] += 0.004f * normals[i] * Mathf.Sin(4f * Time.time + 3 * i + phase);
          ++i;
      }
      mesh.vertices = vertices;
	}
}
