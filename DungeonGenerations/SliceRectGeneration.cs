using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.DungeonGenerations
{
	public static class SliceRectGeneration
	{
		private enum States
		{
			Empty,
			Wall,
			Floor
		}

		public static bool Generate<T>(Vector2Int rectSize, Vector2Int minRoomSize, Vector2Int maxRoomSize, out List<ISliceDungeonable> rooms, out List<Vector2Int> doors) where T : ISliceDungeonable, new()
		{
			rooms = new List<ISliceDungeonable>();
			doors = new List<Vector2Int>();
			var doorData = new List<(ISliceDungeonable, ISliceDungeonable)>();

			var array = new States[rectSize.x, rectSize.y];

			var minSize = minRoomSize.x * minRoomSize.y;
			var maxSize = maxRoomSize.x * maxRoomSize.y;

			for (int i = 0; i < rectSize.x; i++)
			{
				array[i, 0] = States.Wall;
				array[i, rectSize.y - 1] = States.Wall;
			}
			for (int i = 0; i < rectSize.y; i++)
			{
				array[0, i] = States.Wall;
				array[rectSize.x - 1, i] = States.Wall;
			}

			var tries = 0;

			while (tries < 100)
			{
				tries++;
				var size = new Vector2Int(Random.Range(minRoomSize.x, maxRoomSize.x), Random.Range(minRoomSize.y, maxRoomSize.y));

				var room = new T();

				var start = Vector2Int.zero;

				for (int y = 0; y < array.GetLength(1); y++)
				{
					for (int x = 0; x < array.GetLength(0); x++)
					{
						if (array[x, y] == States.Empty)
						{
							start = new Vector2Int(x, y);
							room.Positions.Add(new Vector2Int(x, y));
							break;
						}
					}
					if (start != Vector2Int.zero)
					{
						break;
					}
				}

				for (int y = 0; y < size.y; y++)
				{
					for (int x = 0; x < size.x; x++)
					{
						if (start.x + x >= array.GetLength(0) || start.y + y >= array.GetLength(1))
						{
							continue;
						}

						if (array[start.x + x, start.y + y] == States.Empty)
						{
							array[start.x + x, start.y + y] = States.Floor;
							room.Positions.Add(new Vector2Int(start.x + x, start.y + y));
						}
					}
				}

				for (int y = 0; y < array.GetLength(1); y++)
				{
					for (int x = 0; x < array.GetLength(0); x++)
					{
						var vec = new Vector2Int(x, y);

						if (array[x, y] != States.Empty)
						{
							continue;
						}


						if (room.Positions.Contains(vec))
						{
							continue;
						}

						if (room.Positions.HasAny(t => Vector2Int.Distance(t, vec) < 2))
						{
							array[x, y] = States.Wall;
						}
					}
				}

				rooms.Add(room);

				var isDone = true;
				foreach (var item in array)
				{
					if (item == States.Empty)
					{
						isDone = false;
						break;
					}
				}
				if (isDone)
				{
					break;
				}
			}
			
			for (int i = 0; i < rooms.Count; i++)
			{
				if (rooms[i].Positions.Count < minSize)
				{
					foreach (var tile in rooms[i].Positions)
					{
						array[tile.x, tile.y] = States.Wall;
					}
					rooms.RemoveAt(i);
					i--;
				}
			}

			for (int i = 0; i < rooms.Count; i++)
			{
				foreach (var tile in rooms[i].Positions)
				{
					var nearestTiles = 0;

					if (rooms[i].Positions.Contains(new Vector2Int(tile.x - 1, tile.y)))
					{
						nearestTiles++;
					}

					if (rooms[i].Positions.Contains(new Vector2Int(tile.x, tile.y - 1)))
					{
						nearestTiles++;
					}

					if (rooms[i].Positions.Contains(new Vector2Int(tile.x + 1, tile.y)))
					{
						nearestTiles++;
					}

					if (rooms[i].Positions.Contains(new Vector2Int(tile.x, tile.y + 1)))
					{
						nearestTiles++;
					}

					if (nearestTiles < 2)
					{
						return false;
					}
				}
			}

			//entrance
			{
				if (array[1, rectSize.y / 2 + rectSize.y % 2] != States.Floor)
				{
					return false;
				}

				var entrancePos = new Vector2Int(0, rectSize.y / 2 + rectSize.y % 2);

				doors.Add(entrancePos);
			}

			for (int i = 0; i < rooms.Count; i++)
			{
				var targetRoom = rooms[i];
				var neighbour = targetRoom;

				List<ISliceDungeonable> connected = new List<ISliceDungeonable>();

				foreach (var data in doorData)
				{
					if (data.Item1 == targetRoom)
					{
						connected.Add(data.Item2);
					}
					if (data.Item2 == targetRoom)
					{
						connected.Add(data.Item1);
					}
				}

				foreach (var room in rooms)
				{
					if (room == targetRoom)
					{
						continue;
					}
					if (connected.Contains(room))
					{
						continue;
					}

					if (targetRoom.Positions.HasAny(x=> room.Positions.HasAny(r => Vector2Int.Distance(x,r) <= 2.1f)))
					{
						neighbour = room;
						break;
					}
				}

				if (targetRoom == neighbour)
				{
					if (connected.Count == 0)
					{
						return false;
					}
					else
					{
						continue;
					}
				}

				var tiles = new List<Vector2Int>();

				for (int y = 0; y < array.GetLength(1); y++)
				{
					for (int x = 0; x < array.GetLength(0); x++)
					{
						var vec = new Vector2Int(x, y);

						if (array[x, y] != States.Wall)
						{
							continue;
						}

						if (targetRoom.Positions.HasAny(t => Vector2Int.Distance(t, vec) < 1.1f) && neighbour.Positions.HasAny(t => Vector2Int.Distance(t, vec) < 1.1f))
						{
							tiles.Add(vec);
						}
					}
				}

				if (tiles.Count == 0)
				{
					return false;
				}

				doors.Add(tiles.RandomItem());
				doorData.Add((targetRoom, neighbour));
			}

			for (int i = 0; i < rooms.Count; i++)
			{
				if (Random.value < 0.7f)
				{
					continue;
				}

				var targetRoom = rooms[i];
				var neighbour = targetRoom;

				List<ISliceDungeonable> connected = new List<ISliceDungeonable>();

				foreach (var data in doorData)
				{
					if (data.Item1 == targetRoom)
					{
						connected.Add(data.Item2);
					}
					if (data.Item2 == targetRoom)
					{
						connected.Add(data.Item1);
					}
				}

				foreach (var room in rooms)
				{
					if (room == targetRoom)
					{
						continue;
					}
					if (connected.Contains(room))
					{
						continue;
					}

					if (targetRoom.Positions.HasAny(x => room.Positions.HasAny(r => Vector2Int.Distance(x, r) <= 2.1f)))
					{
						neighbour = room;
						break;
					}
				}

				if (targetRoom == neighbour)
				{
					if (connected.Count == 0)
					{
						return false;
					}
					else
					{
						continue;
					}
				}

				var tiles = new List<Vector2Int>();

				for (int y = 0; y < array.GetLength(1); y++)
				{
					for (int x = 0; x < array.GetLength(0); x++)
					{
						var vec = new Vector2Int(x, y);

						if (array[x, y] != States.Wall)
						{
							continue;
						}

						if (targetRoom.Positions.HasAny(t => Vector2Int.Distance(t, vec) < 1.1f) && neighbour.Positions.HasAny(t => Vector2Int.Distance(t, vec) < 1.1f))
						{
							tiles.Add(vec);
						}
					}
				}

				if (tiles.Count == 0)
				{
					return false;
				}

				doors.Add(tiles.RandomItem());
				doorData.Add((targetRoom, neighbour));
			}

			return true;
		}
	}

	public interface ISliceDungeonable
	{
		List<Vector2Int> Positions { get; }
	}
}