using PM.UsefulThings.UIBinding.Base;
using UnityEngine;

namespace PM.UsefulThings.UIBinding
{
	public class AudioClipProperty : Property<AudioClip>
	{
		public AudioClipProperty() : base() { }
		public AudioClipProperty(AudioClip startValue) : base(startValue) { }

		protected override bool IsValueDifferent(AudioClip value)
		{
			return GetValue() != value;
		}
	}
}