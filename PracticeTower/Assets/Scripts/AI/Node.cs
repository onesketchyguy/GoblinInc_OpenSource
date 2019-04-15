using UnityEngine;

namespace LowEngine.Navigation
{
    public class Node
    {
        public static int GetManhattanDistance(Node node_a, Node node_b)
        {
            int iX = Mathf.Abs(node_a.gridPosition.x - node_b.gridPosition.x);
            int iY = Mathf.Abs(node_a.gridPosition.y - node_b.gridPosition.y);

            return iX + iY;
        }

        public Vector2Int gridPosition;

        public bool IsWall;
        public Vector3 position;

        public Node parent;

        /// <summary>
        /// Distance from the starting node.
        /// </summary>
        public int CostFromStart;
        /// <summary>
        /// Distance from the last node
        /// </summary>
        public int CostFromEnd;
        public int Cost { get { return CostFromStart + CostFromEnd; } } // The total cost of movment

        public Node(Vector2 gridPosition, bool isWall, Vector3 position)
        {
            this.gridPosition = new Vector2Int((int)gridPosition.x, (int)gridPosition.y);
            IsWall = isWall;
            this.position = position;
        }
    }
}