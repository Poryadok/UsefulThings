using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class FloatProperty : Property<float>
	{
		public FloatProperty() : base() { }
		public FloatProperty(float startValue = 0f) : base(startValue) { }

		protected override bool IsValueDifferent(float value)
		{
			return Mathf.Abs(GetValue() - value) > float.Epsilon;
		}
	}
}