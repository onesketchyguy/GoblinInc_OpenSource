using System.Collections.Generic;
using UnityEngine;

namespace LowEngine
{
    public static class PointGenerator
    {
        /// <summary>
        /// Returns the positions between two points.
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        public static HashSet<Vector2> GetPoints(Vector2 StartPoint, Vector2 EndPoint)
        {
            Vector2Int start = new Vector2Int((int)StartPoint.x, (int)StartPoint.y);
            Vector2Int end = new Vector2Int((int)EndPoint.x, (int)EndPoint.y);

            HashSet<Vector2> locations = new HashSet<Vector2>();
            void AddPos(Vector2 po)
            {
                if (locations.Contains(po) == false) locations.Add(po);
            }

            if (start.x > end.x)
            {
                for (int x = end.x; x < start.x + 1; x++)
                {
                    if (start.y > end.y)
                    {
                        for (int y = end.y; y < start.y + 1; y++)
                        {
                            AddPos(new Vector2(x, y));
                        }
                    }
                    else
                    {
                        for (int y = start.y; y < end.y + 1; y++)
                        {
                            AddPos(new Vector2(x, y));
                        }
                    }
                }
            }
            else
            {
                for (int x = start.x; x < end.x + 1; x++)
                {
                    if (start.y > end.y)
                    {
                        for (int y = end.y; y < start.y + 1; y++)
                        {
                            AddPos(new Vector2(x, y));
                        }
                    }
                    else
                    {
                        for (int y = start.y; y < end.y + 1; y++)
                        {
                            AddPos(new Vector2(x, y));
                        }
                    }
                }
            }

            if (!locations.Contains(start)) locations.Add(start);
            if (!locations.Contains(end)) locations.Add(end);


            return locations;
        }
    }
}