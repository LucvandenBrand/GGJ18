using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(Star))]
public class StarInspector : Editor {
	
	private static Vector3 pointSnap = Vector3.one * 0.1f;
	
	private SerializedObject star;
	private SerializedProperty center, points, frequency;

	void OnEnable () {
		star = new SerializedObject(targets);
		center = star.FindProperty("center");
		points = star.FindProperty("points");
		frequency = star.FindProperty("frequency");
	}
	
	public override void OnInspectorGUI () {
		star.Update();
		EditorGUIUtility.LookLikeInspector();
		EditorGUILayout.PropertyField(center, true);
		EditorGUILayout.PropertyField(points, true);
		EditorGUILayout.PropertyField(frequency);
		if (targets.Length == 1) {
			int totalPoints = frequency.intValue * points.arraySize;
			EditorGUI.indentLevel -= 1;
			if (totalPoints < 3) {
				EditorGUILayout.HelpBox("At least three points are needed.", MessageType.Warning);
			}
			else {
				EditorGUILayout.HelpBox(totalPoints + " points in total.", MessageType.Info);
			}
			EditorGUI.indentLevel += 1;
		}
		if (star.ApplyModifiedProperties() ||
			(Event.current.type == EventType.ValidateCommand &&
			Event.current.commandName == "UndoRedoPerformed")) {
			foreach (Star s in targets) {
				if (PrefabUtility.GetPrefabType(s) != PrefabType.Prefab){
					s.UpdateMesh();
				}
			}
		}
	}
	
	void OnSceneGUI () {
		Star star = (Star)target;
		Transform starTransform = star.transform;
		Undo.SetSnapshotTarget(star, "Move Star Point");
		float angle = -360f / (star.frequency * star.points.Length);
		for(int i = 0; i < star.points.Length; i++){
			Quaternion rotation = Quaternion.Euler(0f, 0f, angle * i);
			Vector3
				oldPoint = starTransform.TransformPoint(rotation * star.points[i].position),
				newPoint = Handles.FreeMoveHandle(
					oldPoint, Quaternion.identity, 0.02f, pointSnap, Handles.DotCap);
			if(oldPoint != newPoint){
				star.points[i].position = Quaternion.Inverse(rotation) *
					starTransform.InverseTransformPoint(newPoint);
				star.UpdateMesh();
			}
		}
	}
}