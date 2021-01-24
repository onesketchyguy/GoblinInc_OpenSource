using UnityEngine;
using UnityEngine.UI;
using LowEngine.Saving;

namespace LowEngine
{
    public class PlaceObjectMenu : MonoBehaviour
    {
        public Transform spawningParent;

        public GameObject itemPrefab;

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

        private void SpawnButton(SaveManager.SavableObject.WorldObject obj)
        {
            GameObject go = Instantiate(itemPrefab, spawningParent);
            go.name = $"{obj.name}.Button";

            var img = go.GetComponent<Image>();
            img.sprite = Modding.ModLoader.GetSprite(obj.spriteName);

            var button = go.GetComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(() => SetSpawningObject(obj));
        }

        private void SetSpawningObject(SaveManager.SavableObject.WorldObject obj)
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