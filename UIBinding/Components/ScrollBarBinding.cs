using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(Scrollbar))]
	public class ScrollBarBinding : BaseBinding<FloatProperty>
	{
		private Scrollbar m_scrollBar;

		private void Awake()
		{
			m_scrollBar = GetComponent<Scrollbar>();
			m_scrollBar.onValueChanged.AddListener(ScrollValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			m_scrollBar.value = property.value;
		}

		private void ScrollValueChangeHandler(float value)
		{
			property.value = value;
		}
	}
}