using UnityEngine;
using System.Collections.Generic;
using LowEngine.Navigation;
using LowEngine.Saving;

namespace LowEngine
{
    public class MapLayoutManager : MonoBehaviour
    {
        public static List<Node> ChosenNodes = new List<Node>();

        private static readonly SaveManager.SavableObject.WorldObject Concrete = new SaveManager.SavableObject.WorldObject { name = "Concrete", spriteName = "Concrete", spriteSortingLayer = -50, objectType = PlacedObjectType.Static, type = ObjectType.Abstract };

        /// <summary>
        /// Display half the size you want here.
        /// </summary>
        public Vector2 PlayAreaSize = new Vector2(20, 20);

        public float nodeRadius = 1;

        private GameObject ConcreteParent;

        private static Dictionary<Vector2, GameObject> gameTiles = new Dictionary<Vector2, GameObject>() { };

        public Node NodeFromWorldPosition(Vector3 target)
        {
            Vector2 gridSpace = new Vector2((PlayAreaSize.x * 2), (PlayAreaSize.y * 2));

            float percentX = (target.x + PlayAreaSize.x) / gridSpace.x;
            float percentY = (target.y + PlayAreaSize.y) / gridSpace.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSpace.x - 1) * percentX);
            int y = Mathf.RoundToInt((gridSpace.y - 1) * percentY);

            return grid[x, y];
        }

        public Node GetClosestNodeToWorldPosition(Vector3 target)
        {
            float nearestdist = nodeDiameter * nodeDiameter;
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

        public int GridMaxSize
        {
            get
            {
                return ((int)PlayAreaSize.x * 2) * ((int)PlayAreaSize.y * 2);
            }
        }

        private Node[,] grid;

        private float nodeDiameter
        { get { return nodeRadius * 2; } }
        private Vector2Int gridSize
        { get { return new Vector2Int(Mathf.RoundToInt((PlayAreaSize.x * 2) / nodeDiameter), Mathf.RoundToInt((PlayAreaSize.y * 2) / nodeDiameter)); } }

        // Start is called before the first frame update
        private void Start()
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

            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int xCheck = homeNode.gridPosition.x + x;
                    int yCheck = homeNode.gridPosition.y + y;

                    if (xCheck >= 0 && yCheck >= 0 && xCheck < gridSize.x && xCheck < gridSize.y)
                    {
                        Node neighbor = grid[xCheck, yCheck];

                        if (neighbor != null)
                            neighboringNodes.Add(neighbor);
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

            Vector2 worldBottomLeft = transform.position - new Vector3(PlayAreaSize.x, PlayAreaSize.y);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + new Vector2(x * nodeDiameter, y * nodeDiameter);
                    bool obstruction = false;

                    Collider2D collision = Physics2D.OverlapCircle(worldPoint, nodeRadius / 2);

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

        public static void ReplaceTileInDictionary(Vector2 pos, GameObject go)
        {
            RemoveFromDictionary(pos);

            gameTiles.Remove(pos);

            gameTiles.Add(pos, go);
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
            SaveManager.SavableObject.WorldObject objectData = Constructor.CloneObjectData(Concrete, tilePos, Quaternion.identity, Color.white);

            GameObject n_concrete = Constructor.GetObject(objectData, ConcreteParent.transform);

            ReplaceTileInDictionary(tilePos, n_concrete);

            if (Time.timeSinceLevelLoad > 1)
                UpdateGrid();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Vector2.zero, new Vector2(gridSize.x * 2, gridSize.y * 2) - (Vector2.one * nodeRadius));

            if (grid != null)
            {
                foreach (var node in grid)
                {
                    if (ChosenNodes.Count > 0)
                    {
                        if (ChosenNodes.Contains(node))
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(node.position, Vector2.one * nodeDiameter);

                            continue;
                        }
                    }

                    Gizmos.color = (node.obstrucion) ? Color.red : Color.white;

                    if (node.obstrucion)
                    {
                        Gizmos.DrawCube(node.position, Vector2.one * nodeDiameter);
                    }
                    else
                    {
                        Gizmos.DrawWireCube(node.position, Vector2.one * nodeDiameter);
                    }
                }
            }
        }
    }
}