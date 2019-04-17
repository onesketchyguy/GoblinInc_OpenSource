using UnityEngine;
namespace LowEngine
{
    [CreateAssetMenu(fileName = "New Object", menuName = "Placeable Object")]
    public class PlaceableObject : ScriptableObject
    {
        public bool ChangableColor;
        public bool rotatable;

        public Saving.SaveManager.SavableObject.WorldObject ObjectData;
    }

    public enum ObjectType { Abstract, Table, Ground, Wall }
}