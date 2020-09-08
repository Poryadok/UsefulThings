﻿using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class WindowManagerUT : PrefabMonoSingleton<WindowManagerUT>
	{
		[SerializeField]
		protected WindowsHolderUT WindowsHolder;
		[SerializeField]
		protected Transform MainRootPrefab;
		[SerializeField]
		protected Transform AuxiliaryRootPrefab;

		public Canvas RaycastCanvas { get; private set; }

		public Transform MainRoot;
		public Transform AuxiliaryRoot;

		protected Stack<IWindowUT> panels = new Stack<IWindowUT>();
		protected Stack<IWindowUT> frames = new Stack<IWindowUT>();
		protected List<IWindowUT> allWindows = new List<IWindowUT>();

		protected override void Awake()
		{
			base.Awake();

			if (MainRoot == null)
			{
				MainRoot = Instantiate(MainRootPrefab);
			}
			if (AuxiliaryRoot == null)
			{
				AuxiliaryRoot = Instantiate(AuxiliaryRootPrefab);
			}

			DontDestroyOnLoad(MainRoot);
			DontDestroyOnLoad(AuxiliaryRoot);

			RaycastCanvas = MainRoot.GetComponent<Canvas>();

#if UNITY_EDITOR
			var childCount = MainRoot.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (MainRoot.transform.GetChild(i).gameObject.GetComponent<MonoBehaviour>() is IWindowUT)
				{
					Destroy(MainRoot.transform.GetChild(i).gameObject);
				}
			}

			childCount = AuxiliaryRoot.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (AuxiliaryRoot.transform.GetChild(i).gameObject.GetComponent<MonoBehaviour>() is IWindowUT)
				{
					Destroy(AuxiliaryRoot.transform.GetChild(i).gameObject);
				}
			}
