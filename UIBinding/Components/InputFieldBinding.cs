using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;
using TMPro;

namespace UIBinding.Components
{
	[RequireComponent(typeof(TMP_InputField))]
	public class InputFieldBinding : BaseBinding<StringProperty>
	{
		[SerializeField]
		private bool m_changePropertyValue = true;

		private TMP_InputField m_component;

		private void Awake()
		{
			m_component = GetComponent<TMP_InputField>();
			m_component.onValueChanged.AddListener(ValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			if (m_component.text != property.value)
			{
				m_component.text = property.value;
			}
		}

		private void ValueChangeHandler(string value)
		{
			if (m_changePropertyValue)
			{
				property.value = value;
			}
		}
	}
}