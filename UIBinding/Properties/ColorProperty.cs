using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class ColorProperty : Property<Color>
	{
		public ColorProperty() : base() { }
		public ColorProperty(Color startValue = default(Color)) : base(startValue) { }

		protected override bool IsValueDifferent(Color data)
		{
			return GetValue() != data;
		}
	}
}