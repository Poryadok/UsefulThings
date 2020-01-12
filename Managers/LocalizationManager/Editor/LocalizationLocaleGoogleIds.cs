using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	[CreateAssetMenu(fileName = "LocalizationLocaleGoogleIds", menuName = "Scriptoton/LocalizationLocaleGoogleIds", order = 10)]
	public class LocalizationLocaleGoogleIds : ScriptableObject
	{
		[Serializable]
		public class LocaleIdPair
		{
			public string Name;
			public string Id;
		}

		public string Link;
		[SerializeField]
		public LocaleIdPair[] Locales;
	}
}