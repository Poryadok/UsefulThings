using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Networking;
#endif

namespace PM.UsefulThings
{
	public class LocalizationManager : Singleton<LocalizationManager>
	{
		private const string CurrentLanguageKey = "current_language";
		private const SystemLanguage DefaultLanguage = SystemLanguage.Russian;

		public static event System.Action OnLocaleChanged;

		public SystemLanguage CurrentLanguage { get; private set; }

		public List<SystemLanguage> AvailableLanguages = new List<SystemLanguage>()
		{
			//SystemLanguage.English,
			SystemLanguage.Russian
		};

		private Dictionary<string, string> Locale = new Dictionary<string, string>();

		public LocalizationManager()
		{
			if (PlayerPrefs.HasKey(CurrentLanguageKey))
			{
				CurrentLanguage = (SystemLanguage)PlayerPrefs.GetInt(CurrentLanguageKey);
			}
			else
			{
				CurrentLanguage = SystemLanguage.Russian; //Application.systemLanguage;
			}
			InitCurrentLanguage();
		}

		private void InitCurrentLanguage()
		{
			Locale.Clear();

			var localization = (TextAsset)Resources.Load("Localizations/" + CurrentLanguage.ToString(), typeof(TextAsset));
			if (localization == null)
			{
				localization = (TextAsset)Resources.Load("Localizations/" + DefaultLanguage.ToString(), typeof(TextAsset));
			}

			if (localization == null)
			{
				Debug.LogError("Can't load any localization");
				return;
			}

			IEnumerable<LocalizationString> records = null;

			records = CSVSerializer.Deserialize<LocalizationString>(localization.text);

			foreach (var record in records)
			{
				if (Locale.ContainsKey(record.Key))
				{
					Debug.LogError("There is a double key: " + record.Key);
					continue;
				}
				Locale[record.Key] = record.Value;
			}
		}

		public void ChangeLanguage(SystemLanguage language)
		{
			if (CurrentLanguage == language)
			{
				return;
			}

			CurrentLanguage = language;
			InitCurrentLanguage();

			PlayerPrefs.SetInt(CurrentLanguageKey, (int)language);
			OnLocaleChanged?.Invoke();
		}

		public struct LocalizationString
		{
			public string Key { get; set; }
			public string Value { get; set; }
		}

		public string GetLocalization(string key)
		{
			if (Locale.ContainsKey(key))
			{
				return Locale[key];
			}
			else
			{
				return null;
			}
		}
	}

#if UNITY_EDITOR
	public class LocalizationEditor : Editor
	{
		private static Dictionary<string, IEnumerator> downloaders = new Dictionary<string, IEnumerator>();
		private static int ticks;

		[MenuItem("Tools/Localization/Download")]
		private static void Init()
		{
			var localesIds = Resources.Load<LocalizationLocaleGoogleIds>("LocalizationLocaleGoogleIds");
			var locales = localesIds.Locales;
			foreach (var locale in locales)
			{
				string url = $"https://docs.google.com/spreadsheets/d/e/2PACX-1vQpKSHcuTuvaf1e_h6rEpt5zaz2Db9g7HlZmIHV5OVeW7bb7UjYiafAbL8ZiOOsswDGVfSxnjVJi4Cm/pub?gid={locale.Id}&single=true&output=csv";
				string path = $"Assets/CurrentProject/Resources/Localizations";

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
#endif
}