using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PM.UsefulThings
{
	//[CreateAssetMenu(fileName = "WindowsHolderUT", menuName = "Holders/WindowsHolderUT", order = 100)]
	public class WindowsHolderUT : ScriptableObject
	{
		[SerializeField]
		public MonoBehaviour[] Windows;

		public void FindWindows()
		{
			var windows = Resources.LoadAll<MonoBehaviour>("UIWindows").Where(x => x.GetComponent<IWindowUT>() != null).ToArray();
			Windows = new MonoBehaviour[windows.Length];
			for (int i = 0; i < windows.Length; i++)
			{
				Windows[i] = windows[i].GetComponent<IWindowUT>() as MonoBehaviour;
			}
		}
	}
}