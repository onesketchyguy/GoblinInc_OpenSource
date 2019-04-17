using UnityEngine;
using System.Collections.Generic;

namespace LowEngine.Navigation
{
    public class PathFinding
    {
        public List<Node> Path = new List<Node>();

        public void FindPath(Vector3 start, Vector3 end, MapLayoutManager grid)
        {
            Node StartNode = grid.NodeFromWorldPosition(start);
            Node TargetNode = grid.NodeFromWorldPosition(end);

            if (TargetNode == StartNode)
            {
                TargetNode = grid.GetNeightboringNodes(StartNode)[0];
            }

            if (TargetNode.IsWall)
            {
                float DistToTarget = Vector2.Distance(StartNode.position, TargetNode.position);

                foreach (var node in grid.GetNeightboringNodes(TargetNode))
                {
                    if (node.IsWall) continue;

                    float DistToNode = Vector2.Distance(node.position, TargetNode.position);

                    if (DistToNode < DistToTarget)
                    {
                        DistToTarget = DistToNode;

                        TargetNode = node;
                    }
                }
            }

            List<Node> OpenList = new List<Node>() { StartNode };
            HashSet<Node> ClosedList = new HashSet<Node>();

            while (OpenList.Count > 0)
            {
                Node currentNode = OpenList[0];

                for (int i = 1; i < OpenList.Count; i++)
                {
                    if ((OpenList[i].Cost < currentNode.Cost || OpenList[i].Cost == currentNode.Cost) && (OpenList[i].CostFromEnd < currentNode.CostFromEnd || OpenList[i].CostFromEnd == currentNode.CostFromEnd))
                    {
                        currentNode = OpenList[i];
                    }
                }

                OpenList.Remove(currentNode);
                ClosedList.Add(currentNode);

                if (currentNode == TargetNode)
                {
                    GetFinalPath(StartNode, TargetNode);
                }

                foreach (var neighborNode in grid.GetNeightboringNodes(currentNode))
                {
                    if (neighborNode == null || neighborNode.IsWall || ClosedList.Contains(neighborNode)) continue;

                    int MoveCost = currentNode.CostFromStart + Node.GetManhattanDistance(currentNode, neighborNode);

                    if (MoveCost <= neighborNode.CostFromStart || !OpenList.Contains(neighborNode))
                    {
                        neighborNode.CostFromStart = MoveCost;
                        neighborNode.CostFromEnd = Node.GetManhattanDistance(neighborNode, TargetNode);
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
                    Debug.Log("No parent");

                    break;
                }

                CurrentNode = CurrentNode.parent;
            }

            finalPath.Reverse();

            Path = finalPath;
        }
    }
}