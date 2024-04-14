using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioClipBinding : BaseBinding<AudioClipProperty>
	{
		private AudioSource m_target;

		private void Awake()
		{
			m_target = GetComponent<AudioSource>();
		}

		protected override void OnUpdateValue()
		{
			m_target.clip = property.value;
		}
	}
}