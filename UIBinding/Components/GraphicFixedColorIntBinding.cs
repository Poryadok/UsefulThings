using UnityEngine;
using UnityEngine.UI;
using System;

using UIBinding.Base;
using System.Collections.Generic;

namespace UIBinding.Components
{
	[RequireComponent(typeof(Graphic))]
	public class GraphicFixedColorIntBinding : BaseBinding<IntProperty>
	{
		public enum CheckType
		{
			Equal,
			Greater,
			Less,
			InRange
		}

		[SerializeField]
		private CheckType m_checkType = CheckType.Equal;
		[SerializeField]
		private string m_value = "0";
		[SerializeField]
		private Color m_colorTrue = Color.white;
		[SerializeField]
		private Color m_colorFalse = Color.white;

		private List<int> m_values = new List<int>();

		private Graphic m_graphic;

		private void Awake()
		{
			m_graphic = GetComponent<Graphic>();
			ParseValues();
		}

		private void ParseValues()
		{
			m_values.Clear();

			switch (m_checkType) {
				case CheckType.Equal: {
						var trimmedValue = m_value.Trim();
						var stringValues = trimmedValue.Split(',');
						foreach (var stringValue in stringValues) {
							AddValue(stringValue);
						}
					}
					break;
				case CheckType.Greater:
				case CheckType.Less: {
						AddValue(m_value);
					}
					break;
				case CheckType.InRange: {
						var trimmedValue = m_value.Trim();
						var stringValues = trimmedValue.Split(',');
						if (stringValues.Length < 2) {
#if UNITY_EDITOR
							Debug.LogErrorFormat("Couldn't parse \"{0}\" to \"{1}\" check type", m_value, m_checkType.ToString());
#endif
							return;
						}
						for (int i = 0; i < 2; i++) {
							var stringValue = stringValues[i];
							AddValue(stringValue);
						}
					}
					break;
			}
		}

		private void AddValue(string stringValue)
		{
			var value = 0;
			if (int.TryParse(stringValue, out value)) {
				m_values.Add(value);
			}
#if UNITY_EDITOR
			else {
				Debug.LogErrorFormat("Couldn't parse \"{0}\" to {1} type", m_value, m_checkType.ToString());
			}
#endif
		}


		protected override void OnUpdateValue()
		{
			var result = false;
			switch (m_checkType) {
				case CheckType.Equal: {
						var propertyValue = property.value;
						foreach (var value in m_values) {
							if (propertyValue == value) {
								result = true;
								break;
							}
						}
					}
					break;
				case CheckType.Greater: {
						var value = m_values[0];
						result = property.value > value;
					}
					break;
				case CheckType.Less: {
						var value = m_values[0];
						result = property.value < value;
					}
					break;
				case CheckType.InRange: {
						var minValue = m_values[0];
						var maxValue = m_values[1];
						result = (property.value >= minValue) && (property.value <= maxValue);
					}
					break;
			}
			m_graphic.color = result ? m_colorTrue : m_colorFalse;
		}
	}
}

