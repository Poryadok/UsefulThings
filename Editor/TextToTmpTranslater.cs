using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Assets.Editor
{
	public class TextToTmpTranslater : EditorWindow
	{
		bool showCount = false;
		int count = 0;
		private Vector2 scrollPos;

		[MenuItem("Window/Editor/TextToTmpTranslater")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow<TextToTmpTranslater>("TTTT");
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical();

			if (GUILayout.Button("Translate"))
			{
				var list = Resources.LoadAll<GameObject>("").ToList();

				count = 0;

				string text = null;
				int size = 0;
				GameObject go = null;
				TextMeshProUGUI newText = null;

				foreach (var item in list)
				{
					var path = "UIWindows/Gameplay/Panels/" + item.name;
					GameObject prefab = null;
					if (Resources.Load(path) != null)
					{
						prefab = PrefabUtility.LoadPrefabContents("CurrentProject/Resources/" + path);
					}
					else
					{
						path = "UIWindows/General/Panels/" + item.name;
						if (Resources.Load(path) != null)
						{
							prefab = PrefabUtility.LoadPrefabContents("CurrentProject/Resources/" + path);
						}
						else
						{
							path = "UIWindows/Menu/Panels/" + item.name;
							if (Resources.Load(path) != null)
							{
								prefab = PrefabUtility.LoadPrefabContents("CurrentProject/Resources/" + path);
							}
							else
							{
								continue;
							}
						}
					}

					if (prefab == null)
					{
						continue;
					}

					var oldTexts = item.transform.GetComponentsInChildren<Text>();

					foreach (var textComp in oldTexts)
					{
						text = textComp.text;
						size = textComp.fontSize;
						go = textComp.gameObject;
						DestroyImmediate(textComp, true);

						newText = go.AddComponent<TextMeshProUGUI>();
						newText.fontSize = size;
						newText.text = text;
						count++;
					}
				}

				showCount = true;
			}

			if (showCount)
			{
				GUILayout.Label("ChangedElements: " + count, EditorStyles.boldLabel);
			}

			GUILayout.EndVertical();
		}
	}
}