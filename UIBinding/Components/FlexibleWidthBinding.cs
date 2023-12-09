using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(LayoutElement))]
	public class FlexibleWidthBinding : BaseBinding<IntProperty>
	{
		private LayoutElement m_component;

		private void Awake()
		{
			m_component = GetComponent<LayoutElement>();
		}

		protected override void OnUpdateValue()
		{
			m_component.flexibleWidth = property.value;	
		}
	}
}