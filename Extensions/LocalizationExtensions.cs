using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public static class LocalizationExtensions
	{
		public static string Localized(this string key, bool disableWarning = false)
		{
			if (key == null)
			{
				return string.Empty;
			}

			key = key.Replace(" ", "_").ToLower();

			string result = LocalizationManager.Instance.GetLocalization(key);
			if (result != null)
			{
				return result;
			}

			return disableWarning ? key : "[!!!]" + key;
		}
	}
}