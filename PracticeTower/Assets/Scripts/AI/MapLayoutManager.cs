using UnityEngine;
using System.Collections.Generic;
using LowEngine.Navigation;
using LowEngine.Saving;

namespace LowEngine
{
    public class MapLayoutManager : MonoBehaviour
    {
        public GameObject Concrete;

        /// <summary>
        /// Display half the size you want here.
        /// </summary>
        public Vector2 PlayAreaSize = new Vector2(20, 20);

        public float nodeRadius = 1;
        public float Distance;

        GameObject ConcreteParent;

        static Dictionary<Vector2, GameObject> concreteTiles = new Dictionary<Vector2, GameObject>() { };

        public Node NodeFromWorldPosition(Vector3 target)
        {
            //      float xPoint = Mathf.Clamp01((target.x + PlayAreaSize.x) / PlayAreaSize.x);
            //        float yPoint = Mathf.Clamp01((target.y + PlayAreaSize.y) / PlayAreaSize.y);

            //            int x = Mathf.RoundToInt((gridSize.x - 1) * xPoint);
            //          int y = Mathf.RoundToInt((gridSize.y - 1) * yPoint);

            //            return grid[x, y];

            float nearestdist = 2;
            Node nearestToTarget = null;

            foreach (var node in grid)
            {
                if (Vector2.Distance(node.position, target) < nearestdist && node.IsWall != true)
                {
                    nearestToTarget = node;
                    nearestdist = Vector2.Distance(node.position, target);
                }
            }

            return nearestToTarget;
        }

        Node[,] grid;

        float nodeDiameter;
        Vector2Int gridSize;

        // Start is called before the first frame update
        void Start()
        {
            nodeDiameter = nodeRadius * 2;

            gridSize.x = Mathf.RoundToInt((PlayAreaSize.x * 2) / nodeDiameter);
            gridSize.y = Mathf.RoundToInt((PlayAreaSize.y * 2) / nodeDiameter);

            ConcreteParent = new GameObject("ConcreteParent");

            for (int x = (int)-PlayAreaSize.x; x < PlayAreaSize.x; x++)
            {
                for (int y = (int)-PlayAreaSize.y; y < PlayAreaSize.y; y++)
                {
                    Vector2 pos = transform.position + new Vector3(x, y);

                    ReplaceTile(pos);
                }
            }

            UpdateGrid();
        }

        public List<Node> GetNeightboringNodes(Node homeNode)
        {
            List<Node> neighboringNodes = new List<Node>();

            int xCheck;
            int yCheck;

            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    xCheck = homeNode.gridPosition.x + x;
                    yCheck = homeNode.gridPosition.y + y;

                    if (xCheck >= 0 && xCheck < gridSize.x)
                    {
                        if (yCheck >= 0 && yCheck < gridSize.y)
                        {
                            Node neighbor = grid[xCheck, yCheck];

                            if (neighbor != null)
                                neighboringNodes.Add(neighbor);
                        }
                    }
                }
            }

            return neighboringNodes;
        }

        private void LateUpdate()
        {
            for (int x = (int)-PlayAreaSize.x; x < PlayAreaSize.x; x++)
            {
                for (int y = (int)-PlayAreaSize.y; y < PlayAreaSize.y; y++)
                {
                    Vector2 pos = new Vector2(x, y);

                    GameObject obj;

                    concreteTiles.TryGetValue(pos, out obj);

                    if (obj == null)
                    {
                        concreteTiles.Remove(pos);

                        ReplaceTile(pos);
                    }
                }
            }
        }

        public void UpdateGrid()
        {
            grid = new Node[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    float padding = (nodeDiameter + Distance);

                    Vector3 worldPoint = Vector2.one + new Vector2(((x - 1) - (gridSize.x / 2)) * padding, ((y - 1) - gridSize.y / 2) * padding);
                    bool wall = false;

                    Collider2D collision = Physics2D.OverlapCircle(worldPoint, nodeRadius/2);

                    if (collision != null && collision.GetComponent<PlacedObject>() && collision.GetComponent<PlacedObject>().obj)
                    {
                        if (collision.GetComponent<PlacedObject>().obj.type == ObjectType.Wall)
                        {
                            wall = true;
                        }
                    }

                    grid[x, y] = new Node(new Vector2(x, y), wall, worldPoint);
                }
            }

            Vector3 topRight = transform.position + Vector3.right * (PlayAreaSize.x / 2) + Vector3.forward * (PlayAreaSize.y / 2);

        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(PlayAreaSize.x * 2, PlayAreaSize.y * 2));

            if (grid != null)
            {
                foreach (var node in grid)
                {
                    Gizmos.color = (node.IsWall) ? Color.red : Color.white;

                    Gizmos.DrawWireCube(node.position, Vector2.one * (nodeDiameter - Distance));
                }
            }
        }

        public static void ReplaceTileInDictionary(Vector2 pos, GameObject go)
        {
            concreteTiles.Remove(pos);

            concreteTiles.Add(pos, go);
        }

        public void ReplaceTile(Vector2 tilePos)
        {
            GameObject n_concrete = Instantiate(Concrete, tilePos, Quaternion.identity, ConcreteParent.transform);

            n_concrete.GetComponent<SpriteRenderer>().material = GameHandler.instance.gameObjectMaterial;

            ReplaceTileInDictionary(tilePos, n_concrete);

            UpdateGrid();
        }
    }
}