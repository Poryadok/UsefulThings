using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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

		public static bool WriteProjectFile(string data, string fileName, string extension = "txt", string path = null)
		{
			bool retValue = false;
			try
			{
				var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"{path}/{fileName}.{extension}");
				if (textAsset == null)
				{
					textAsset = new TextAsset(data);
					AssetDatabase.CreateAsset(textAsset, $"{path}/{fileName}.{extension}");
				}
				else
				{
					File.WriteAllText(AssetDatabase.GetAssetPath(textAsset), data);
				}
				EditorUtility.SetDirty(textAsset);
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
				if (Directory.Exists(dir))
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