using System;
using PM.UsefulThings.UIBinding.Base;

namespace PM.UsefulThings.UIBinding.Elements
{
	public abstract class BaseListElement : BaseBindingBehaviourTarget, IComparable<BaseListElement>
	{
		public bool IsSelectionOffable;
		
		public BaseListElementData data { get { return m_data; } }

		private BaseListElementData m_data;

		public void Init(BaseListElementData data)
		{
			OnDataChange(data);

			m_data = data;

			OnInit();
			ForceUpdateProperties();
			data.IsSelectionOffable = IsSelectionOffable;
		}

		protected virtual void OnInit() { }
		protected virtual void OnDataChange(BaseListElementData newData) { }

		public int CompareTo(BaseListElement other)
		{
			return m_data.Sort.CompareTo(other.m_data.Sort);
		}

		public virtual BaseListElement Resolve(BaseListElementData data)
		{
			return this;
		}

		public virtual void ButtonClickHandler()
		{
			data.ClickFromUI();
		}

		protected virtual void OnDestroy()
		{
			data.OnDestroy();
		}
	}
}