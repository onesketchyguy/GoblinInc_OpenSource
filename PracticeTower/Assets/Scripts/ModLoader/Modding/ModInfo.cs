using System;
using UnityEngine;
using LowEngine.Saving;
using LowEngine.Tasks.Needs;

namespace LowEngine.Modding
{
    /// <summary>
    /// The base mod info.  Intended to be deserialized from the mod info file.
    /// </summary>
    public class ModInfo
    {
        /// <summary>
        /// The name of the mod.
        /// </summary>
        public string name;

        /// <summary>
        /// An icon for this mod.
        /// </summary>
        public string spriteName;
        public int spriteSortingLayer;

        public Color color;

        public float price;
        public float wVal;

        public ObjectType type;
        public PlacedObjectType objectType;

        public NeedDefinition fulfilsNeed;
        public Vector3 childPos;

        public bool rotatable;
        public bool changableColor;

        // public string GetInfo()
        // {
        //     return $"${name}, wVal{wVal}, price{price}, oType{type}, poType{objectType}, fulfills{fulfilsNeed}";
        // }

        public ModInfo(string name, string spriteLocation, int spriteSortingLayer, Color color, float price, float wVal, ObjectType type, PlacedObjectType objectType, NeedDefinition fulfilsNeed, Vector3 childPos, bool rotatable, bool changableColor)
        {
            this.name = name;
            this.spriteName = spriteLocation;
            this.spriteSortingLayer = spriteSortingLayer;
            this.color = color;
            this.price = price;
            this.wVal = wVal;
            this.type = type;
            this.objectType = objectType;
            this.fulfilsNeed = fulfilsNeed;
            this.childPos = childPos;
            this.rotatable = rotatable;
            this.changableColor = changableColor;
        }
    }
}