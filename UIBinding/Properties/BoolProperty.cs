﻿using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding
{
	public class BoolProperty : Property<bool>
	{
		public BoolProperty() : base() { }
		public BoolProperty(bool startValue = false) : base(startValue) { }

		protected override bool IsValueDifferent(bool value)
		{
			return GetValue() != value;
		}
	}
}