using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LowEngine.Saving;

namespace LowEngine
{
    public class PlaceObjectMenu : MonoBehaviour
    {
        public Transform spawningParent;

        public Text Descriptiontext;

        public PlaceableObject[] placeableObjects;

        PlaceableObject Spawning;
        GhostObject ghostObject;

        [Range(0.1f, 0.9f)]
        public float ghostAlpha = 0.5f;

        Vector2 GridLockedMousePos => (Utilities.GridLockedMousePosition());

        public Sprite bulldozer;
        public static bool bullDozing;

        private Color PlacingColor;

        private GameObject worldObjectParent;

        private void Start()
        {
            foreach (var item in placeableObjects)
            {
                SpawnButton(item);
            }
        }

        void SpawnButton(PlaceableObject obj)
        {
            GameObject newObj = new GameObject($"{obj.name}");
            GameObject go = Instantiate(newObj, spawningParent);
            Destroy(newObj);

            go.name = $"{obj.name}.Button";
            Image img = go.AddComponent<Image>();

            img.sprite = obj.ObjectData.sprite;

            Button button = go.AddComponent<Button>();

            button.targetGraphic = img;

            button.onClick.AddListener(() => SetSpawningObject(obj));
        }

        private void Update()
        {
            if (Spawning != null)
            {
                Descriptiontext.text = $"{Spawning.name}\n${Spawning.ObjectData.value}";
            }
            else
            {
                Descriptiontext.text = "Select an object.";
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                CancelInvoke("SpawnObject");

                Cursor.visible = true;

                if (Input.GetButtonDown("Fire1"))
                {
                    if (Spawning != null)
                    {
                        ClearObject();
                    }
                }

                if (ghostObject != null)
                {
                    ghostObject.transform.position = Utilities.GetMousePosition();
                }
                
                return;
            }

            Cursor.visible = Spawning == null && bullDozing == false;

            PlacingColor = FindObjectOfType<ColorPicker>().SelectedColor;

            if (Spawning != null)
            {
                GameHandler.gameState = GameHandler.GameState.Placing;

                bool okayToPlace = (GameHandler.instance.Money >= Spawning.ObjectData.value);

                if (ghostObject == null)
                {
                    ghostObject = new GameObject($"{Spawning.name}.GhostObject").AddComponent<GhostObject>();

                    ghostObject.placing = Spawning;

                    ghostObject.enabled = false;

                    SpriteRenderer spr = ghostObject.gameObject.AddComponent<SpriteRenderer>();
                    spr.sortingOrder = 100;
                    Color c = (Spawning.ChangableColor) ? PlacingColor : Spawning.ObjectData.color;
                    spr.sprite = Spawning.ObjectData.sprite;
                    spr.color = new Color(c.r, c.g, c.b, ghostAlpha);

                    ghostObject.transform.position = GridLockedMousePos;
                }
                else
                {
                    if (ghostObject.okayToPlace == false) okayToPlace = false;
                    ghostObject.enabled = true;
                    ghostObject.transform.position = GridLockedMousePos;

                    SpriteRenderer spr = ghostObject.gameObject.GetComponent<SpriteRenderer>();
                    Color c = (Spawning.ChangableColor) ? PlacingColor : Spawning.ObjectData.color;

                    spr.sprite = Spawning.ObjectData.sprite;
                    spr.color = okayToPlace ? new Color(c.r, c.g, c.b, ghostAlpha) : new Color(1, 0, 0, ghostAlpha);
                }

                if (Input.GetAxisRaw("Mouse ScrollWheel") != 0 && Spawning.rotatable)
                {
                    int rounded = Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10);

                    Mathf.Clamp(rounded, -1, 1);

                    RotatePlacing(90 * rounded);
                }

                if (Input.GetButtonDown("Fire1") & okayToPlace)
                {
                    InvokeRepeating("SpawnObject", 0, Time.deltaTime * 8);
                }
                else 
                if (Input.GetButtonUp("Fire1"))
                {
                    CancelInvoke("SpawnObject");
                }

                if (Input.GetButton("Fire2"))
                {
                    ClearObject();
                }
            }

