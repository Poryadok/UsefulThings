using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding.Components
{
	public class VisibilityListBinding : BaseBinding<Property>
	{
		[SerializeField] private bool m_invert = false;

		protected override void OnUpdateValue()
		{
			var propertyList = property as ListProperty;

			if (propertyList == null || property == null)
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(m_invert
					? propertyList.count == 0 
					: propertyList.count > 0);
			}
		}
	}
}