using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteColorBinding : BaseBinding<ColorProperty>
	{
		private SpriteRenderer m_spriteRen;

		private void Awake()
		{
			m_spriteRen = GetComponent<SpriteRenderer>();
		}

		protected override void OnUpdateValue()
		{
			m_spriteRen.color = property.value;
		}
	}
}