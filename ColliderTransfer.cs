using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class ColliderTransfer : MonoBehaviour
	{
		public event System.Action OnMouseClick;
		public event System.Action OnMousePressed;
		public event System.Action OnMouseReleased;

		private bool isMouseOver;

		private void OnMouseOver()
		{
			isMouseOver = true;
		}

		private void OnMouseExit()
		{
			isMouseOver = false;
		}

		private void OnMouseUpAsButton()
		{
			OnMouseClick?.Invoke();
		}
		
		private void OnMouseDown()
		{
			OnMousePressed?.Invoke();
		}
		
		private void OnMouseUp()
		{
			if (isMouseOver)
				OnMouseReleased?.Invoke();
		}
	}
}