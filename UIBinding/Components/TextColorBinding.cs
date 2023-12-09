using UnityEngine;
using UnityEngine.UI;
using PM.UsefulThings.Extensions;
using PM.UsefulThings.UIBinding.Base;
using TMPro;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(TMP_Text))]
	public class TextColorBinding : BaseBinding<ColorProperty>
	{
		private TMP_Text m_text;

		private void Awake()
		{
			m_text = GetComponent<TMP_Text>();
		}

		protected override void OnUpdateValue()
		{
			m_text.SetColorOnly(property.value);
		}
	}
}
