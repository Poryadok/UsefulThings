using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings.UIBinding.Components
{
	[RequireComponent(typeof(Button))]
	public class ButtonInteractableEnableBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private bool m_invert = false;

		private Button m_button;

		private void Awake()
		{
			m_button = GetComponent<Button>();
		}

		protected override void OnUpdateValue()
		{
			if (m_button)
			{
				m_button.interactable = m_invert ? !property.value : property.value;
			}
		}
	}
}
