using UnityEngine;
using UnityEngine.UI;
using LowEngine.Saving;

namespace LowEngine
{
    public class PlaceObjectMenu : MonoBehaviour
    {
        public Transform spawningParent;

        public Text Descriptiontext;

        private void Start()
        {
            Modding.ModLoader.LoadMods();

            if (Modding.ModLoader.objectMods != null)
            {
                foreach (var item in Modding.ModLoader.objectMods)
                {
                    SpawnButton(item);
                }
            }

            ClearObject();
        }

        void SpawnButton(SaveManager.SavableObject.WorldObject obj)
        {
            GameObject newObj = new GameObject($"{obj.name}");
            GameObject go = Instantiate(newObj, spawningParent);
            Destroy(newObj);

            go.name = $"{obj.name}.Button";
            Image img = go.AddComponent<Image>();

            img.sprite = Modding.ModLoader.GetSprite(obj.spriteName);

            Button button = go.AddComponent<Button>();

            button.targetGraphic = img;

            button.onClick.AddListener(() => SetSpawningObject(obj));
        }

        void SetSpawningObject(SaveManager.SavableObject.WorldObject obj)
        {
            ClearObject();

            ObjectPlacingManager.Spawning = obj;

            CursorManager.instance.UpdateCursor(Modding.ModLoader.GetTexture(ObjectPlacingManager.Spawning.spriteName));

            Descriptiontext.text = $"{obj.name}\n${obj.pVal}";
        }

        public void ClearObject()
        {
            FindObjectOfType<ObjectPlacingManager>().ClearObject();

            Descriptiontext.text = "Select an object.";
        }
    }
}