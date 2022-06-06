using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public class Hexagrid<T> : IEnumerable<KeyValuePair<Vector3Int, T>> where T : class
	{
		private readonly Dictionary<Vector3Int, T> cells = new Dictionary<Vector3Int, T>();

		public System.Func<Vector3Int, T> HexcellConstructor { get; set; }

		public float CellSize;
		public bool IsHorizontalAlignment;

		public Hexagrid(System.Func<Vector3Int, T> hexcellConstructor, float cellSize, bool isHorizontalAlignment)
		{
			this.HexcellConstructor = hexcellConstructor;
			this.CellSize = cellSize;
			this.IsHorizontalAlignment = isHorizontalAlignment;
		}

		[System.Runtime.CompilerServices.IndexerName("Position")]
		public T this[Vector3Int index]
		{
			get
			{
				//valid cell should have sum of axis == 0
				if (index.x + index.y + index.z != 0)
				{
					throw new System.Exception("Invalid cell position requested");
				}

				if (cells.ContainsKey(index))
					return cells[index];

				var cell = HexcellConstructor(index);
				cells[index] = cell;

				return cell;
			}
			set
			{
				//valid cell should have sum of axis == 0
				if (index.x + index.y + index.z != 0)
				{
					throw new System.Exception("Invalid cell position set");
				}

				if (cells.ContainsKey(index))
				{
					//todo destroy old cell
				}

				cells[index] = value;
			}
		}

		public List<T> GetNeighbours(Vector3Int center)
		{
			return GetCellsInRadius(center, 1, false);
		}

		public T GetCell(Vector3Int pos)
		{
			return this[pos];
		}

		public bool IsCellCreated(Vector3Int pos)
		{
			return cells.ContainsKey(pos);
		}

		public List<T> GetCellsInRadius(Vector3Int center, int offset, bool isCenterIncluded = true)
		{
			//valid cell should have sum of axis == 0
			if (center.x + center.y + center.z != 0)
			{
				throw new System.Exception("Invalid center position set");
			}

			offset = Mathf.Abs(offset);

			var result = new List<T>();

			for (int x = -offset; x <= offset; x++)
			{
				for (int y = -offset; y <= offset; y++)
				{
					if (Mathf.Abs(x + y) > offset)
						continue;
					
					result.Add(this[center + new Vector3Int(x, y, -x -y)]);
				}
			}

			if (!isCenterIncluded)
				result.Remove(this[center]);

			return result;
		}

		public List<T> GetExistingCellsInRadius(Vector3Int center, int offset, bool isCenterIncluded = true)
		{
			//valid cell should have sum of axis == 0
			if (center.x + center.y + center.z != 0)
			{
				throw new System.Exception("Invalid center position set");
			}

			offset = Mathf.Abs(offset);

			var result = new List<T>();

			for (int x = -offset; x <= offset; x++)
			{
				for (int y = -offset; y <= offset; y++)
				{
					for (int z = -offset; z <= offset; z++)
					{
						//valid cell should have sum of axis == 0
						if (x + y + z != 0)
							continue;

						if (IsCellCreated(center + new Vector3Int(x, y, z)))
						{
							result.Add(this[center + new Vector3Int(x, y, z)]);
						}
					}
				}
			}

			if (!isCenterIncluded)
				result.Remove(this[center]);

			return result;
		}

		public ICollection<T> Cells
		{
			get
			{
				return cells.Values;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return cells.GetEnumerator();
		}

		public IEnumerator<KeyValuePair< Vector3Int, T>> GetEnumerator()
		{
			return cells.GetEnumerator();
		}

		public void RemoveAt(Vector3Int pos)
		{
			cells.Remove(pos);
		}

		public void Remove(T cell)
		{
			KeyValuePair<Vector3Int, T> result;

			if (cells.TryFind(x => x == cell, out result))
			{
				cells.Remove(result.Key);
			}
		}

		public void Clear()
		{
			cells.Clear();
		}

		public Vector3Int ConvertUnityToHexPosition(Vector3 worldPos)
		{
			return CubeRound(ConvertUnityToHexWorldPosition(worldPos));
		}

		public Vector3 ConvertHexToUnityPosition(Vector3Int hexPosition)
		{
			return ConvertHexToUnityPosition(hexPosition.x, hexPosition.y, hexPosition.z);
		}

		public Vector3 ConvertHexWorldToUnityPosition(Vector3 hexPosition)
		{
			return ConvertHexToUnityPosition(hexPosition.x, hexPosition.y, hexPosition.z);
		}

		private Vector3 ConvertHexToUnityPosition(float hexPositionX,float hexPositionY,float hexPositionZ)
		{
			if (IsHorizontalAlignment)
			{
				return new Vector3(hexPositionY * CellSize * 1.5f / 2, 0,
					(hexPositionX - hexPositionZ) * CellSize * Mathf.Sqrt(3) / 4);
			}
			else
			{
				return new Vector3((hexPositionX - hexPositionZ) * CellSize * Mathf.Sqrt(3) / 4, 0,
					hexPositionY * CellSize * 1.5f / 2);
			}
		}

		public Vector3 ConvertUnityToHexWorldPosition(Vector3 worldPos)
		{
			var q = (Mathf.Sqrt(3) / 3 * worldPos.x - 1f / 3 * worldPos.z) / CellSize;
			var r = 2f / 3 * worldPos.z / CellSize;
			return new Vector3(r, q, -q - r);
		}

		private Vector3Int CubeRound(Vector3 frac)
		{
			var q = Mathf.RoundToInt(frac.x);
			var r = Mathf.RoundToInt(frac.y);
			var s = Mathf.RoundToInt(frac.z);

			var q_diff = Mathf.Abs(q - frac.x);
			var r_diff = Mathf.Abs(r - frac.y);
			var s_diff = Mathf.Abs(s - frac.z);

			if (q_diff > r_diff && q_diff > s_diff)
				q = -r - s;
			else if (r_diff > s_diff)
				r = -q - s;
			else
				s = -q - r;

			return new Vector3Int(q, r, s);
		}
	}

	public static class MathHex
	{
		public static Vector3Int RandomInRadius(int radius)
		{
			var min = -radius;
			var max = radius + 1;
			int x = Random.Range(min, max);
			if (x > 0)
			{
				max -= x;
			}
			else
			{
				min -= x;
			}

			int y = Random.Range(min, max);
			int z = -x - y;
			return new Vector3Int(x, y, z);
		}
		
		public static Vector3Int RandomOnCircle(int radius)
		{
			var min = -radius;
			var max = radius + 1;
			int x = Random.Range(min, max);
			if (x > 0)
			{
				max -= x;
			}
			else
			{
				min -= x;
			}

			int y = Random.value > 0.5f ? max - 1 : min;
			int z = -x - y;

			int normalizeRandom = Random.Range(0, 3);
			switch (normalizeRandom)
			{
				case 0:
					return new Vector3Int(x, y, z);
				case 1:
					return new Vector3Int(z, x, y);
				default:
					return new Vector3Int(y, z, x);
			}
		}

		public static int CellCountInRadius(int radius)
		{
			return (int)(1 + (1 + radius) * radius / 2f * 6);
		}
	}
}