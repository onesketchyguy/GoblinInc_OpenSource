using UnityEngine;

namespace LowEngine
{
    public class GhostObject : MonoBehaviour
    {
        public PlaceableObject placing;

        public GameObject[] overlapping;

        public bool okayToPlace = true;

        private void OnEnable()
        {
            if (placing == null)
            {
                okayToPlace = false;

                return;
            }

            okayToPlace = (placing.type == ObjectType.Ground || placing.type == ObjectType.Wall);
        }

        private void FixedUpdate()
        {
            if (placing != null)
            {
                okayToPlace = (placing.type == ObjectType.Ground || placing.type == ObjectType.Wall);
            }

            Collider2D[] collisions = null;

            collisions = Physics2D.OverlapCircleAll(transform.position, 0.4f);

            if (collisions == null || collisions.Length == 0)
            {
                okayToPlace = false;

                return;
            }

            overlapping = new GameObject[collisions.Length];

            okayToPlace = true;

            for (int i = 0; i < collisions.Length; i++)
            {
                PlacedObject PlacedObject = collisions[i].gameObject.GetComponent<PlacedObject>();

                if (PlacedObject)
                {
                    overlapping[i] = collisions[i].gameObject;

                    if (placing != null)
                    {
                        if (i > 0)
                        {
                            if (okayToPlace)
                            {
                                okayToPlace = NothingBlocking(PlacedObject);
                            }
                        }
                        else
                        {
                            okayToPlace = NothingBlocking(PlacedObject);
                        }
                    }
                    else
                    {
                        okayToPlace = false;
                    }
                }
            }

        }

        bool NothingBlocking(PlacedObject placedObject)
        {
            if (placing == null || placedObject.obj.type == placing.type)
            {
                return false;
            }

            switch (placedObject.obj.type)
            {
                case ObjectType.Abstract:
                    return (placing.type == ObjectType.Ground || placing.type == ObjectType.Wall);
                case ObjectType.Table:
                    return (placing.type == ObjectType.Ground);
                case ObjectType.Ground:
                    return (placing.type == ObjectType.Table);
                case ObjectType.Wall:
                    return false;
                default:
                    return false;
            }
        }
    }
}