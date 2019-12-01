using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
				Debug.Log($"Saved data to file {dir}");
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