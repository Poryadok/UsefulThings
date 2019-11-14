using UnityEngine;
using UIBinding.Base;
using TMPro;

namespace UIBinding.Components
{
	[RequireComponent(typeof(TMP_Dropdown))]
	public class DropdownBinding : BaseBinding<IntProperty>
	{
		[SerializeField]
		private bool m_changePropertyValue = true;

		private TMP_Dropdown m_component;

		private void Awake()
		{
			m_component = GetComponent<TMP_Dropdown>();
			m_component.onValueChanged.AddListener(ValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			if (m_component.value != property.value)
			{
				m_component.value = property.value;
			}
		}

		private void ValueChangeHandler(int value)
		{
			if (m_changePropertyValue)
			{
				property.value = value;
			}
		}
	}
}