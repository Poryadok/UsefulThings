using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UIBinding;
using UnityEngine;
using UnityEngine.UI;

namespace PM.UsefulThings
{
	public class PetalButton : Button
	{
		public Button OptionPrefab;

		private PetalOption[] options;
		public Button[] buttons;

		private FloatProperty arcPerOption = new FloatProperty();

		public void FillOptions(PetalOption[] options)
		{
			this.options = options;
			UpdateOptions();
		}

		private void UpdateOptions()
		{
			//arcPerOption.value = 1f / options.Length;
			//if (buttons != null && buttons.Length > 0)
			//{
			//	foreach (var item in buttons)
			//	{
			//		Destroy(item.gameObject);
			//	}
			//}
			//buttons = new Button[options.Length];
			//for (int i = 0; i < options.Length; i++)
			//{
			//	var button = Instantiate<Button>(OptionPrefab, this.transform);
			//	button.transform.Rotate(Vector3.forward, arcPerOption.value * i * 360);
			//	button.onClick.AddListener(delegate { OnButtonClick(i); });
			//	buttons[i] = button;
			//}
			for (int i = 0; i < buttons.Length; i++)
			{
				if (options.Length > i)
				{
					var pass = i;
					buttons[i].gameObject.SetActive(true);
					buttons[i].onClick.AddListener(delegate { OnButtonClick(pass); });
				}
				else
				{
					buttons[i].gameObject.SetActive(false);
				}
			}
		}

		public void OnButtonClick(int index)
		{
			options[index].Action.SafeCall();
			Destroy(this.gameObject);
		}

		public void Close()
		{
			Destroy(this.gameObject);
		}
	}

	public class PetalOption
	{
		public Sprite Sprite;
		public System.Action Action;

		public PetalOption(Sprite sprite, System.Action action)
		{
			this.Sprite = sprite;
			this.Action = action;
		}
	}
}