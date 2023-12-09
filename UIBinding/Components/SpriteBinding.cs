using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteBinding : BaseBinding<SpriteProperty>
	{
		private SpriteRenderer m_component;

		private void Awake()
		{
			m_component = GetComponent<SpriteRenderer>();
		}

		protected override void OnUpdateValue()
		{
			m_component.sprite = property.value;
		}
	}
}
