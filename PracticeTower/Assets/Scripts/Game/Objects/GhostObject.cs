using UnityEngine;
using Unity.Entities;
using LowEngine.Saving;

namespace LowEngine
{
    public class GhostObject : MonoBehaviour
    {
        public GhostData ghostData;

        private void OnEnable()
        {
            if (ghostData.placing == null)
            {
                ghostData.okayToPlace = false;

                return;
            }

            ghostData.okayToPlace = (ghostData.placing.type == ObjectType.Ground || ghostData.placing.type == ObjectType.Wall);

            CheckForCollisions();
        }

        public void CheckForCollisions()
        {
            if (ghostData.placing != null)
            {
                ghostData.okayToPlace = (ghostData.placing.type == ObjectType.Ground || ghostData.placing.type == ObjectType.Wall);
            }

            Collider2D[] collisions = null;

            collisions = Physics2D.OverlapCircleAll(transform.position, 0.25f);

            if (collisions == null || collisions.Length == 0)
            {
                ghostData.okayToPlace = false;

                return;
            }

            ghostData.overlapping = new PlacedObject[collisions.Length];

            ghostData.okayToPlace = true;

            for (int i = 0; i < collisions.Length; i++)
            {
                PlacedObject PlacedObject = collisions[i].gameObject.GetComponent<PlacedObject>();

                if (PlacedObject)
                {
                    ghostData.overlapping[i] = collisions[i].gameObject.GetComponent<PlacedObject>();

                    if (ghostData.placing != null)
                    {
                        if (i > 0)
                        {
                            if (ghostData.okayToPlace)
                            {
                                ghostData.okayToPlace = Constructor.NothingBlocking(ghostData.placing, PlacedObject.objectData);
                            }
                        }
                        else
                        {
                            ghostData.okayToPlace = Constructor.NothingBlocking(ghostData.placing, PlacedObject.objectData);
                        }
                    }
                    else
                    {
                        ghostData.okayToPlace = false;
                    }
                }
            }

            SpriteRenderer spr = GetComponent<SpriteRenderer>();

            if (spr)
            {
                float ghostAlpha = FindObjectOfType<ObjectPlacingManager>().ghostAlpha;

                Color c = (ObjectPlacingManager.Spawning.ChangableColor) ? ObjectPlacingManager.PlacingColor : ObjectPlacingManager.Spawning.color;

                spr.sprite = ObjectPlacingManager.ghostSprite;
                spr.color = ghostData.okayToPlace ? new Color(c.r, c.g, c.b, ghostAlpha) : new Color(1, 0, 0, ghostAlpha);
            }
        }
    }
}