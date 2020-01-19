using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PM.UsefulThings
{
	public static class FileWorker
	{
		public static bool WriteFile(string data, string fileName, string path = null)
		{
			bool retValue = false;
			try
			{
				var dir = Application.persistentDataPath;
				if (path != null)
				{
					dir += $"/{path}";
				}
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				System.IO.File.WriteAllText($"{dir}/{fileName}.txt", data);
				retValue = true;
				Debug.Log($"Saved data to file {dir}/{fileName}.txt");
			}
			catch (System.Exception ex)
			{
				string ErrorMessages = "File Write Error\n" + ex.Message;
				retValue = false;
				Debug.LogError(ErrorMessages);
			}
			return retValue;
		}

#if UNITY_EDITOR
		public static bool WriteProjectFile(string data, string fileName, string extension = "txt", string path = null)
		{
			bool retValue = false;
			try
			{
				var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"{path}/{fileName}.{extension}");
				if (textAsset == null)
				{
					var dir = Application.dataPath + "/CurrentProject/Resources";
					if (path != null)
					{
						dir += $"/{path}";
					}
					if (!Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}

					System.IO.File.WriteAllText($"{dir}/{fileName}.txt", data);
				}
				else
				{
					File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), data);
				}
				if (textAsset != null)
				{
					EditorUtility.SetDirty(textAsset);
				}
				AssetDatabase.SaveAssets();

				retValue = true;
				Debug.Log($"Saved data to file {path}/{fileName}.{extension}");
			}
			catch (System.Exception ex)
			{
				string ErrorMessages = "File Write Error\n" + ex.Message;
				retValue = false;
				Debug.LogError(ErrorMessages);
			}
			return retValue;
		}
#endif

		public static bool ReadFile(string fileName, out string result, string path = null)
		{
			bool retValue = false;
			result = null;
			try
			{
				var dir = Application.persistentDataPath;
				if (path != null)
				{
					dir += $"/{path}";
				}
				if (Directory.Exists($"{dir}/{fileName}.txt"))
				{
					result = File.ReadAllText($"{dir}/{fileName}.txt");
					retValue = true;
				}
			}
			catch (System.Exception ex)
			{
				string ErrorMessages = "File Write Error\n" + ex.Message;
				retValue = false;
				Debug.LogError(ErrorMessages);
			}
			return retValue;
		}
	}
}