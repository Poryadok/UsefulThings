using UnityEngine;
using UnityEngine.UI;

using UIBinding.Base;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Image))]
	public class ImageFixedSpriteBoolBinding : BaseBinding<BoolProperty>
	{
		[SerializeField]
		private Sprite imageOnTrue = null;
		[SerializeField]
		private Sprite imageOnFalse = null;

		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
		}

		protected override void OnUpdateValue()
		{
			if (property.value)
			{
				image.sprite = imageOnTrue;
			}
			else
			{
				image.sprite = imageOnFalse;
			}
		}
	}
}

