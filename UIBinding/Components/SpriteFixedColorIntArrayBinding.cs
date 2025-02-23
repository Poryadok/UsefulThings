using UnityEngine;
using UnityEngine.UI;
using System;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpriteFixedColorIntArrayBinding : BaseBinding<IntProperty>
	{
		[Serializable]
		public class ColorElement
		{
			public int value { get { return m_value; } }
			public Color color { get { return m_color; } }

			[SerializeField]
			private int m_value = 0;
			[SerializeField]
			private Color m_color = Color.white;
		}

		[SerializeField]
		private ColorElement[] m_elements = null;

		private SpriteRenderer m_SpriteRen;

		private void Awake()
		{
			m_SpriteRen = GetComponent<SpriteRenderer>();
		}

		protected override void OnUpdateValue()
		{
			var value = property.value;
			for (int i = 0; i < m_elements.Length; i++)
			{
				var element = m_elements[i];
				if (element.value == value)
				{
					m_SpriteRen.color = element.color;
					break;
				}
			}
		}
	}
}

