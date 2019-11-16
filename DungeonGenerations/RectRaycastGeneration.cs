using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.DungeonGenerations
{
	public static class RectRaycastGeneration
	{
		public static int Offset = 0;

		public static bool Generate<T>(List<IRectDungeonable> rooms, out List<Rect> passages)
		{
			passages = new List<Rect>();

			if (rooms.Count == 0)
			{
				return false;
			}

			if (rooms.HasAny(x => x.Size.x <= Offset * 2 || x.Size.y <= Offset * 2))
			{
				return false;
			}

			var elements = new List<IRectDungeonable>(rooms.Count * 2);

			elements.Add(rooms[0]);
			elements[0].Position = Vector2Int.zero;

			var min = Vector2Int.zero;
			var max = elements[0].Size - Vector2Int.one;

			if (rooms.Count == 1)
			{
				return true;
			}

			for (int r = 1; r < rooms.Count; r++)
			{
				var room = rooms[r];

				// randomize clipping point
				var targetSide = (Sides2D)Random.Range(0, 4);
				var sideLength = GetSideValue(room.Size, targetSide);
				var clippingPoint = Random.Range(Offset, sideLength - Offset);

				var possibilities = new List<int>(System.Linq.Enumerable.Range(GetSideValue(min, targetSide) + Offset, GetSideValue(max, targetSide) - GetSideValue(min, targetSide) + 1 - Offset - Offset));

				var targetPos = 0;
				var success = false;

				// try to randomize place to clip to
				while (possibilities.Count > 0 && !success)
				{
					targetPos = possibilities.RandomItem();

					//~raycast
					var anotherCoord = GetAnotherCoord(targetSide);
					bool hasHit = false;

					foreach (var item in elements)
					{
						if (GetSideValue(item.Position, targetSide) <= targetPos - Offset && GetSideValue(item.Position + item.Size - Vector2Int.one, targetSide) >= targetPos + Offset)
						{
							hasHit = true;
							anotherCoord = CheckAnotherCoord(anotherCoord, GetSideCord(item.Rect, targetSide), targetSide);
						}
					}

					if (!hasHit)
					{
						possibilities.Remove(targetPos);
						continue;
					}

					var newRect = GetNewRoomRect(GetPointPosition(targetPos, anotherCoord, targetSide), room.Size, clippingPoint, targetSide);
					bool isOverlaps = false;

					foreach (var item in elements)
					{
						if (item.Rect.Overlaps(newRect))
						{
							isOverlaps = true;
							break;
						}
					}

					if (isOverlaps)
					{
						possibilities.Remove(targetPos);
						continue;
					}

					room.Position = new Vector2Int((int)newRect.position.x + 1, (int)newRect.position.y + 1);
					elements.Add(room);
					passages.Add(GetPassage(GetPointPosition(targetPos, anotherCoord, targetSide), targetSide));

					if (room.Position.x < min.x)
					{
						min.x = room.Position.x;
					}
					if (room.Position.x + room.Size.x - 1 > max.x)
					{
						max.x = room.Position.x + room.Size.x - 1;
					}
					if (room.Position.y < min.y)
					{
						min.y = room.Position.y;
					}
					if (room.Position.y + room.Size.y - 1 > max.y)
					{
						max.y = room.Position.y + room.Size.y - 1;
					}

					success = true;
				}

				if (!success)
				{
					return false;
				}
			}

			return true;
		}

		private static int GetSideValue(Vector2Int values, Sides2D side)
		{
			return side == Sides2D.Bottom || side == Sides2D.Top ? values.x : values.y;
		}

		private static int GetAnotherCoord(Sides2D side)
		{
			return side == Sides2D.Bottom || side == Sides2D.Left ? int.MaxValue : int.MinValue;
		}

		private static int CheckAnotherCoord(int currentValue, int newValue, Sides2D side)
		{
			return side == Sides2D.Bottom || side == Sides2D.Left
				? newValue < currentValue ? newValue : currentValue
				: newValue > currentValue ? newValue : currentValue;
		}

		private static int GetSideCord(Rect rect, Sides2D side)
		{
			switch (side)
			{
				case Sides2D.Right:
					return (int)(rect.position.x + rect.size.x) - 1;
				case Sides2D.Top:
					return (int)(rect.position.y + rect.size.y) - 1;
				case Sides2D.Left:
					return (int)rect.position.x;
				case Sides2D.Bottom:
					return (int)rect.position.y;
				default:
					throw new System.Exception("no such side");
			}
		}

		private static Vector2Int GetPointPosition(int mainCoord, int anotherCoord, Sides2D side)
		{
			return side == Sides2D.Bottom || side == Sides2D.Top ? new Vector2Int(mainCoord, anotherCoord) : new Vector2Int(anotherCoord, mainCoord);
		}

		private static Rect GetNewRoomRect(Vector2Int pos, Vector2Int size, int clipPoint, Sides2D side)
		{
			Rect result = new Rect();
			result.size = size + Vector2Int.one * 2;
			switch (side)
			{
				case Sides2D.Right:
					var rx = pos.x + 1;
					var ry = pos.y - clipPoint - 1;
					result.position = new Vector2(rx, ry);
					break;
				case Sides2D.Top:
					var tx = pos.x - clipPoint - 1;
					var ty = pos.y + 1;
					result.position = new Vector2(tx, ty);
					break;
				case Sides2D.Left:
					var lx = pos.x - size.x - 2;
					var ly = pos.y - clipPoint - 1;
					result.position = new Vector2(lx, ly);
					break;
				case Sides2D.Bottom:
					var bx = pos.x - clipPoint - 1;
					var by = pos.y - size.y - 2;
					result.position = new Vector2(bx, by);
					break;
				default:
					throw new System.Exception("no such side");
			}

			return result;
		}

		private static Rect GetPassage(Vector2Int pos, Sides2D side)
		{
			Rect result = new Rect();
			result.size = Vector2Int.one;
			switch (side)
			{
				case Sides2D.Right:
					var rx = pos.x + 1;
					var ry = pos.y;
					result.position = new Vector2(rx, ry);
					break;
				case Sides2D.Top:
					var tx = pos.x;
					var ty = pos.y + 1;
					result.position = new Vector2(tx, ty);
					break;
				case Sides2D.Left:
					var lx = pos.x - 1;
					var ly = pos.y;
					result.position = new Vector2(lx, ly);
					break;
				case Sides2D.Bottom:
					var bx = pos.x;
					var by = pos.y - 1;
					result.position = new Vector2(bx, by);
					break;
				default:
					throw new System.Exception("no such side");
			}

			return result;
		}
	}

	public interface IRectDungeonable
	{
		Vector2Int Size { get; set; }
		Vector2Int Position { get; set; }
		Rect Rect { get; }
	}
}