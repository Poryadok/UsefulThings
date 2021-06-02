using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefsClear : Editor
{

	[MenuItem("UT/Prefs/ClearAll")]
	public static void ClearAll()
	{
		ClearPrefs();
		ClearSaves();
	}

	[MenuItem("UT/Prefs/ClearPrefs")]
	public static void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("UT/Prefs/ClearSaves")]
	public static void ClearSaves()
	{
		var path = Application.persistentDataPath;
		if (Directory.Exists(path))
		{
			Directory.Delete(path, true);
		}
	}
}
