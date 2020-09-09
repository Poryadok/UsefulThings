using UnityEngine;
using UnityEngine.UI;
using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteFixedColorBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private Color m_colorOnTrue = Color.white;
		[SerializeField]
		private Color m_colorOnFalse = Color.white;

		private SpriteRenderer spriteRenderer;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		protected override void OnUpdateValue()
		{
			if (property.value)
			{
				spriteRenderer.color = m_colorOnTrue;
			}
			else
			{
				spriteRenderer.color = m_colorOnFalse;
			}
		}
	}
}

