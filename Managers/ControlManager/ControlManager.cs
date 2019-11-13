using PM.UsefulThings.SimpleJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PM.UsefulThings
{
	public class ControlManager : Singleton<ControlManager>
	{
		private string controlsFilePath { get { return Application.persistentDataPath + @"/controls.txt"; } }

		public ControlManager()
		{
			Init();
		}

		public void Init()
		{
			if (!File.Exists(controlsFilePath))
			{
				return;
			}

			string custom = null;

			using (TextReader reader = new StreamReader(controlsFilePath))
			{
				custom = reader.ReadLine();
			}

			if (string.IsNullOrEmpty(custom))
			{
				return;
			}

			var controls = JSON.Parse<Dictionary<string, KeyCode>>(custom);

			FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				field.SetValue(this, controls[field.Name]);
			}
		}

		public void Save()
		{
			var controls = new Dictionary<string, KeyCode>();

			FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				controls.Add(field.Name, (KeyCode)field.GetValue(this));
			}

			using (TextWriter writer = new StreamWriter(controlsFilePath, false))
			{
				writer.WriteLine(JSON.Serialize(controls));
			}
		}

		public KeyCode Back = KeyCode.Escape;
		public KeyCode BackAlt = KeyCode.None;
		public KeyCode Inventory = KeyCode.Tab;
		public KeyCode InventoryAlt = KeyCode.None;
		public KeyCode Interact = KeyCode.E;
		public KeyCode InteractAlt = KeyCode.None;
	}
}