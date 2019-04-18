using UnityEngine;
using System.Collections.Generic;
using LowEngine.Navigation;
using LowEngine.Saving;

namespace LowEngine
{
    public class MapLayoutManager : MonoBehaviour
    {
        public PlaceableObject Concrete;

        /// <summary>
        /// Display half the size you want here.
        /// </summary>
        public Vector2 PlayAreaSize = new Vector2(20, 20);

        public float nodeRadius = 1;

        GameObject ConcreteParent;

        static Dictionary<Vector2, GameObject> gameTiles = new Dictionary<Vector2, GameObject>() { };

        public Node NodeFromWorldPosition(Vector3 target)
        {
            float nearestdist = nodeDiameter * 2;
            Node nearestToTarget = null;

            foreach (var node in grid)
            {
                if (Vector2.Distance(node.position, target) < nearestdist && node.obstrucion != true)
                {
                    nearestToTarget = node;
                    nearestdist = Vector2.Distance(node.position, target);
                }
            }

            return nearestToTarget;
        }

        Node[,] grid;

        float nodeDiameter { get { return nodeRadius * 2; } }
        Vector2Int gridSize { get { return new Vector2Int(Mathf.RoundToInt((PlayAreaSize.x * 2) / nodeDiameter), Mathf.RoundToInt((PlayAreaSize.y * 2) / nodeDiameter)); } }

        // Start is called before the first frame update
        void Start()
        {
            ConcreteParent = new GameObject("ConcreteParent");

            UpdateGrid();

            for (int x = (int)-PlayAreaSize.x; x < PlayAreaSize.x; x++)
            {
                for (int y = (int)-PlayAreaSize.y; y < PlayAreaSize.y; y++)
                {
                    Vector2 pos = transform.position + new Vector3(x, y);

                    ReplaceTile(pos);
                }
            }
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

                    gameTiles.TryGetValue(pos, out obj);

                    if (obj == null)
                    {
                        gameTiles.Remove(pos);

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
                    Vector3 worldPoint = new Vector2((x - (gridSize.x / 2)) * nodeDiameter, (y - (gridSize.y / 2)) * nodeDiameter);
                    bool obstruction = false;

                    Collider2D collision = Physics2D.OverlapCircle(worldPoint, nodeRadius/2);

                    if (collision != null)
                    {
                        PlacedObject placedObject = collision.GetComponent<PlacedObject>();

                        if (placedObject && placedObject.objectData != null)
                        {
                            if (placedObject.objectData.type == ObjectType.Wall)
                            {
                                obstruction = true;
                            }
                        }
                    }

                    grid[x, y] = new Node(new Vector2(x, y), obstruction, worldPoint);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector2.zero, new Vector2(gridSize.x * 2, gridSize.y * 2) - (Vector2.one * nodeRadius));

            if (grid != null)
            {
                foreach (var node in grid)
                {
                    Gizmos.color = (node.obstrucion) ? Color.red : Color.white;

                    Gizmos.DrawWireCube(node.position, Vector2.one * nodeDiameter);
                }
            }
        }

        public static void ReplaceTileInDictionary(Vector2 pos, GameObject go)
        {
            RemoveFromDictionary(pos);

            gameTiles.Remove(pos);

            gameTiles.Add(pos, go);

            if (go.name != "Concrete")
                Debug.Log($"Replaced tile at {pos} with {go.name}");
        }

        private static void RemoveFromDictionary(Vector2 pos)
        {
            GameObject toRemove;
            gameTiles.TryGetValue(pos, out toRemove);

            if (toRemove != null)
            {
                if (toRemove.GetComponent<PlacedObject>() != null)
                {
                    if (toRemove.GetComponent<PlacedObject>().objectData.objectType == PlacedObjectType.Static)
                    {
                        Destroy(toRemove);
                    }
                }
                else
                {
                    Destroy(toRemove);
                }
            }
        }

        public void ReplaceTile(Vector2 tilePos)
        {
            SaveManager.SavableObject.WorldObject objectData = Constructor.CloneObjectData(Concrete.ObjectData, tilePos, Quaternion.identity, Color.white);

            GameObject n_concrete = Constructor.GetObject(objectData, ConcreteParent.transform);

            n_concrete.GetComponent<SpriteRenderer>().material = GameHandler.instance.gameObjectMaterial;

            ReplaceTileInDictionary(tilePos, n_concrete);

            UpdateGrid();
        }
    }
}