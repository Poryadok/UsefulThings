#if UNITY_EDITOR
using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PM.UsefulThings.Extensions
{
    public static class AssetDatabaseExtensions
    {
        public static List<T> FindAssetsByType<T>(string path) where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
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
            return assets;
        }
    }
}
#endif