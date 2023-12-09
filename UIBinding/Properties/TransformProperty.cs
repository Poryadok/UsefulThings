using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class TransformProperty : Property<Transform>
	{
		public TransformProperty() : base() { }
		public TransformProperty(Transform startValue = null) : base(startValue) { }
	}
}