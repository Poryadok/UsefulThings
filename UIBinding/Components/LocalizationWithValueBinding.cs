using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using UnityEngine;
using TMPro;
using UIBinding.Base;
using UnityEngine.UI;

namespace UIBinding.Components
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LocalizationWithValueBinding : BaseBinding<Property>
	{
		public string key;

		private TextMeshProUGUI m_text;
		private InputField m_inputField;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField]
		private bool m_forceUpperCase = false;

		private string localization = string.Empty;

		private void Awake()
		{
			m_text = GetComponent<TextMeshProUGUI>();

			if (m_text == null)
			{
				m_inputField = GetComponent<InputField>();
			}
			LocalizationManager.OnLocaleChanged += OnLocalizationChanged;
		}

		private void LocalizeText()
		{
			localization = string.Format(m_format, m_forceUpperCase ? key.Localized().ToUpper() : key.Localized());
		}

		protected override void Start()
		{
			base.Start();

			LocalizeText();
			OnUpdateValue();
		}

		private void OnDestroy()
		{
			LocalizationManager.OnLocaleChanged -= OnLocalizationChanged;
		}

		private void OnLocalizationChanged()
		{
			LocalizeText();
            OnUpdateValue();
        }

		protected override void OnUpdateValue()
		{
			var val = property.ValueToString();
			if (!string.IsNullOrEmpty(val) && m_forceUpperCase)
			{
				val = val.ToUpper();
			}

			if (m_text != null)
			{
				m_text.text = string.Format(localization, val);
			}
			else if (m_inputField != null)
			{
				m_inputField.text = string.Format(localization, val);
			}
		}
	}
}