using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using UnityEngine;
using TMPro;

namespace UIBinding.Components
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizationBinding : MonoBehaviour
	{
		public string key;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField]
		private bool m_forceUpperCase = true;

		private TextMeshProUGUI m_label;

		private void LocalizeText()
		{
			if (m_label != null)
			{
				m_label.text = string.Format(m_format, m_forceUpperCase ? key.Localized().ToUpper() : key.Localized());
			}
		}

		private void Awake()
		{
			m_label = GetComponent<TextMeshProUGUI>();
			LocalizationManager.OnLocaleChanged += OnLocalizationChanged;
		}

		private void Start()
		{
			LocalizeText();
		}

		private void OnDestroy()
		{
			LocalizationManager.OnLocaleChanged -= OnLocalizationChanged;
		}

		private void OnLocalizationChanged()
		{
			LocalizeText();
		}
	}
}