using CsvHelper;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PM.UsefulThings
{
	public class LocalizationManager : Singleton<LocalizationManager>
	{
		private const string CurrentLanguageKey = "current_language";
		private const SystemLanguage DefaultLanguage = SystemLanguage.English;

		public static event System.Action OnLocaleChanged;

		public SystemLanguage CurrentLanguage { get; private set; }

		public List<SystemLanguage> AvailableLanguages = new List<SystemLanguage>()
		{
			SystemLanguage.English,
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
				CurrentLanguage = Application.systemLanguage;
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

			using (var reader = new StringReader(localization.text))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.HasHeaderRecord = false;
				records = csv.GetRecords<LocalizationString>().ToList();
			}

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
}