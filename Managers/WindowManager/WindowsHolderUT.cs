using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PM.UsefulThings.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PM.UsefulThings
{
    //[CreateAssetMenu(fileName = "WindowsHolder", menuName = "Holders/WindowsHolder", order = 100)]
    public class WindowsHolderUT : ScriptableObject
    {
        [Serializable]
        public class AssetReferenceWithName
        {
            public ComponentReference<Component> Reference;
            public string Name;
        }

        [SerializeField] public AssetReferenceWithName[] Windows;

#if UNITY_EDITOR

        public void FindWindows()
        {
            var windows = new List<AssetReferenceWithName>();

            foreach (var fileName in GetFiles("Assets/CurrentProject/Addressables/UIWindows"))
            {
                if (fileName.EndsWith("meta"))
                {
                    continue;
                }

                windows.Add(new AssetReferenceWithName()
                {
                    Reference = new ComponentReference<Component>(AssetDatabase.AssetPathToGUID(fileName)),
                    Name = Enumerable.First(fileName.Split('\\').Last().Split('.'))
                });
            }

            Windows = windows.ToArray();

            EditorUtility.SetDirty(this);
        }

        private static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
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
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }
#endif
        
    }
}