using System.Collections;
using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding
{
	public class EnumerableProperty : Property<IEnumerable>
	{
		public EnumerableProperty() : base() { }
		public EnumerableProperty(IEnumerable startValue = null) : base(startValue) { }

		protected override bool IsValueDifferent(IEnumerable value)
		{
			return GetValue() != value;
		}
	}
}