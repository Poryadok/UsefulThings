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
		public event System.Action OnClose;

		public Button OptionPrefab;

		private PetalOption[] options;
		public Button[] buttons;

		private FloatProperty arcPerOption = new FloatProperty();

		protected override void Start()
		{
			base.Start();
			this.GetComponent<Canvas>().worldCamera = Camera.main;
		}

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
					buttons[i].GetComponent<Image>().sprite = options[i].Sprite;
					buttons[i].gameObject.SetActive(true);
                    buttons[i].interactable = options[i].IsInteractable;

                    var pass = i;
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
			Close();
		}

		public void Close()
		{
			Destroy(this.gameObject);
			OnClose.SafeCall();
		}
	}

	public class PetalOption
	{
		public Sprite Sprite;
		public System.Action Action;
		public bool IsInteractable;

		public PetalOption(Sprite sprite, System.Action action, bool isInteractable = true)
		{
			this.Sprite = sprite;
			this.Action = action;
            this.IsInteractable = isInteractable;
		}
	}
}