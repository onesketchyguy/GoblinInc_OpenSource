using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using LowEngine.Saving;

namespace LowEngine.Modding
{
    /// <summary>
    /// Provides methods for loading the mods.
    /// </summary>
    public static class ModLoader
    {
        public static readonly string ModsDirectory = $"{Application.dataPath}/LocalData/Mods";
        public static readonly string ModImagesDirectory = $"{Application.dataPath}/LocalData/Mods/images";

        public static Dictionary<string, Sprite> LoadedSprites = new Dictionary<string, Sprite>();
        public static Dictionary<string, Texture2D> LoadedTextures = new Dictionary<string, Texture2D>();

        public static List<SaveManager.SavableObject.WorldObject> objectMods;

        public static void LoadMods()
        {
            objectMods = new List<SaveManager.SavableObject.WorldObject>();

            if (Directory.Exists(ModsDirectory))
            {
                LoadInImages();

                foreach (var file in Directory.EnumerateFiles(ModsDirectory, "*.txt"))
                {
                    string contents = File.ReadAllText(file);

                    var modInfo = JsonUtility.FromJson<ModInfo>(contents);

                    if (modInfo != null)
                    {
                        objectMods.Add(GetObjectData(modInfo));

                        Debug.Log($"Loaded {modInfo.name}");
                    }
                }
            }
        }

        public static void LoadInImages()
        {
            if (LoadedSprites.Count > 0) return;

            foreach (var file in Directory.EnumerateFiles(ModImagesDirectory, "*.PNG"))
            {
                var sprite = GenerateSprite(file);
            }
        }

        private static SaveManager.SavableObject.WorldObject GetObjectData(ModInfo data)
        {
            SaveManager.SavableObject.WorldObject r = new SaveManager.SavableObject.WorldObject { };

            r = new SaveManager.SavableObject.WorldObject
            {
                name = data.name,
                pVal = data.price,
                wVal = data.wValue,
                type = data.type,
                objectType = data.objectType,
                fulFills = data.fulfilsNeed,
                childPos = data.childPos,
                spriteSortingLayer = data.spriteSortingLayer,
                color = data.color,
                spriteName = data.spriteName,
                rotatable = data.rotatable,
                ChangableColor = data.changableColor
            };


            return r;
        }

        public static void GenerateMod(Saving.SaveManager.SavableObject.WorldObject ModData)
        {
            if (Directory.Exists(ModsDirectory) == false)
            {
                Directory.CreateDirectory(ModsDirectory);
            }

            ModInfo mod = new ModInfo(ModData.name, $"{ModData.name}", ModData.spriteSortingLayer, ModData.color, ModData.pVal, ModData.wVal, ModData.type, ModData.objectType, ModData.fulFills, ModData.childPos, ModData.rotatable, ModData.ChangableColor);

            string fileInfo = JsonUtility.ToJson(mod);

            File.WriteAllText(ModsDirectory + $"/{mod.name}.txt", fileInfo);
        }

        public static Sprite GetSprite(string spriteName)
        {
            LoadInImages();

            Sprite loadedSprite;

            LoadedSprites.TryGetValue(spriteName, out loadedSprite);

            string success = loadedSprite != null ? $"Successfully retrieved sprite: {spriteName}" : $"Failed to retrieve sprite: {spriteName}";
            Debug.Log(success);

            return loadedSprite;
        }

        public static Texture2D GetTexture(string textureName)
        {
            LoadInImages();

            Texture2D loadedTexture;

            LoadedTextures.TryGetValue(textureName, out loadedTexture);

            string success = loadedTexture != null ? $"Successfully retrieved texture: {textureName}" : $"Failed to retrieve texture: {textureName}";
            Debug.Log(success);

            return loadedTexture;
        }

        private static Sprite GenerateSprite(string path)
        {
            byte[] SpriteData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false, false);
            texture.LoadImage(SpriteData);
            texture.filterMode = FilterMode.Point;
            texture.name = Path.GetFileNameWithoutExtension(path);

            string name = Path.GetFileNameWithoutExtension(path);


            if (texture != null)
            {
                LoadedTextures.Add(name, texture);

                Sprite spr = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, 32, 0, SpriteMeshType.FullRect);

                if (spr != null)
                    LoadedSprites.Add(name, spr);

                return spr;
            }

            return null;
        }
    }
}