﻿using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class MaterialProperty : Property<Material>
	{
		public MaterialProperty() : base() { }
		public MaterialProperty(Material startValue = null) : base(startValue) { }

		protected override bool IsValueDifferent(Material data)
		{
			return GetValue() != data;
		}
	}
}
