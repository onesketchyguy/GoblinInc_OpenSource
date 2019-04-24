using UnityEngine;
using LowEngine.Saving;
using Unity.Entities;

namespace LowEngine
{
    public struct GhostData : IComponentData
    {
        public SaveManager.SavableObject.WorldObject placing;

        public PlacedObject[] overlapping;

        public bool okayToPlace;
    }
}