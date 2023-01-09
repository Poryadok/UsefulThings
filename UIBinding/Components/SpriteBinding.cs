using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
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
