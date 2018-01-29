using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPoint))]
public class ColorPointDrawer : PropertyDrawer {
	
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		return property.isExpanded && label != GUIContent.none ? 32f : 16f;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		position.height = 16f;
		if (label != GUIContent.none) {
			Rect foldoutPosition = position;
			foldoutPosition.x -= 14f;
			foldoutPosition.width += 14f;
			label = EditorGUI.BeginProperty(position, label, property);
			property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label, true);
			EditorGUI.EndProperty();
			
			if (!property.isExpanded) {
				return;
			}
			position.y += 16f;
		}
		position = EditorGUI.IndentedRect(position);
		position.width /= 4f;
		int oldIndentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.LookLikeControls(12f);
		EditorGUI.PropertyField(position, property.FindPropertyRelative("position.x"));
		position.x += position.width;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("position.y"));
		position.x += position.width;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("position.z"));
		position.x += position.width;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("color"), new GUIContent("C"));
		EditorGUI.indentLevel = oldIndentLevel;
	}
}