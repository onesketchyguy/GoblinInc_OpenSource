using UnityEngine;
using LowEngine.Saving;
namespace LowEngine
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlacedObject : MonoBehaviour
    {
        public PlaceableObject obj;

        public SaveManager.SavableObject.WorldObject thisObject;

        private void LateUpdate()
        {
            if (thisObject == null)
            {
                thisObject = new SaveManager.SavableObject.WorldObject
                {
                    color = GetComponent<SpriteRenderer>().color,
                    sprite = GetComponent<SpriteRenderer>().sprite,
                    spriteSortingLayer = GetComponent<SpriteRenderer>().sortingOrder,
                    name = $"{gameObject.name}.{transform.position}",
                    position = transform.position,
                    rotation = transform.rotation
                };
            }
        }
    }
}