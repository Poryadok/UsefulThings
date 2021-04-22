using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace PM.UsefulThings
{
	public class LocalizationManagerEditor : UnityEditor.Editor
	{
		private static Dictionary<string, IEnumerator> downloaders = new Dictionary<string, IEnumerator>();
		private static int ticks;

		[MenuItem("Tools/Localization/Download")]
		private static void Init()
		{
			var localesIds = Resources.LoadAll<LocalizationLocaleGoogleIds>("")[0];
			var locales = localesIds.Locales;
			var linkPostfix = "?gid={0}&single=true&output=csv";
			string link = localesIds.Link + linkPostfix;
			string path = $"Assets/CurrentProject/Resources/Localizations";

			foreach (var locale in locales)
			{
				var url = string.Format(link, locale.Id);

				downloaders.Add(url, DownloadAndImport(url, locale.Name, path));
			}

			ticks = 0;

			EditorApplication.update += EditorUpdate;
		}

		private static void EditorUpdate()
		{
			if (downloaders != null && downloaders.Count > 0)
			{
				var copy = new Dictionary<string, IEnumerator>(downloaders);
				foreach (var downloader in copy)
				{
					if (downloader.Value != null)
					{
						downloader.Value.MoveNext();
					}
				}
			}
			ticks++;

			if (ticks > 10000 || downloaders.Count == 0)
			{
				downloaders.Clear();
				EditorApplication.update -= EditorUpdate;
			}
		}

		private static IEnumerator DownloadAndImport(string url, string assetFile, string path)
		{
			//WWWForm form = new WWWForm();
			//UnityWebRequest www = UnityWebRequest.Post(url, form);
			UnityWebRequest www = UnityWebRequest.Get(url);
			www.SendWebRequest();

			while (!www.isDone)
			{
				yield return null;
			}

			if (www.error != null)
			{
				Debug.Log("UnityWebRequest.error:" + www.error);
			}
			else if (www.downloadHandler.text == "")
			{
				Debug.Log("Empty text:" + url);
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(assetFile))
				{
					FileWorker.WriteProjectFile(www.downloadHandler.text, assetFile, "csv", path);
					Debug.Log("Imported Asset: " + assetFile);
				}
				else
				{
					Debug.Log("Locale name is not specified");
				}
			}

			downloaders.Remove(url);
			AssetDatabase.SaveAssets();
		}
	}
}