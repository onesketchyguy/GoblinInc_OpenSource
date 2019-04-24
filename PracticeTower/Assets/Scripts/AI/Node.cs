using UnityEngine;

namespace LowEngine.Navigation
{
    public class Node : IHeapItem<Node>
    {

        public int HeapIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the distance between two nodes.
        /// </summary>
        /// <param name="node_a"></param>
        /// <param name="node_b"></param>
        /// <returns></returns>
        public static int GetManhattanDistance(Node node_a, Node node_b)
        {
            int distX = Mathf.Abs(node_a.gridPosition.x - node_b.gridPosition.x);
            int distY = Mathf.Abs(node_a.gridPosition.y - node_b.gridPosition.y);

            if (distX > distY)
            {
                return 14 * distY + 10*(distX - distY);
            }

            return 14 * distX + 10 * (distY - distX);
        }

        public int CompareTo(Node other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                //Int.Compare
                compare = fCost.CompareTo(other.hCost);
            }

            return -compare;
        }

        public Vector2Int gridPosition;

        public bool obstrucion;
        public Vector3 position;

        public Node parent;

        /// <summary>
        /// Distance from the starting node.
        /// </summary>
        public int gCost;
        /// <summary>
        /// Distance from the last node
        /// </summary>
        public int hCost;

        /// <summary>
        /// The total cost of movment.
        /// </summary>
        public int fCost { get { return gCost + hCost; } }

        public Node(Vector2 gridPosition, bool obstrucion, Vector3 position)
        {
            this.gridPosition = new Vector2Int((int)gridPosition.x, (int)gridPosition.y);
            this.obstrucion = obstrucion;
            this.position = position;
        }
    }
}