using System;
using PM.UsefulThings.UIBinding.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace PM.UsefulThings.UIBinding.Elements
{
	public class BaseListElementResolver : BaseListElement
	{
		[SerializeField]
		private BaseListElement[] m_possibleListElements;
		
		public override BaseListElement Resolve(BaseListElementData data)
		{
			foreach (var possibleListElement in m_possibleListElements)
			{
				if (possibleListElement.Resolve(data) == possibleListElement)
				{
					return possibleListElement;
				}
			}

			Debug.LogError($"Can't resolve {data.GetType()}");
			return null;
		}
	}
}