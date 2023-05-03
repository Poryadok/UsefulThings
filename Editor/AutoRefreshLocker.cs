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

			Menu.SetChecked("Sklapps/Editor/Auto Refresh", status == 1);

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
			Debug.Log("Script realoded!");

			AssetDatabase.SaveAssets();

			EditorApplication.LockReloadAssemblies();
		}
	}
}