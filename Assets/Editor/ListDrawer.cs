using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ListAttribute))]
public class ListDrawer : PropertyDrawer {
	
	private const float buttonWidth = 20f, buttonHeight = 15f, buttonMargin = 1f;
	
	private static GUIContent
		moveButtonContent = new GUIContent("\u21b4", "move down"),
		duplicateButtonContent = new GUIContent("+", "duplicate"),
		deleteButtonContent = new GUIContent("-", "delete"),
		addButtonContent = new GUIContent("+", "add element");
	
	private new ListAttribute attribute {
		get {
			return base.attribute as ListAttribute;
		}
	}
	
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		float height = 0f;
		if (attribute.showListLabel && label != GUIContent.none) {
			height = 16f;
			if (!property.isExpanded) {
				return height;
			}
		}
		if (!property.isArray || property.FindPropertyRelative("Array.size").hasMultipleDifferentValues) {
			return height + 20f;
		}
		if (attribute.showSize) {
			height += 16f;
		}
		if (property.arraySize == 0 && attribute.showButtons) {
			return height + 16f;
		}
		if (attribute.showElementLabels) {
			for (int i = 0; i < property.arraySize; i++) {
				height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
			}
		}
		else {
			for (int i = 0; i < property.arraySize; i++) {
				height += EditorGUI.GetPropertyHeight(
					property.GetArrayElementAtIndex(i), GUIContent.none);
			}
		}
		return height;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		bool showFoldout = attribute.showListLabel && label != GUIContent.none;
		position.height = 16f;
		if (showFoldout) {
			ShowFoldout(position, property, label);
			position.y += 16f;
			
			if (!property.isExpanded) {
				return;
			}
		}
		if (!property.isArray) {
			ShowError(position);
			return;
		}
		SerializedProperty size = property.FindPropertyRelative("Array.size");
		if (size.hasMultipleDifferentValues) {
			ShowDifferentSizesText(position);
			return;
		}
		
		if (showFoldout) {
			EditorGUI.indentLevel += 1;
		}
		if (attribute.showSize) {
			EditorGUI.PropertyField(position, size);
			position.y += 16f;
		}
		if (attribute.showButtons) {
			position.width -= buttonWidth * 3 + buttonMargin * 2;
		}
		if (attribute.showButtons && size.intValue == 0) {
			ShowAddButton(position, property);
		}
		else {
			ShowElements(position, property);
		}
		if (showFoldout) {
			EditorGUI.indentLevel -= 1;
		}
	}
	
	private void ShowFoldout (Rect position, SerializedProperty property, GUIContent label) {
		position.x -= 14f;
		position.width += 14f;
		label = EditorGUI.BeginProperty(position, label, property);
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
		EditorGUI.EndProperty();
	}
	
	private void ShowError (Rect position) {
		position.x += 2f;
		position.width -= 4f;
		position.height = 20f;
		EditorGUI.HelpBox(position, "Property is not an array nor a list.", MessageType.Error);
	}
	
	private void ShowDifferentSizesText (Rect position) {
		position.x += 2;
		position.width -= 4f;
		position.height = 20f;
		EditorGUI.HelpBox(position, "Not showing lists with different sizes.", MessageType.Info);
	}
	
	private void ShowAddButton (Rect position, SerializedProperty property) {
		position.height = buttonHeight;
		if (GUI.Button(EditorGUI.IndentedRect(position), addButtonContent, EditorStyles.miniButton)) {
			property.arraySize += 1;
		}
	}
	
	private void ShowElements (Rect position, SerializedProperty property) {
		bool
			showButtons = attribute.showButtons,
			showElementLabels = attribute.showElementLabels;
		Rect buttonPosition = new Rect(position.xMax + buttonMargin, position.y, buttonWidth, buttonHeight);
		for (int i = 0; i < property.arraySize; i++) {
			SerializedProperty element = property.GetArrayElementAtIndex(i);
			if (showElementLabels) {
				position.height = EditorGUI.GetPropertyHeight(element);
				EditorGUI.PropertyField(position, element, true);
			}
			else {
				position.height = EditorGUI.GetPropertyHeight(element, GUIContent.none);
				EditorGUI.PropertyField(position, element, GUIContent.none, true);
			}
			if (showButtons) {
				ShowButtons(buttonPosition, property, i);
			}
			buttonPosition.y = position.y += position.height;
		}
	}
	
	private void ShowButtons (Rect position, SerializedProperty property, int index) {
		if (GUI.Button(position, moveButtonContent, EditorStyles.miniButtonLeft)) {
			property.MoveArrayElement(index, index + 1);
		}
		position.x += buttonWidth;
		if (GUI.Button(position, duplicateButtonContent, EditorStyles.miniButtonMid)) {
			property.InsertArrayElementAtIndex(index);
		}
		position.x += buttonWidth;
		if (GUI.Button(position, deleteButtonContent, EditorStyles.miniButtonRight)) {
			property.DeleteArrayElementAtIndex(index);
		}
	}
}