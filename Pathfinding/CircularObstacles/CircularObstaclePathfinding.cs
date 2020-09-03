using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using PM.UsefulThings.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Pathfinding
{
	public static class CircularObstaclePathfinding
	{
		public static Node[] InsertStartFinish(NodeMap nodeMap, Circle[] newCircs)
		{
			nodeMap.LocalPFSimplifiedCircles = new List<Circle>(nodeMap.Circles);

			var circles = nodeMap.LocalPFSimplifiedCircles;
			if (newCircs.HasAny(x => x.Radius != 0))
			{
				Debug.LogError($"You can't insert circle with radius > 0. Remake the whole map instead");
				return null;
			}

			foreach (var circle in newCircs)
			{
				if (circles.IndexOf(circle) >= 0)
				{
					continue;
				}

				// surfing edges
				//for (var j = 0; j < circles.Count; j++)
				//{
				//	var firstCircle = circle;
				//	var secondCircle = circles[j];
				//	var internalBA = new InternalBitangents(firstCircle, secondCircle);
				//	if (!internalBA.IsOverlapping)
				//	{
				//		AddEdge(nodeMap, firstCircle, internalBA.C, secondCircle, internalBA.F);
				//		if (firstCircle.Radius != 0 && secondCircle.Radius != 0) { AddEdge(nodeMap, firstCircle, internalBA.D, secondCircle, internalBA.E); }
				//	}
				//	var external = new ExternalBitangents(firstCircle, secondCircle);
				//	if (firstCircle.Radius != 0 || secondCircle.Radius != 0)
				//	{
				//		AddEdge(nodeMap, firstCircle, external.C, secondCircle, external.F);
				//	}
				//	if (firstCircle.Radius != 0 && secondCircle.Radius != 0) { AddEdge(nodeMap, firstCircle, external.D, secondCircle, external.E); }
				//}

				// hugging edges
				//var buckets = new Dictionary<Circle, List<Node>>();
				//foreach (var node in nodeMap.Map)
				//{
				//	if (!buckets.ContainsKey(node.Circle)) { buckets[node.Circle] = new List<Node>(); }

				//	buckets[node.Circle].Add(node);
				//}

				//foreach (var bucketPair in buckets)
				//{
				//	var bucket = bucketPair.Value;

				//	var rightVec = new Vector2(bucket[0].Circle.Radius, 0);
				//	bucket.Sort(x => (Vector2.SignedAngle(rightVec, x.Point - x.Circle.Center) + 360) % 360);

				//	for (var i = 0; i < bucket.Count; i++)
				//	{
				//		for (var j = 0; j < i; j++)
				//		{
				//			if (bucket[i].Edges.HasAny(x => x.Nodes.Contains(bucket[i]) && x.Nodes.Contains(bucket[j])))
				//			{
				//				continue;
				//			}

				//			if (!IsHuggingEdgeValid(nodeMap.Intersections, bucket[i], bucket[j]))
				//			{
				//				continue;
				//			}

				//			var edge = new Edge();
				//			edge.Nodes[0] = bucket[i];
				//			edge.Nodes[1] = bucket[j];
				//			edge.Distance = CalculateHeuristic(edge);

				//			edge.Nodes[0].AddEdge(edge, false);
				//			edge.Nodes[1].AddEdge(edge, false);
				//		}
				//	}
				//}
				circles.Sort(x => Vector2.Distance(x.Center, circle.Center));
				for (var i = 0; i < circles.Count && i < 5; i++)
				{
					var secondCircle = circles[i];

					// Fill simplified map
					AddLocalSimplifiedEdge(nodeMap, circle, secondCircle);
				}

				circles.Add(circle);
			}


			foreach (var node in nodeMap.LocalPFSimplifiedMap)
			{
				node.Sort();
			}

			var result = new Node[newCircs.Length];

			for (int i = 0; i < result.Length; i++)
			{
				result[i] = nodeMap.LocalPFSimplifiedMap.Find(x => x.Circle == newCircs[i]);
			}
			return result;
		}

		/** Intersection between segment AB and circle C */
		private static Intersection SegmentCircleIntersection(Vector2 A, Vector2 B, Circle C)
		{
			var CA = C.Center - A;
			var BA = B - A;
			var u = (CA.x * BA.x + CA.y * BA.y) / (BA.x * BA.x + BA.y * BA.y);
			u = Mathf.Clamp01(u);

			var E = COPlib.vec_interpolate(A, B, u);
			var d = Vector2.Distance(C.Center, E);
			return new Intersection() { u = u, d = d, E = E, Intersects = d <= C.Radius };
		}

		private static bool IsLineOfSightFree(List<Circle> circles, Circle firstCircle, Vector2 firstPoint, Circle secondCircle, Vector2 secondPoint)
		{
			for (int i = 0; i < circles.Count; i++)
			{
				if (!circles[i].Equals(firstCircle) && !circles[i].Equals(secondCircle)
					&& SegmentCircleIntersection(firstPoint, secondPoint, circles[i]).Intersects)
				{
					return false;
				}
			}
			return true;
		}

		private static Node MakeNode(List<Node> nodeMap, Circle circle, Vector2 point)
		{
			var node = nodeMap.Find(x => x.Circle.Equals(circle) && x.Point == point);
			if (node == null)
			{
				node = new Node() { Circle = circle, Point = point };
				nodeMap.Add(node);
			}
			return node;
		}

		private static void AddEdge(NodeMap nodeMap, Circle firstCircle, Vector2 firstPoint, Circle secondCircle, Vector2 secondPoint)
		{
			if (!IsLineOfSightFree(nodeMap.Circles, firstCircle, firstPoint, secondCircle, secondPoint))
			{
				return;
			}
			var edge = new Edge();
			edge.Distance = Vector2.Distance(firstPoint, secondPoint);
			edge.Nodes[0] = MakeNode(nodeMap.LocalPFMap, firstCircle, firstPoint);
			edge.Nodes[1] = MakeNode(nodeMap.LocalPFMap, secondCircle, secondPoint);

			edge.Nodes[0].AddEdge(edge, false);
			edge.Nodes[1].AddEdge(edge, false);
		}

		private static void AddSimplifiedEdge(NodeMap nodeMap, Circle firstCircle, Circle secondCircle)
		{
			if (!IsLineOfSightFree(nodeMap.Circles, firstCircle, firstCircle.Center, secondCircle, secondCircle.Center))
			{
				return;
			}
			var edge = new Edge();
			edge.Distance = Vector2.Distance(firstCircle.Center, secondCircle.Center);
			edge.Nodes[0] = MakeNode(nodeMap.SimplifiedMap, firstCircle, firstCircle.Center);
			edge.Nodes[1] = MakeNode(nodeMap.SimplifiedMap, secondCircle, secondCircle.Center);

			edge.Nodes[0].AddEdge(edge, false);
			edge.Nodes[1].AddEdge(edge, false);
		}

		private static void AddLocalSimplifiedEdge(NodeMap nodeMap, Circle firstCircle, Circle secondCircle)
		{
			if (!IsLineOfSightFree(nodeMap.Circles, firstCircle, firstCircle.Center, secondCircle, secondCircle.Center))
			{
				return;
			}
			var edge = new Edge();
			edge.Distance = Vector2.Distance(firstCircle.Center, secondCircle.Center);
			edge.Nodes[0] = MakeNode(nodeMap.LocalPFSimplifiedMap, firstCircle, firstCircle.Center);
			edge.Nodes[1] = MakeNode(nodeMap.LocalPFSimplifiedMap, secondCircle, secondCircle.Center);

			edge.Nodes[0].AddEdge(edge, false);
			edge.Nodes[1].AddEdge(edge, false);
		}

		public static NodeMap GenerateNodemap(List<Circle> circles)
		{
			NodeMap nodeMap = new NodeMap();
			nodeMap.Circles = circles;
			// Surfing Edges
			for (var i = 0; i < circles.Count; i++)
			{
				for (var j = 0; j < i; j++)
				{
					var firstCircle = circles[i];
					var secondCircle = circles[j];

					System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
					stopwatch.Start();
					AddSimplifiedEdge(nodeMap, firstCircle, secondCircle);
					stopwatch.Stop();
					Debug.Log($"AddSimplifiedEdge: {stopwatch.Elapsed.TotalMilliseconds}");

					// Fill simplified map
					//AddSimplifiedEdge(nodeMap, firstCircle, secondCircle);

					// Fill intersections
					if (Vector2.Distance(firstCircle.Center, secondCircle.Center) <= firstCircle.Radius + secondCircle.Radius)
					{
						if (!nodeMap.Intersections.ContainsKey(firstCircle))
						{
							nodeMap.Intersections[firstCircle] = new List<Circle>();
						}
						if (!nodeMap.Intersections.ContainsKey(secondCircle))
						{
							nodeMap.Intersections[secondCircle] = new List<Circle>();
						}
						nodeMap.Intersections[firstCircle].Add(secondCircle);
						nodeMap.Intersections[secondCircle].Add(firstCircle);
					}

					// Fill general map
					//var internalBA = new InternalBitangents(firstCircle, secondCircle);
					//if (!internalBA.IsOverlapping)
					//{
					//	AddEdge(nodeMap, firstCircle, internalBA.C, secondCircle, internalBA.F);
					//	if (firstCircle.Radius != 0 && secondCircle.Radius != 0) { AddEdge(nodeMap, firstCircle, internalBA.D, secondCircle, internalBA.E); }
					//}
					//var external = new ExternalBitangents(firstCircle, secondCircle);
					//if (firstCircle.Radius != 0 || secondCircle.Radius != 0)
					//{
					//	AddEdge(nodeMap, firstCircle, external.C, secondCircle, external.F);
					//}
					//if (firstCircle.Radius != 0 && secondCircle.Radius != 0) { AddEdge(nodeMap, firstCircle, external.D, secondCircle, external.E); }
				}
			}

			//GenerateHuggingEdges(nodeMap);

			foreach (var node in nodeMap.SimplifiedMap)
			{
				node.Sort();
			}

			return nodeMap;
		}

		private static void CompleteMap(NodeMap nodeMap)
		{
			nodeMap.LocalPFMap = new List<Node>();
			for (var i = 0; i < nodeMap.LocalPFCircles.Count; i++)
			{
				for (var j = 0; j < i; j++)
				{
					var firstCircle = nodeMap.LocalPFCircles[i];
					var secondCircle = nodeMap.LocalPFCircles[j];
					GenerateSurfingEdges(nodeMap, firstCircle, secondCircle);
				}
			}

			GenerateHuggingEdges(nodeMap);

			foreach (var node in nodeMap.LocalPFMap)
			{
				node.Sort();
			}
		}

		private static void GenerateSurfingEdges(NodeMap nodeMap, Circle firstCircle, Circle secondCircle)
		{
			// Fill general map
			var internalBA = new InternalBitangents(firstCircle, secondCircle);
			if (!internalBA.IsOverlapping)
			{
				if (firstCircle.Radius != 0 || secondCircle.Radius != 0)
				{
					AddEdge(nodeMap, firstCircle, internalBA.D, secondCircle, internalBA.E);
					AddEdge(nodeMap, firstCircle, internalBA.C, secondCircle, internalBA.F);
				}
			}
			var external = new ExternalBitangents(firstCircle, secondCircle);
			if (firstCircle.Radius != 0 || secondCircle.Radius != 0)
			{
				AddEdge(nodeMap, firstCircle, external.D, secondCircle, external.E);
				AddEdge(nodeMap, firstCircle, external.C, secondCircle, external.F);
			}
			else
			{
				AddEdge(nodeMap, firstCircle, firstCircle.Center, secondCircle, secondCircle.Center);
			}
		}

		private static float CalculateHeuristic(Edge edge)
		{
			var circle = edge.Nodes[0].Circle;
			var a_angle = COPlib.vec_facing(circle, edge.Nodes[0].Point);
			var b_angle = COPlib.vec_facing(circle, edge.Nodes[1].Point);
			var delta_angle = COPlib.angle_difference(a_angle, b_angle);
			return delta_angle * circle.Radius;
		}

		private static void GenerateHuggingEdges(NodeMap nodeMap)
		{
			var buckets = new Dictionary<Circle, List<Node>>();
			foreach (var node in nodeMap.LocalPFMap)
			{
				if (!buckets.ContainsKey(node.Circle)) { buckets[node.Circle] = new List<Node>(); }

				buckets[node.Circle].Add(node);
			}

			foreach (var bucketPair in buckets)
			{
				var bucket = bucketPair.Value;
				var rightVec = new Vector2(bucket[0].Circle.Radius, 0);
				bucket.Sort(x => (Vector2.SignedAngle(rightVec, x.Point - x.Circle.Center) + 360) % 360);

				if (bucket.Count <= 1)
				{
					continue;
				}

				for (var i = 1; i < bucket.Count; i++)
				{
					if (!IsHuggingEdgeValid(nodeMap.Intersections, bucket[i - 1], bucket[i]))
					{
						continue;
					}

					var edge = new Edge();
					edge.Nodes[0] = bucket[i - 1];
					edge.Nodes[1] = bucket[i];
					edge.Distance = CalculateHeuristic(edge);

					edge.Nodes[0].AddEdge(edge, false);
					edge.Nodes[1].AddEdge(edge, false);
				}

				{ // connect last to first
					if (!IsHuggingEdgeValid(nodeMap.Intersections, bucket.Last(), bucket.First()))
					{
						continue;
					}

					var edge = new Edge();
					edge.Nodes[0] = bucket.Last();
					edge.Nodes[1] = bucket.First();
					edge.Distance = CalculateHeuristic(edge);

					edge.Nodes[0].AddEdge(edge, false);
					edge.Nodes[1].AddEdge(edge, false);
				}
			}
		}

		private static bool IsHuggingEdgeValid(Dictionary<Circle, List<Circle>> intersections, Node firstNode, Node secondNode)
		{
			List<Circle> inters;
			if (intersections.TryGetValue(firstNode.Circle, out inters))
			{
				var A = firstNode.Circle.Center;
				var B = firstNode.Point;
				var C = secondNode.Point;

				// todo check DOT as optimization
				if (Mathf.Approximately((B + C - 2 * A).sqrMagnitude * 1E-35f, 0))
				{
					var r = false;
					var l = false;
					for (int i = 0; i < inters.Count - 1; i++)
					{
						if (!r && IsPointOnTheRight(A, B, inters[i].Center))
						{
							r = true;
						}
						if (!l && IsPointOnTheLeft(A, B, inters[i].Center))
						{
							l = true;
						}
					}

					if (r && l)
					{
						return false;
					}
				}

				Vector2 G;
				foreach (var secondCircle in inters)
				{
					if (Vector2.Distance(firstNode.Circle.Center, secondCircle.Center) == firstNode.Circle.Radius + secondCircle.Radius)
					{
						G = firstNode.Circle.Center + (secondCircle.Center - firstNode.Circle.Center).normalized * firstNode.Circle.Radius;
					}
					else
					{
						var halfDist = (firstNode.Circle.Radius * firstNode.Circle.Radius - secondCircle.Radius * secondCircle.Radius + (firstNode.Circle.Radius + secondCircle.Radius) * (firstNode.Circle.Radius + secondCircle.Radius)) / (2 * (firstNode.Circle.Radius + secondCircle.Radius));
						var angle = Mathf.Acos(halfDist / firstNode.Circle.Radius);
						G = firstNode.Circle.Center + ((secondCircle.Center - firstNode.Circle.Center).normalized * firstNode.Circle.Radius).Rotate(angle);
					}

					// check if G is inside arc of AB and AC vectors
					var f = IsPointOnTheLeft(A, B, G);
					var s = IsPointOnTheRight(A, C, G);

					if (f && s)
					{
						return false;
					}
				}
			}

			return true;
		}

		private static bool IsPointOnTheRight(Vector2 A, Vector2 B, Vector2 G)
		{
			return (G.y - A.y) * (B.x - A.x) - (G.x - A.x) * (B.y - A.y) <= 0;
		}

		private static bool IsPointOnTheLeft(Vector2 A, Vector2 B, Vector2 G)
		{
			return (G.y - A.y) * (B.x - A.x) - (G.x - A.x) * (B.y - A.y) >= 0;
		}

		private static void FillPFCircles(NodeMap nodeMap, List<Node> path)
		{
			nodeMap.LocalPFCircles = new List<Circle>();
			nodeMap.TargetPFCircles = new List<Circle>();

			foreach (var node in path)
			{
				nodeMap.LocalPFCircles.AddNoDoubling(node.Circle);
				nodeMap.TargetPFCircles.AddNoDoubling(node.Circle);
			}
			foreach (var circle in nodeMap.TargetPFCircles)
			{
				AddIntersections(nodeMap, circle);
			}
		}

		private static void AddIntersections(NodeMap nodeMap, Circle circle)
		{
			List<Circle> inters;
			if (nodeMap.Intersections.TryGetValue(circle, out inters))
			{
				foreach (var inter in inters)
				{
					if (nodeMap.LocalPFCircles.AddNoDoubling(inter))
					{
						AddIntersections(nodeMap, inter);
					}
				}
			}
		}

		/** Pathfinding */
		public static List<IPathNode> FindPath(NodeMap nodemap, Circle start, Circle finish, out NodeMap nm)
		{
			nodemap.CloneSimplifiedMap();
			var stfn = InsertStartFinish(nodemap, new Circle[2] { start, finish });

			var simplifiedPath = AStar.FindPath(stfn[0] as IPathNode, stfn[1] as IPathNode).Cast(x => x as Node);
			simplifiedPath.Insert(0, stfn[0]);

			FillPFCircles(nodemap, simplifiedPath);

			CompleteMap(nodemap);

			var st = nodemap.LocalPFMap.Find(x => x.Circle == start);
			var fn = nodemap.LocalPFMap.Find(x => x.Circle == finish);
			nm = nodemap;
			return AStar.FindPath(st as IPathNode, fn as IPathNode);
		}
	}

	public class NodeMap
	{
		public List<Node> SimplifiedMap = new List<Node>();
		public List<Circle> Circles = new List<Circle>();
		public List<Node> LocalPFSimplifiedMap = new List<Node>();
		public List<Node> LocalPFMap;
		public List<Circle> LocalPFSimplifiedCircles = new List<Circle>();
		public List<Circle> LocalPFCircles = new List<Circle>();
		public List<Circle> TargetPFCircles = new List<Circle>();
		public Dictionary<Circle, List<Circle>> Intersections = new Dictionary<Circle, List<Circle>>();

		// generates too much garbage
		public void CloneSimplifiedMap()
		{
			var cache = new Dictionary<Node, Node>();

			LocalPFSimplifiedMap.Clear();
			foreach (var node in SimplifiedMap)
			{
				var newNode = new Node(node);
				LocalPFSimplifiedMap.Add(newNode);
				cache[node] = newNode;
			}
			for (int i = 0; i < LocalPFSimplifiedMap.Count; i++)
			{
				foreach (var edge in SimplifiedMap[i].Edges)
				{
					LocalPFSimplifiedMap[i].Edges.Add(new Edge(edge.Distance, cache[edge.Nodes[0]], cache[edge.Nodes[0]]));
				}
				foreach (var neighbour in SimplifiedMap[i].Neighbours)
				{
					LocalPFSimplifiedMap[i].Neighbours.Add(cache[neighbour as Node]);
				}
			}
		}
	}

	public class Node : IPathNode
	{
		public Circle Circle;
		public Vector2 Point;

		public List<Edge> Edges = new List<Edge>();
		public List<IPathNode> Neighbours = new List<IPathNode>();
		private Dictionary<Node, float> neighboursDict = new Dictionary<Node, float>();

		public float Priority { get; set; }
		public int QueueIndex { get; set; }
		public long InsertionIndex { get; set; }

		public Node() { }

		public Node(Node node)
		{
			this.Circle = node.Circle;
			this.Point = node.Point;
		}

		public float GetCost(IPathNode current)
		{
			var node = current as Node;
			return Vector2.Distance(this.Circle.Center + this.Point, node.Circle.Center + node.Point);
		}

		public float GetHeuristic(IPathNode target)
		{
			var node = target as Node;
			return Mathf.Sqrt(Vector2.Distance(this.Circle.Center + this.Point, node.Circle.Center + node.Point));
		}

		public List<IPathNode> GetOrderedNeighbours()
		{
			return Neighbours;
		}

		public void AddEdge(Edge edge, bool autoSort = true)
		{
			Edges.Add(edge);
			if (autoSort)
			{
				Sort(edge);
			}
		}

		public void Sort(Edge edge = null)
		{
			if (edge != null)
			{
				UpdateHeuristic(edge);
			}
			else
			{
				foreach (var edg in Edges)
				{
					UpdateHeuristic(edg);
				}
			}

			System.Comparison<KeyValuePair<Node, float>> comp = (x, y) => x.Value == y.Value ? 0 : x.Value > y.Value ? 1 : -1;

			var list = neighboursDict.ToList();
			list.Sort(comp);
			System.Func<KeyValuePair<Node, float>, IPathNode> convert = (x) => x.Key as IPathNode;
			Neighbours = list.Cast(convert);
		}

		private void UpdateHeuristic(Edge edge)
		{
			var neighbour = edge.Nodes[0] == this ? edge.Nodes[1] : edge.Nodes[0];
			if (neighboursDict.ContainsKey(neighbour))
			{
				neighboursDict[neighbour] = Mathf.Min(neighboursDict[neighbour], edge.Distance);
			}
			else
			{
				neighboursDict[neighbour] = edge.Distance;
			}
		}
	}

	public class Edge
	{
		public float Distance;
		public Node[] Nodes = new Node[2];

		public Edge() { }
		public Edge(float dist, Node first, Node second)
		{
			this.Distance = dist;
			Nodes[0] = first;
			Nodes[1] = second;
		}
	}

	internal class HuggingEdge
	{
		public bool Valid;
		public Vector2 MidBD;
		public float AngleToA;
		public float AngleToE;
		public Vector2 B;
		public Vector2 D;
		public Vector2 FckThisIsWrong;
	}


	public struct Circle
	{
		public Vector2 Center;
		public float Radius;

		public Circle(Vector2 center)
		{
			this.Center = center;
			this.Radius = 0f;
		}

		public Circle(Vector2 center, float radius)
		{
			this.Center = center;
			this.Radius = radius;
		}

		public override bool Equals(object obj)
		{
			if (obj is Circle other)
			{
				return this.Center == other.Center && this.Radius == other.Radius;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Center.GetHashCode() + Radius.GetHashCode();
		}

		public static Circle operator *(Circle a, float b)
		{
			return new Circle() { Center = a.Center, Radius = a.Radius * b };
		}

		public static Circle operator /(Circle a, float b)
		{
			return new Circle() { Center = a.Center, Radius = a.Radius / b };
		}

		public static bool operator ==(Circle a, Circle b)
		{
			return a.Center == b.Center && a.Radius == b.Radius;
		}

		public static bool operator !=(Circle a, Circle b)
		{
			return !(a == b);
		}
	}

	public struct Intersection
	{
		public float u;
		public float d;
		public Vector2 E;
		public bool Intersects;
	}

	/** Calculations needed for internal bitangents */
	internal class InternalBitangents
	{
		public bool IsOverlapping => float.IsNaN(theta);

		public Circle A;
		public Circle B;

		public InternalBitangents(Circle A, Circle B)
		{
			this.A = A;
			this.B = B;
		}

		public float theta
		{
			get
			{
				var P = Vector2.Distance(this.A.Center, this.B.Center);
				var cos_angle = (this.A.Radius + this.B.Radius) / P;
				return Mathf.Acos(cos_angle);
			}
		}

		public float AB_angle() { return COPlib.vec_facing(this.A, this.B); }

		public float BA_angle() { return COPlib.vec_facing(this.B, this.A); }

		public Vector2 C => COPlib.direction_step(this.A.Center, this.A.Radius, this.AB_angle() - this.theta);
		public Vector2 D => COPlib.direction_step(this.A.Center, this.A.Radius, this.AB_angle() + this.theta);
		public Vector2 E => COPlib.direction_step(this.B.Center, this.B.Radius, this.BA_angle() + this.theta);
		public Vector2 F => COPlib.direction_step(this.B.Center, this.B.Radius, this.BA_angle() - this.theta);
	}


	/** Calculations needed for external bitangents */
	internal class ExternalBitangents
	{
		public Circle A;
		public Circle B;

		public ExternalBitangents(Circle A, Circle B)
		{
			this.A = A;
			this.B = B;
		}

		public float theta
		{
			get
			{
				var P = Vector2.Distance(this.A.Center, this.B.Center);
				var cos_angle = (this.A.Radius - this.B.Radius) / P;
				return Mathf.Acos(cos_angle);
			}
		}

		public float AB_angle() { return COPlib.vec_facing(this.A, this.B); }
		public float BA_angle() { return COPlib.vec_facing(this.B, this.A); }

		public Vector2 C => COPlib.direction_step(this.A.Center, this.A.Radius, this.AB_angle() - this.theta);
		public Vector2 D => COPlib.direction_step(this.A.Center, this.A.Radius, this.AB_angle() + this.theta);
		public Vector2 E => COPlib.direction_step(this.B.Center, this.B.Radius, this.AB_angle() + this.theta);
		public Vector2 F => COPlib.direction_step(this.B.Center, this.B.Radius, this.AB_angle() - this.theta);
	}
}