using UnityEngine;
using UIBinding.Base;
using System.Collections.Generic;
using TMPro;

namespace UIBinding.Components
{
	[RequireComponent(typeof(TMP_Dropdown))]
	public class DropdownListBinding : BaseBinding<EnumerableProperty>
	{
		private TMP_Dropdown m_component;

		private void Awake()
		{
			m_component = GetComponent<TMP_Dropdown>();
		}

		protected override void OnUpdateValue()
		{
			var options = new List<TMP_Dropdown.OptionData>();

			foreach (var item in property.value)
			{
				options.Add(new TMP_Dropdown.OptionData(item.ToString()));
			}

			m_component.options = options;
		}
	}
}