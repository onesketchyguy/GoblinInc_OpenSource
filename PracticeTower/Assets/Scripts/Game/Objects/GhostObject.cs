using UnityEngine;
using Unity.Entities;
using LowEngine.Saving;

namespace LowEngine
{
    public class GhostObject : MonoBehaviour
    {
        public GhostData ghostData;

        SpriteRenderer spr;

        private void OnEnable()
        {
            if (ghostData.placing == null)
            {
                ghostData.okayToPlace = false;

                return;
            }

            ghostData.okayToPlace = (ghostData.placing.type == ObjectType.Ground || ghostData.placing.type == ObjectType.Wall);

            spr = GetComponent<SpriteRenderer>();

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

            if (ghostData.placing != null)
                UpdateColor();
        }

        private void UpdateColor()
        {
            float ghostAlpha = ObjectPlacingManager.Reference.ghostAlpha;

            Color c = (ghostData.placing.ChangableColor) ? ObjectPlacingManager.PlacingColor : ghostData.placing.color;

            if (spr == null) spr = GetComponent<SpriteRenderer>();

            spr.sprite = ObjectPlacingManager.ghostSprite;
            spr.color = ghostData.okayToPlace ? new Color(c.r, c.g, c.b, ghostAlpha) : new Color(1, 0, 0, ghostAlpha);
        }
    }
}