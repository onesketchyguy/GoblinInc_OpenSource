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
        public static List<Vector3> GetPoints(Vector3 StartPoint, Vector3 EndPoint)
        {
            Vector3Int start = new Vector3Int((int)StartPoint.x, (int)StartPoint.y, (int)StartPoint.z);
            Vector3Int end = new Vector3Int((int)EndPoint.x, (int)EndPoint.y, (int)EndPoint.z);

            List<Vector3> locations = new List<Vector3>();

            if (start.x > end.x)
            {
                for (int x = end.x; x < start.x + 1; x++)
                {
                    if (start.y > end.y)
                    {
                        for (int y = end.y; y < start.y + 1; y++)
                        {
                            Vector3 pos = new Vector3(x, y);

                            locations.Add(pos);
                        }
                    }
                    else
                    {
                        for (int y = start.y; y < end.y + 1; y++)
                        {
                            Vector3 pos = new Vector3(x, y);

                            locations.Add(pos);
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
                            Vector3 pos = new Vector3(x, y);

                            locations.Add(pos);
                        }
                    }
                    else
                    {
                        for (int y = start.y; y < end.y + 1; y++)
                        {
                            Vector3 pos = new Vector3(x, y);

                            locations.Add(pos);
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