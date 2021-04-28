using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Pathfinding.COP
{
	public static class COPlib
	{
		public static Vector2 direction_step(Vector2 start, float distance, float angle)
		{
			return start + vec_polar(distance, angle);
		}

		public static Vector2 vec_polar(float r, float a)
		{
			return new Vector2(r * Mathf.Cos(a), r * Mathf.Sin(a));
		}

		public static Vector2 vec_interpolate(Vector2 first, Vector2 second, float scale)
		{
			return first + (second - first) * scale;
		}

		public static float vec_facing(Circle p, Circle q)
		{
			var dx = q.Center.x - p.Center.x;
			var dy = q.Center.y - p.Center.y;
			return Mathf.Atan2(dy, dx);
		}

		public static float vec_facing(Circle p, Vector2 q)
		{
			var dx = q.x - p.Center.x;
			var dy = q.y - p.Center.y;
			return Mathf.Atan2(dy, dx);
		}

		public static float angle_difference(float a, float b)
		{
			return Mathf.Abs(b - a) % (2 * Mathf.PI);
		}
	}
}