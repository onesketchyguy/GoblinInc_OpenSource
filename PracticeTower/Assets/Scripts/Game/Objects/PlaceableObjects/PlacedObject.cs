using UnityEngine;
using UnityEditor;
using LowEngine.Saving;
namespace LowEngine
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlacedObject : MonoBehaviour
    {
        public PlaceableObject obj;

        public SaveManager.SavableObject.WorldObject thisObject;

        public void UpdateObject()
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

        private void LateUpdate()
        {
            if (thisObject == null) UpdateObject();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlacedObject))]
    public class PlacedObjectCusomtEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PlacedObject obj = (PlacedObject)target;

            obj.UpdateObject();

            DrawDefaultInspector();
        }
    }
#endif
}