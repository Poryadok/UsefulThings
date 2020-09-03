using PM.UsefulThings.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Pathfinding
{
	public class AStar
	{
		public static List<IPathNode> FindPath(IPathNode start, IPathNode finish)
		{
			if (start == finish)
			{
				return new List<IPathNode>();
			}

			var frontier = new GenericPriorityQueue<IPathNode, float>(1000);
			frontier.Enqueue(start, 0);
			var cameFrom = new Dictionary<IPathNode, IPathNode>();
			var costSoFar = new Dictionary<IPathNode, float>();
			IPathNode current;
			IPathNode next;
			List<IPathNode> neighbours;
			float newCost;
			float priority;

			cameFrom[start] = null;
			costSoFar[start] = 0;


			while (frontier.Count > 0)
			{
				current = frontier.Dequeue();

				if (current == finish)
				{
					break;
				}

				neighbours = current.GetOrderedNeighbours();

				for (int i = 0; i < neighbours.Count; i++)
				{
					next = neighbours[i];

					newCost = costSoFar[current] + next.GetHeuristic(current);

					if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
					{
						if (costSoFar.ContainsKey(next))
						{
							costSoFar[next] = newCost;
						}
						else
						{
							costSoFar.Add(next, newCost);
						}

						priority = newCost + next.GetHeuristic(finish);
						if (frontier.Contains(next))
						{
							frontier.UpdatePriority(next, priority);
						}
						else
						{
							frontier.Enqueue(next, priority);
						}


						cameFrom[next] = current;
					}
				}
			}

			var path = new List<IPathNode>();

			if (cameFrom.ContainsKey(finish))
			{
				IPathNode last = finish;
				path.Add(finish);
				while (cameFrom[last] != start)
				{
					path.Add(cameFrom[last]);
					last = cameFrom[last];
				}

				path.Reverse();
			}

			return path;
		}


		public static bool HasPath(IPathNode start, IPathNode finish)
		{
			return FindPath(start, finish).Count > 0;
		}
	}

	public interface IPathNode : IGenericPriorityQueueNode<float>
	{
		List<IPathNode> GetOrderedNeighbours();
		float GetCost(IPathNode current);
		float GetHeuristic(IPathNode target);
	}
}