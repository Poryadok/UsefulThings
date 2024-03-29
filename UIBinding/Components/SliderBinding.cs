﻿using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(Slider))]
	public class SliderBinding : BaseBinding<FloatProperty>
	{
		private Slider m_slider;

		private void Awake()
		{
			m_slider = GetComponent<Slider>();
			m_slider.onValueChanged.AddListener(SliderValueChangeHandler);
		}

		protected override void OnUpdateValue()
		{
			m_slider.value = property.value;
		}

		private void SliderValueChangeHandler(float value)
		{
			property.value = value;
		}
	}
}