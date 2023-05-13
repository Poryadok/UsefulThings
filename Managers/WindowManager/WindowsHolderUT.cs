using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PM.UsefulThings
{
	//[CreateAssetMenu(fileName = "WindowsHolderUT", menuName = "Holders/WindowsHolderUT", order = 100)]
	public class WindowsHolderUT : ScriptableObject
	{
		[SerializeField]
		public AssetReference[] Windows;

		public void FindWindows()
		{
			var windows = Resources.LoadAll<MonoBehaviour>("UIWindows").Where(x => x is IWindowUT).ToArray();

			Windows = new AssetReference[windows.Length];

			for (var i = 0; i < windows.Length; i++)
			{
				var window = windows[i];
				Windows[i] = new AssetReference(window.name);
			}
		}
	}
}