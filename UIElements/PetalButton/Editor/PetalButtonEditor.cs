using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.Editor
{
	[UnityEditor.CustomEditor(typeof(PetalButton))]
	public class PetalButtonEditor : UnityEditor.UI.ButtonEditor
	{
		public override void OnInspectorGUI()
		{
			var serializedObject = new SerializedObject(target);
			var buttons = serializedObject.FindProperty("buttons");
			var OptionPrefab = serializedObject.FindProperty("OptionPrefab");
			serializedObject.Update();
			EditorGUILayout.PropertyField(buttons, true);
			EditorGUILayout.PropertyField(OptionPrefab, true);
			serializedObject.ApplyModifiedProperties();

			//PetalButton t = (PetalButton)target;

			//t.OptionPrefab = (Button)EditorGUILayout.ObjectField("Option Prefab", t.OptionPrefab, typeof(Button), true);
			//t.buttons = (Button[])EditorGUILayout.field("Option Prefab", t.OptionPrefab, typeof(Button), true);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			base.OnInspectorGUI();
		}
	}
}