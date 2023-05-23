using UnityEditor;

using UIBinding.Components;

[CustomEditor(typeof(VisibilityFloatBinding)), CanEditMultipleObjects]
public class VisibilityFloatBindingEditor : Editor
{
	private SerializedProperty m_enumType;

	private void OnEnable()
	{
		m_enumType = serializedObject.FindProperty("m_checkType");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var checkType = (VisibilityFloatBinding.CheckType)m_enumType.enumValueIndex;
		var info = "";
		switch (checkType)
		{
			case VisibilityFloatBinding.CheckType.Equal:
				info = "1, 2, 3";
				break;
			case VisibilityFloatBinding.CheckType.Greater:
				info = "1";
				break;
			case VisibilityFloatBinding.CheckType.Less:
				info = "2";
				break;
			case VisibilityFloatBinding.CheckType.InRange:
				info = "1, 3";
				break;
		}
		EditorGUILayout.HelpBox(string.Format("Example value: {0}", info), MessageType.Info);
	}
}