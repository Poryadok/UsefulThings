using PM.UsefulThings;
using PM.UsefulThings.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PM.UsefulThings.Pathfinding
{
	public class RectangleObstaclePathfinding
    {
        public static Node[] InsertStartFinish(NodeMap nodeMap, Vector2[] startfinish)
        {
            var result = new Node[2];

            return result;
        }


        public static NodeMap GenerateNodemap(List<Rectangle> Rectangles)
        {
            NodeMap nodeMap = new NodeMap();
            nodeMap.Rectangles = Rectangles;

            Rectangles.Sort((f, s) =>
                {
                    int fx = (int)(f.Center.x - f.Horizontal / 2) / 10;
                    int sx = (int)(s.Center.x - s.Horizontal / 2) / 10;
                    if (fx == sx)
                    {
                        return (f.Center.y - f.Vertical / 2) > (s.Center.y - s.Vertical / 2) ? 1 : -1;
                    }
                    else
                    {
                        return fx > sx ? 1 : -1;
                    }
                });

            var startNode = MakeNode(nodeMap.Map, nodeMap.Rectangles[0].Center - new Vector2(nodeMap.Rectangles[0].Horizontal / 2, 0));


            return nodeMap;
        }
        
        private static Node MakeNode(List<Node> existingNodes, Vector2 point)
        {
            var node = existingNodes.Find(x => x.Position == point);
            if (node == null)
            {
                node = new Node() { Position = point };
                existingNodes.Add(node);
            }
            return node;
        }



        public class NodeMap
        {
            public List<Node> Map = new List<Node>();
            public List<Rectangle> Rectangles;
        }

        public class Node : IPathNode
        {
            public Vector2 Position;

            public List<Edge> Edges = new List<Edge>();
            public List<IPathNode> Neighbours = new List<IPathNode>();
            private Dictionary<Node, float> neighboursDict = new Dictionary<Node, float>();

            public float Priority { get; set; }
            public int QueueIndex { get; set; }
            public long InsertionIndex { get; set; }

            public Node() { }

            public Node(Node node)
            {
                this.Position = node.Position;
            }

            public float GetCost(IPathNode current)
            {
                var node = current as Node;
                return Vector2.Distance(this.Position, node.Position);
            }

            public float GetHeuristic(IPathNode target)
            {
                var node = target as Node;
                return Mathf.Sqrt(Vector2.Distance(this.Position, node.Position));
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

        public struct Rectangle
        {
            public Vector2 Center;
            public float Horizontal;
            public float Vertical;

            public Rectangle(Vector2 center)
            {
                this.Center = center;
                this.Horizontal = 0f;
                this.Vertical = 0f;
            }

            public Rectangle(Vector2 center, float horizontal, float vertical)
            {
                this.Center = center;
                this.Horizontal = horizontal;
                this.Vertical = vertical;
            }

            public override bool Equals(object obj)
            {
                if (obj is Rectangle other)
                {
                    return this.Center == other.Center && this.Horizontal == other.Horizontal && this.Vertical == other.Vertical;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Center.GetHashCode() + Horizontal.GetHashCode() + Vertical.GetHashCode();
            }

            public static Rectangle operator *(Rectangle a, float b)
            {
                return new Rectangle() { Center = a.Center, Horizontal = a.Horizontal * b, Vertical = a.Vertical * b };
            }

            public static Rectangle operator /(Rectangle a, float b)
            {
                return new Rectangle() { Center = a.Center, Horizontal = a.Horizontal / b, Vertical = a.Vertical / b };
            }

            public static bool operator ==(Rectangle a, Rectangle b)
            {
                return a.Center == b.Center && a.Horizontal == b.Horizontal && a.Vertical == b.Vertical;
            }

            public static bool operator !=(Rectangle a, Rectangle b)
            {
                return !(a == b);
            }
        }

    }
}