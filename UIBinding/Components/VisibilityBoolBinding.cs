using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding.Components
{
	public class VisibilityBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_invert = false;

		protected override void OnUpdateValue()
		{
			gameObject.SetActive(m_invert ? !property.value : property.value);
		}
	}
}