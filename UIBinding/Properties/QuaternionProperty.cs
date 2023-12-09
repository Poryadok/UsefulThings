using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class QuaternionProperty : Property<Quaternion>
	{
		public QuaternionProperty() : base() { }
		public QuaternionProperty(Quaternion startValue = default(Quaternion)) : base(startValue) { }

		protected override bool IsValueDifferent(Quaternion value)
		{
			return GetValue() != value;
		}
	}
}