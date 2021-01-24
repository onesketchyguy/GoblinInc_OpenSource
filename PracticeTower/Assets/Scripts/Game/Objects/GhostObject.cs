using UnityEngine;
using LowEngine.Saving;

namespace LowEngine
{
    public class GhostObject : MonoBehaviour
    {
        public GhostData ghostData;

        private SpriteRenderer spriteRenderer;

        private void OnEnable()
        {
            if (ghostData.placing == null)
            {
                ghostData.okayToPlace = false;

                return;
            }

            ghostData.okayToPlace = (ghostData.placing.type == ObjectType.Ground || ghostData.placing.type == ObjectType.Wall);

            spriteRenderer = GetComponent<SpriteRenderer>();

            CheckForCollisions();
        }

        public void CheckForCollisions()
        {
            //------------------------THIS WHOLE SYSTEM SHOULD BE SWAPPED OUT
            //------------------------WE HAVE LITERALLY NO REASON NOT TO JUST
            //------------------------CREATE OUR OWN COLLISION SYSTEM, INSTEAD OF
            //------------------------USING ACTUAL PHYSICS. THIS IS SLOW AND
            //------------------------UNDER-PERFORMANT AND SHOULD BE SHUNNED
            if (ghostData.placing != null)
            {
                ghostData.okayToPlace = (ghostData.placing.type == ObjectType.Ground || ghostData.placing.type == ObjectType.Wall);
            }

            Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, 0.25f);

            // If no collisions occure then something is terrible and we need to rectify it
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

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            spriteRenderer.sprite = ObjectPlacingManager.ghostSprite;
            spriteRenderer.color = ghostData.okayToPlace ? new Color(c.r, c.g, c.b, ghostAlpha) : new Color(1, 0, 0, ghostAlpha);
        }
    }
}