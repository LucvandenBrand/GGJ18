using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour {
	
	public ColorPoint center;
	
//	[List(showSize = false, showElementLabels = false)]
	public ColorPoint[] points;
	
//	[UnityEngine.Range(1, 20)]
	public int frequency = 1;
	
	private Mesh mesh;
	private Vector3[] vertices;
	private Color[] colors;
	private int[] triangles;

	void OnEnable () {
		UpdateMesh();
	}
	
	void Reset () {
		UpdateMesh();
	}
	
	public void UpdateMesh () {
		if (mesh == null) {
			GetComponent<MeshFilter>().mesh = mesh = new Mesh();
			mesh.name = "Star Mesh";
			mesh.hideFlags = HideFlags.HideAndDontSave;
		}
		
		if (frequency < 1) {
			frequency = 1;
		}
		if (points == null) {
			points = new ColorPoint[0];
		}
		int numberOfPoints = frequency * points.Length;
		if (vertices == null || vertices.Length != numberOfPoints + 1) {
			vertices = new Vector3[numberOfPoints + 1];
			colors = new Color[numberOfPoints + 1];
			triangles = new int[numberOfPoints * 3];
			mesh.Clear();
		}
		
		if (numberOfPoints >= 3) {
			vertices[0] = center.position;
			colors[0] = center.color;
			float angle = -360f / numberOfPoints;
			for(int repetitions = 0, v = 1, t = 1; repetitions < frequency; repetitions++){
				for(int p = 0; p < points.Length; p += 1, v += 1, t += 3){
					vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) * points[p].position;
					colors[v] = points[p].color;
					triangles[t] = v;
					triangles[t + 1] = v + 1;
				}
			}
			triangles[triangles.Length - 1] = 1;
		}
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.triangles = triangles;
	}
}