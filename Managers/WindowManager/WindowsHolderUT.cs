#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace PM.UsefulThings
{
	//[CreateAssetMenu(fileName = "WindowsHolder", menuName = "Holders/WindowsHolder", order = 100)]
	public class WindowsHolderUT : ScriptableObject
	{
		[SerializeField] private Component[] windows;

		public IEnumerable<IWindowUT> Windows => windows.Cast<IWindowUT>();

#if UNITY_EDITOR

		public void FindWindows()
		{
			var newWindows = new List<IWindowUT>();

			foreach (var fileName in GetFiles("Assets/CurrentProject/Addressables/UIWindows"))
			{
				if (fileName.EndsWith("meta"))
				{
					continue;
				}

				var guid = AssetDatabase.AssetPathToGUID(fileName);
				var file = AssetDatabase.LoadAssetAtPath<GameObject>(fileName);
				if (!file.TryGetComponent(out IWindowUT component))
					continue;

				newWindows.Add(component);
			}

			windows = newWindows.Cast<Component>().ToArray();

			EditorUtility.SetDirty(this);
		}

		private static IEnumerable<string> GetFiles(string path)
		{
			var queue = new Queue<string>();
			queue.Enqueue(path);
			while (queue.Count > 0)
			{
				path = queue.Dequeue();
				try
				{
					foreach (var subDir in Directory.GetDirectories(path))
					{
						queue.Enqueue(subDir);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				string[] files = null;
				try
				{
					files = Directory.GetFiles(path);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				if (files != null)
				{
					for (var i = 0; i < files.Length; i++)
					{
						yield return files[i];
					}
				}
			}
		}
#endif
	}
}