﻿using PM.UsefulThings.Extensions;
using System;
using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding
{
	public abstract class BaseListElementData : BaseBindingTarget, IComparable<BaseListElementData>
	{
		public event Action<BaseListElementData> OnClick;
		public event Action<BaseListElementData> OnSelectionChanged;

		public bool Selected { get { return selected.value; } set { SetSelection(value); } }
		
		public bool IsSelectionOffable;

		public virtual int Sort { get; set; }

		private BoolProperty selected = new BoolProperty();

		public virtual void OnDestroy() { }

		public void ClickFromUI()
		{
			OnClick.Call(this);

			SetSelection(IsSelectionOffable ? !selected.value : true);
		}

		// IComparable
		public int CompareTo(BaseListElementData other)
		{
			return Sort.CompareTo(other.Sort);
		}

		protected virtual void OnSetSelection() { }

		private void SetSelection(bool value)
		{
			if (selected.value != value)
			{
				selected.value = value;
				OnSetSelection();

				OnSelectionChanged.Call(this);
			}
		}
	}
}