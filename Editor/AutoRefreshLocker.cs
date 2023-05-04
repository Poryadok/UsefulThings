using UnityEditor;
using UnityEngine;

namespace PM.UsefulThings.Editor
{
	public class AutoRefreshLocker : UnityEditor.Editor
	{
		[MenuItem("UT/Editor/Auto Refresh")]
		private static void AutoRefreshToggle()
		{
			var status = EditorPrefs.GetInt("kAutoRefresh");

			EditorPrefs.SetInt("kAutoRefresh", status == 1 ? 0 : 1);
		}

		[MenuItem("UT/Editor/Auto Refresh", true)]
		private static bool AutoRefreshToggleValidation()
		{
			var status = EditorPrefs.GetInt("kAutoRefresh");

			Menu.SetChecked("UT/Editor/Auto Refresh", status == 1);

			return true;
		}
		
		[MenuItem("UT/Editor/Lock Reload")]
		private static void LockReloadToggle()
		{
			var status = EditorPrefs.GetInt("lockReload");

			EditorPrefs.SetInt("lockReload",status == 1 ? 0 : 1);
		}

		[MenuItem("UT/Editor/Lock Reload", true)]
		private static bool LockReloadToggleValidation()
		{
			var status = EditorPrefs.GetInt("lockReload");

			Menu.SetChecked("UT/Editor/Lock Reload", status == 1);

			return true;
		}

		[MenuItem("UT/Editor/Refresh %t")]
		private static void Refresh()
		{
			Debug.Log("Request script reload.");

			EditorApplication.UnlockReloadAssemblies();

			AssetDatabase.Refresh();

			EditorUtility.RequestScriptReload();
		}

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			AssetDatabase.SaveAssets();

			var status = EditorPrefs.GetInt("lockReload");

			if (status == 1)
			{
				Debug.Log("Script reloaded! Assembly reload locked!");

				EditorApplication.LockReloadAssemblies();
			}
		}
	}
}