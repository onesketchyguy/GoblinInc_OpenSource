using LowEngine.Saving;

namespace LowEngine
{
    public struct GhostData
    {
        public SaveManager.SavableObject.WorldObject placing;

        public PlacedObject[] overlapping;

        public bool okayToPlace;
    }
}