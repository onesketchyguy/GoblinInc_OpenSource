using UnityEngine;
namespace LowEngine
{
    [CreateAssetMenu(fileName = "New Object", menuName = "Placeable Object")]
    public class PlaceableObject : ScriptableObject
    {
        public float itemCost = 10;

        public GameObject prefab;

        public ObjectType type;

        public bool ChangableColor;
        public bool rotatable;
    }

    public enum ObjectType { Abstract, Table, Ground, Wall }
}