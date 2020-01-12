using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

			records = CSVSerializer.Deserialize<LocalizationString>(localization.text, false);
			
			foreach (var record in records)
			{
				if (record.Key == null || record.Value == null)
				{
					continue;
				}
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
}