            if (bullDozing)
            {
                if (ghostObject == null)
                {
                    ghostObject = new GameObject($"bulldozer.GhostObject").AddComponent<GhostObject>();

                    SpriteRenderer spr = ghostObject.gameObject.AddComponent<SpriteRenderer>();
                    spr.sortingOrder = 100;
                    spr.sprite = bulldozer;
                    spr.color = new Color(1, 0, 0, ghostAlpha);
                }

                ghostObject.transform.position = GridLockedMousePos;

                if (ghostObject.overlapping != null)
                {
                    SpriteRenderer spr = ghostObject.gameObject.GetComponent<SpriteRenderer>();
                    spr.color = new Color(0, 0, 0, ghostAlpha);

                    if (ghostObject.overlapping[ghostObject.overlapping.Length - 1] != null && ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>() != null)
                    {
                        if (ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>().objectData.type != ObjectType.Abstract)
                        {
                            spr.color = new Color(1, 0, 0, ghostAlpha);

                            if (Input.GetButton("Fire1"))
                            {
                                foreach (var obj in ghostObject.overlapping)
                                {
                                    if (obj.GetComponent<PlacedObject>() == null || obj.GetComponent<PlacedObject>().objectData == null) continue;

                                    if (obj.GetComponent<PlacedObject>().objectData.value > 0)
                                    {
                                        GameHandler.instance.Money += obj.GetComponent<PlacedObject>().objectData.value;
                                    }
                                }

                                Destroy(ghostObject.overlapping[ghostObject.overlapping.Length - 1]);
                            }
                        }
                    }
                }
                if (Input.GetButton("Fire2")) ToggleBulldozer();
            }
        }

        void SetSpawningObject(PlaceableObject obj)
        {
            Spawning = obj;
        }

        private void SpawnObject()
        {
            if (Spawning == null || ghostObject == null || ghostObject.okayToPlace == false)
            {
                return;
            }

            if (GameHandler.instance.Money < Spawning.ObjectData.value)
            {
                NotificationManager.instance.ShowNotification($"You don't have enought money for that!\n${Spawning.ObjectData.value}");

                return;
            }


            AudioManager.instance.PlayBuildSound(ghostObject.transform.position);

            if (worldObjectParent == null)
            {
                worldObjectParent = new GameObject("Building");
            }

            Spawning.ObjectData.position = GridLockedMousePos;
            Spawning.ObjectData.rotation = ghostObject.transform.rotation;

            GameObject go = Constructor.GetObject(Spawning.ObjectData, worldObjectParent.transform);

            if (ghostObject.overlapping != null)
            {
                if (ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>().objectData.type == ObjectType.Abstract)
                {
                    Destroy(ghostObject.overlapping[ghostObject.overlapping.Length - 1]);

                    MapLayoutManager.ReplaceTileInDictionary(GridLockedMousePos, go);
                }
                    
            }


            if (Spawning.ChangableColor) go.GetComponent<SpriteRenderer>().color = PlacingColor;

            go.name = Spawning.ObjectData.name;

            GameHandler.instance.Money -= Spawning.ObjectData.value;

            if (Spawning.ObjectData.value > 0 && !Input.GetButton("Fire3"))
            {
                ClearObject();
            }
        }

        void RotatePlacing(float degrees)
        {
            ghostObject.transform.Rotate(Vector3.forward * degrees);
        }

        private void ClearObject()
        {
            Spawning = null;

            if (ghostObject != null)
                Destroy(ghostObject.gameObject);

            ghostObject = null;

            GameHandler.gameState = GameHandler.GameState.Default;
        }

        public void ToggleBulldozer()
        {
            if (bullDozing)
            {
                ClearObject();
            }

            bullDozing = !bullDozing;
        }
    }
}