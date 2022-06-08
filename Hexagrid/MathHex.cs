using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings
{
	public static class MathHex
	{
		public static Vector3Int RandomInRadius(int radius)
		{
			var min = -radius + 1;
			var max = radius;
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
			var min = -radius + 1;
			var max = radius;
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
			radius -= 1;
			return (int)(1 + (1 + radius) * radius / 2f * 6);
		}

		public static int Distance(Vector3Int one, Vector3Int another)
		{
			return Mathf.Max(Mathf.Abs(one.x - another.x), Mathf.Abs(one.y - another.y));
		}

		public static float Distance(Vector3 one, Vector3 another)
		{
			return Mathf.Max(Mathf.Abs(one.x - another.x), Mathf.Abs(one.y - another.y));
		}
		
		public static float VolumetricDistance(Vector3 one, Vector3 another, int oneSize, int anotherSize)
		{
			var sumSize = oneSize + anotherSize;
			if (oneSize > 0)
			{
				sumSize -= 1;
			}
			if (anotherSize > 0)
			{
				sumSize -= 1;
			}
			return Mathf.Max(Mathf.Abs(one.x - another.x), Mathf.Abs(one.y - another.y)) - sumSize;
		}
	}
}