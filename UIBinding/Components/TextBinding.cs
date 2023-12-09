using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PM.UsefulThings.UIBinding.Components
{
	public class TextBinding : BaseBinding<Property>
	{
		private TextMeshProUGUI m_text;
		private Text m_inputField;

		[SerializeField]
		private string m_format = "{0}";

		[SerializeField]
		private bool m_forceUpperCase = false;

		private void Awake()
		{
			m_text = GetComponent<TextMeshProUGUI>();

			if (m_text == null)
			{
				m_inputField = GetComponent<Text>();
			}
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
				m_text.text = string.Format(m_format, val);
			}
			else if (m_inputField != null)
			{
				m_inputField.text = string.Format(m_format, val);
			}
		}
	}
}