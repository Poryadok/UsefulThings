using UnityEngine;
using UnityEngine.UI;
using System;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Image))]
	public class ImageFixedSpriteIntBinding : BaseBinding<IntProperty>
	{
		[Serializable]
		public class SpriteElement
		{
			public int value { get { return m_value; } }
			public Sprite sprite { get { return _sprite; } }

			[SerializeField]
			private int m_value = 0;
			[SerializeField]
			private Sprite _sprite = null;
		}

		[SerializeField]
		private SpriteElement[] m_elements = null;

		private Image m_graphic;

		private void Awake()
		{
			m_graphic = GetComponent<Image>();
		}

		protected override void OnUpdateValue()
		{
			var value = property.value;
			for (int i = 0; i < m_elements.Length; i++)
			{
				var element = m_elements[i];
				if (element.value == value)
				{
					m_graphic.sprite = element.sprite;
					break;
				}
			}
		}
	}
}

