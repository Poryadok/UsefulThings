using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding.Components
{
	public class RotationBinding : BaseBinding<QuaternionProperty>
	{
		protected override void OnUpdateValue()
		{
			transform.rotation = property.value;
		}
	}
}