﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public abstract class AbFrame : MonoBehaviour, IWindowUT
	{
		public virtual bool IsSolid
		{
			get
			{
				return false;
			}
		}

		public virtual bool CanHaveChildren
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsInteractable { get; set; }
		public virtual bool IsFocused { get; set; }

		public List<IWindowUT> Children { get; } = new List<IWindowUT>();

		public event Action<IWindowUT> OnClosed;
		public event Action<IWindowUT, bool> OnSetInteractable;
		public event Action<IWindowUT, bool> OnSetFocus;
		public event Action<IWindowUT, IWindowUT> OnChildAdded;

		public virtual IWindowUT AddChild(IWindowUT newChild)
		{
			Children.Add(newChild);
			OnChildAdded?.Invoke(this, newChild);
			return this;
		}

		public virtual void Close()
		{
			OnClosed?.Invoke(this);
			Destroy(this.gameObject);
		}

		public IWindowUT Init(bool isInteractable = true, bool isFocused = true)
		{
			this.IsInteractable = isInteractable;
			this.IsFocused = isFocused;
            if (IsInteractable)
            {
                OnSetInteractable.Invoke(this, true);
            }
            if (IsFocused)
            {
                OnSetFocus.Invoke(this, true);
            }
			return this;
		}
	}
}