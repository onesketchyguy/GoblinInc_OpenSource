using UnityEngine;

namespace LowEngine.Saving
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlacedObject : MonoBehaviour
    {
        public SaveManager.SavableObject.WorldObject objectData;
    }
}