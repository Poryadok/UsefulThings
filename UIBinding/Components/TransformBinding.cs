using System.Collections;
using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding.Components
{
	public class TransformBinding : BaseBinding<TransformProperty>
	{
		protected override void Start()
		{
			base.Start();
			property.value = transform;
		}
	}
}