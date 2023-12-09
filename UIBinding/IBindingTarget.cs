using System;

namespace PM.UsefulThings.UIBinding.Base
{
	public interface IBindingTarget
	{
		event Action OnForceUpdateProperties;

		BaseProperty[] GetProperties();
	}
}