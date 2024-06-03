using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace LowEngine.Navigation
{
    public class PathFinding
    {
        public List<Node> Path = new List<Node>();

        public Stopwatch stopwatch;

        public void FindPath(Vector3 start, Vector3 end, MapLayoutManager grid)
        {
            stopwatch = new Stopwatch();

            stopwatch.Start();

            grid.UpdateGrid();

            Node StartNode = grid.NodeFromWorldPosition(start);
            Node TargetNode = grid.NodeFromWorldPosition(end);

            if (TargetNode == null || TargetNode == StartNode)
            {
                TargetNode = grid.GetClosestNodeToWorldPosition(end);
            }

            if (TargetNode.obstrucion)
            {
                float DistToTarget = Vector2.Distance(StartNode.position, TargetNode.position);

                foreach (var node in grid.GetNeightboringNodes(TargetNode))
                {
                    if (node.obstrucion) continue;

                    float DistToNode = Vector2.Distance(node.position, TargetNode.position);

                    if (DistToNode < DistToTarget)
                    {
                        DistToTarget = DistToNode;

                        TargetNode = node;
                    }
                }
            }

            Heap<Node> OpenList = new Heap<Node>(grid.GridMaxSize);
            HashSet<Node> ClosedList = new HashSet<Node>();

            OpenList.Add(StartNode);

            while (OpenList.Count > 0)
            {
                Node currentNode = OpenList.RemoveFirst();

                ClosedList.Add(currentNode);

                if (currentNode == TargetNode)
                {
                    stopwatch.Stop();

                    UnityEngine.Debug.Log($"Stopped search at: {stopwatch.ElapsedMilliseconds}");

                    GetFinalPath(StartNode, TargetNode);
                    return;
                }

                foreach (var neighborNode in grid.GetNeightboringNodes(currentNode))
                {
                    if (neighborNode.obstrucion || ClosedList.Contains(neighborNode)) continue;

                    int MoveCostToNeighbor = currentNode.gCost + Node.GetManhattanDistance(currentNode, neighborNode);

                    if (MoveCostToNeighbor < neighborNode.gCost || !OpenList.Contains(neighborNode))
                    {
                        neighborNode.gCost = MoveCostToNeighbor;
                        neighborNode.hCost = Node.GetManhattanDistance(neighborNode, TargetNode);
                        neighborNode.parent = currentNode;

                        if (!OpenList.Contains(neighborNode))
                        {
                            OpenList.Add(neighborNode);
                        }
                    }
                }
            }
        }

        private void GetFinalPath(Node startNode, Node targetNode)
        {
            List<Node> finalPath = new List<Node>();
            Node CurrentNode = targetNode;

            while (CurrentNode != startNode)
            {
                finalPath.Add(CurrentNode);

                if (CurrentNode.parent == null)
                {
                    UnityEngine.Debug.Log("No parent");

                    break;
                }

                CurrentNode = CurrentNode.parent;
            }

            finalPath.Reverse();

            Path = finalPath;

            if (Path == null || Path.Count < 1)
            {
                UnityEngine.Debug.Log("Unable to create path!");
            }
        }
    }
}