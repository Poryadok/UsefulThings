using UnityEngine;
using UnityEditor;

using UIBinding.Components;

[CustomEditor(typeof(GraphicFixedColorIntBinding))]
public class GraphicFixedColorIntBindingEditor : BaseBindingEditor
{
	private SerializedProperty m_enumType;

	protected override void OnEnable()
	{
		base.OnEnable();

		m_enumType = serializedObject.FindProperty("m_checkType");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var checkType = (VisibilityIntBinding.CheckType)m_enumType.enumValueIndex;
		var info = "";
		switch (checkType) {
			case VisibilityIntBinding.CheckType.Equal:
				info = "1, 2, 3";
				break;
			case VisibilityIntBinding.CheckType.Greater:
				info = "1";
				break;
			case VisibilityIntBinding.CheckType.Less:
				info = "2";
				break;
			case VisibilityIntBinding.CheckType.InRange:
				info = "1, 3";
				break;
		}
		EditorGUILayout.HelpBox(string.Format("Example value: {0}", info), MessageType.Info);

		GUILayout.BeginHorizontal();
		var color = serializedObject.FindProperty("m_colorTrue");
		EditorGUILayout.LabelField("True Color", GUILayout.Width(50f));
		color.colorValue = EditorGUILayout.ColorField(color.colorValue);
		color = serializedObject.FindProperty("m_colorFalse");
		EditorGUILayout.LabelField("False Color", GUILayout.Width(50f));
		color.colorValue = EditorGUILayout.ColorField(color.colorValue);
		GUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}