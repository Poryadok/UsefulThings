using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class SpriteProperty : Property<Sprite>
	{
		public SpriteProperty() : base() { }
		public SpriteProperty(Sprite startValue = null) : base(startValue) { }

		protected override bool IsValueDifferent(Sprite data)
		{
			return GetValue() != data;
		}
	}
}