#endif

		}

		public IWindowUT ActivePanel
		{
			get
			{
				if (panels.Count > 0)
				{
					return panels.Peek();
				}
				else
				{
					return null;
				}
			}
		}

		protected IWindowUT _panelWithFocus;
		public IWindowUT PanelWithFocus
		{
			get
			{
				if (panels.Count > 0 && _panelWithFocus != null && _panelWithFocus.IsFocused)
				{
					return _panelWithFocus;
				}
				else
				{
					return null;
				}
			}
			protected set
			{
				_panelWithFocus = value;
			}
		}

		public T AddNewFrame<T>(WindowCloseModes mode = WindowCloseModes.CloseNonSolid) where T : MonoBehaviour
		{
			foreach (var window in WindowsHolder.Windows)
			{
				if (window.GetType() == typeof(T))
				{
					return AddNewFrame(window as IWindowUT, mode) as T;
				}
			}
			Debug.LogError("There is no window prefab with class " + typeof(T).ToString());
			return null;
		}

		public IWindowUT AddNewFrame(IWindowUT prefab, WindowCloseModes mode = WindowCloseModes.CloseNonSolid)
		{
			var newFrame = CreateWindow(AuxiliaryRoot, prefab, mode);

			frames.Push(newFrame);
			allWindows.Add(newFrame);
			Listen(newFrame);

			newFrame.Init(false, false);

			return newFrame;
		}

		public T OpenNewPanel<T>(WindowCloseModes mode = WindowCloseModes.CloseNonSolid) where T : MonoBehaviour
		{
			foreach (var window in WindowsHolder.Windows)
			{
				if (window.GetType() == typeof(T))
				{
					return OpenNewPanel(window as IWindowUT, mode) as T;
				}
			}
			Debug.LogError("There is no window prefab with class " + typeof(T).ToString());
			return null;
		}

		public IWindowUT OpenNewPanel(IWindowUT prefab, WindowCloseModes mode = WindowCloseModes.CloseNonSolid)
		{
			var newPanel = CreateWindow(MainRoot, prefab, mode);

			panels.Push(newPanel);
			allWindows.Add(newPanel);
			Listen(newPanel);

			newPanel.Init();

			return newPanel;
		}

		private IWindowUT CreateWindow(Transform parent, IWindowUT prefab, WindowCloseModes mode = WindowCloseModes.CloseNonSolid, bool isPanel = true)
		{
			if (isPanel)
			{
				ClosePanels(mode);
			}
			else
			{
				CloseFrames(mode);
			}

			if (ActivePanel != null)
			{
				ActivePanel.IsInteractable = false;
				ActivePanel.IsFocused = false;
			}

			return GameObject.Instantiate(prefab as MonoBehaviour, parent) as IWindowUT;
		}

		public IWindowUT AddChildToActivePanel(IWindowUT newChild)
		{
			if (ActivePanel != null)
			{
				if (ActivePanel.CanHaveChildren)
				{
					return ActivePanel.AddChild(newChild).Init();
				}
			}

			return null;
		}

		protected void Listen(IWindowUT window)
		{
			window.OnChildAdded += Window_OnChildAdded;
			window.OnClosed += Window_OnClosed;
			window.OnSetFocus += Window_OnSetFocus;
		}

		protected void Unlisten(IWindowUT window)
		{
			window.OnChildAdded -= Window_OnChildAdded;
			window.OnClosed -= Window_OnClosed;
			window.OnSetFocus -= Window_OnSetFocus;
		}

		private void Window_OnSetFocus(IWindowUT sender, bool value)
		{
			if (value)
			{
				if (PanelWithFocus != sender)
				{
					if (PanelWithFocus != null)
					{
						PanelWithFocus.IsFocused = false;
					}

					PanelWithFocus = sender;
				}
			}
			else
			{
				if (PanelWithFocus == sender)
				{
					PanelWithFocus = null;
				}
			}
		}

		private void Window_OnClosed(IWindowUT sender)
		{
			bool shouldSetInteractable = false;
			if (sender == ActivePanel)
			{
				shouldSetInteractable = true;
			}

			if (panels.Contains(sender))
			{
				//remove sender from stack even if it's in the middle
				var temp = new Stack<IWindowUT>();
				while (panels.Contains(sender))
				{
					var panel = panels.Pop();
					if (panel != sender)
					{
						temp.Push(panel);
					}
				}
				while (temp.Count > 0)
				{
					panels.Push(temp.Pop());
				}
			}
			if (frames.Contains(sender))
			{
				//remove sender from stack even if it's in the middle
				var temp = new Stack<IWindowUT>();
				while (frames.Contains(sender))
				{
					var panel = frames.Pop();
					if (panel != sender)
					{
						temp.Push(panel);
					}
				}
				while (temp.Count > 0)
				{
					frames.Push(temp.Pop());
				}
			}

			allWindows.Remove(sender);
			//close all children. it's recursive
			for (int i = sender.Children.Count - 1; i >= 0; i--)
			{
				sender.Children[i].Close();
			}

			if (shouldSetInteractable)
			{
				if (ActivePanel != null)
				{
					ActivePanel.IsInteractable = true;
				}
			}

			if (sender == PanelWithFocus || PanelWithFocus == null)
			{
				if (ActivePanel != null)
				{
					ActivePanel.IsFocused = true;
				}
			}

			Unlisten(sender);
		}

		private void Window_OnChildAdded(IWindowUT sender, IWindowUT child)
		{
			allWindows.Add(child);
			Listen(child);
		}

		public void ClosePanel<T>() where T:IWindowUT
		{
			var list = new List<IWindowUT>(panels);

			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] is T)
				{
					list[i].Close();
					return;
				}
			}
		}

		public void CloseFrames(WindowCloseModes closeMode = WindowCloseModes.CloseEverything)
		{
			CloseWindows(frames, closeMode);
		}

		public void ClosePanels(WindowCloseModes closeMode = WindowCloseModes.CloseEverything)
		{
			CloseWindows(panels, closeMode);
		}

		public void CloseWindows(Stack<IWindowUT> stack, WindowCloseModes closeMode)
		{
			switch (closeMode)
			{
				case WindowCloseModes.CloseNothing:
					break;
				case WindowCloseModes.CloseNonSolid:
					while (ActivePanel != null && !ActivePanel.IsSolid)
					{
						stack.Pop().Close();
					}

					break;
				case WindowCloseModes.CloseEverything:
					while (stack.Count > 0)
					{
						stack.Pop().Close();
					}

					break;
				default:
					throw new System.Exception("Non registered mode!");
			}
		}

		public T GetWindow<T>() where T : class, IWindowUT
		{
			return allWindows.Find(x => x is T) as T;
		}
	}
}