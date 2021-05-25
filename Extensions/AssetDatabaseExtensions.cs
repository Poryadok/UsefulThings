using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PM.UsefulThings.Extensions
{
    public static class AssetDatabaseExtensions
    {
        public static List<T> FindAssetsByType<T>(string path) where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).ToString().Replace("UnityEngine.", "")));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!assetPath.StartsWith(path))
                {
                    continue;
                }
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
#endif
            return assets;
        }
    }
}