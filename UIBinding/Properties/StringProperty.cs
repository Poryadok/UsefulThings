using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding
{
	public class StringProperty : Property<string>
	{
		public StringProperty() : base() { }
		public StringProperty(string startValue = "") : base(startValue) { }

		protected override bool IsValueDifferent(string value)
		{
			return GetValue() != value;
		}
	}
}