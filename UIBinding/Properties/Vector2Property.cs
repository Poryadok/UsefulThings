﻿using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class Vector2Property : Property<Vector2>
	{
		public Vector2Property() : base() { }
		public Vector2Property(Vector2 startValue = default(Vector2)) : base(startValue) { }

		protected override bool IsValueDifferent(Vector2 value)
		{
			return GetValue() != value;
		}
	}
}