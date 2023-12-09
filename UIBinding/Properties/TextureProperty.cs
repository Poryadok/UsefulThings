using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class TextureProperty : Property<Texture>
	{
		public TextureProperty() : base() { }
		public TextureProperty(Texture startValue = null) : base(startValue) { }

		protected override bool IsValueDifferent(Texture data)
		{
			return GetValue() != data;
		}
	}
}
