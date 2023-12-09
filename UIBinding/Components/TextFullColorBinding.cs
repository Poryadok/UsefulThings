using UnityEngine;
using UnityEngine.UI;
using PM.UsefulThings.Extensions;
using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(Text))]
	public class TextFullColorBinding : BaseBinding<ColorProperty>
	{
		private Text m_text;

		private void Awake()
		{
			m_text = GetComponent<Text>();
		}

		protected override void OnUpdateValue()
		{
			var alpha = property.value.a;

			m_text.SetColorOnly(property.value);
			m_text.SetAlpha(alpha);
		}
	}
}

