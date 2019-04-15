using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LowEngine
{
    public class PlaceObjectMenu : MonoBehaviour
    {
        public Transform spawningParent;

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

            img.sprite = obj.prefab.GetComponent<SpriteRenderer>().sprite;

            Button button = go.AddComponent<Button>();

            button.targetGraphic = img;

            button.onClick.AddListener(() => SetSpawningObject(obj));
        }

        private void Update()
        {
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

                bool okayToPlace = (GameHandler.instance.Money >= Spawning.itemCost);

                if (ghostObject == null)
                {
                    ghostObject = new GameObject($"{Spawning.name}.GhostObject").AddComponent<GhostObject>();

                    ghostObject.placing = Spawning;

                    ghostObject.enabled = false;

                    SpriteRenderer spr = ghostObject.gameObject.AddComponent<SpriteRenderer>();
                    spr.sortingOrder = 100;
                    Color c = (Spawning.ChangableColor) ? PlacingColor : Spawning.prefab.GetComponent<SpriteRenderer>().color;
                    spr.sprite = Spawning.prefab.GetComponent<SpriteRenderer>().sprite;
                    spr.color = new Color(c.r, c.g, c.b, ghostAlpha);

                    ghostObject.transform.position = GridLockedMousePos;
                }
                else
                {
                    if (ghostObject.okayToPlace == false) okayToPlace = false;
                    ghostObject.enabled = true;
                    ghostObject.transform.position = GridLockedMousePos;

                    SpriteRenderer spr = ghostObject.gameObject.GetComponent<SpriteRenderer>();
                    Color c = (Spawning.ChangableColor)? PlacingColor : Spawning.prefab.GetComponent<SpriteRenderer>().color;

                    spr.sprite = Spawning.prefab.GetComponent<SpriteRenderer>().sprite;
                    spr.color = okayToPlace ? new Color(c.r, c.g, c.b, ghostAlpha) : new Color(1, 0, 0, ghostAlpha);
                }

                if (Input.GetAxisRaw("Mouse ScrollWheel") != 0 && Spawning.rotatable)
                {
                    if (Input.GetButton("Fire3"))
                    {
                        RotatePlacing(90 * Input.GetAxisRaw("Mouse ScrollWheel"));
                    }
                    else
                    {
                        int rounded = Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10);

                        Mathf.Clamp(rounded, -1, 1);

                        RotatePlacing(90 * rounded);
                    }
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

                    ghostObject.transform.position = GridLockedMousePos;
                }

                if (ghostObject.overlapping != null)
                {
                    SpriteRenderer spr = ghostObject.gameObject.GetComponent<SpriteRenderer>();
                    spr.color = (ghostObject.overlapping[ghostObject.overlapping.Length - 1] != null && ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>().obj.type != ObjectType.Abstract) ? new Color(1, 0, 0, ghostAlpha) : new Color(0, 0, 0, ghostAlpha);

                    ghostObject.transform.position = GridLockedMousePos;


                    if (ghostObject.overlapping.Length > 0 && ghostObject.overlapping[ghostObject.overlapping.Length - 1] != null)
                    {
                        if (ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>().obj.type != ObjectType.Abstract)
                        {
                            if (Input.GetButton("Fire1"))
                            {
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
            if (GameHandler.instance.Money >= obj.itemCost)
            {
                Spawning = obj;
            }
            else
            {
                NotificationManager.instance.ShowNotification($"You don't have enought money for that!\n${obj.itemCost}");
            }
        }

        private void SpawnObject()
        {
            if (Spawning == null || ghostObject == null || ghostObject.okayToPlace == false)
            {
                return;
            }

            if (GameHandler.instance.Money < Spawning.itemCost)
            {
                NotificationManager.instance.ShowNotification($"You don't have enought money for that!\n${Spawning.itemCost}");

                return;
            }


            AudioManager.instance.PlayBuildSound(ghostObject.transform.position);

            if (worldObjectParent == null)
            {
                worldObjectParent = new GameObject("Building");
            }

            GameObject go = Instantiate(Spawning.prefab, GridLockedMousePos, ghostObject.transform.rotation, worldObjectParent.transform);

            if (ghostObject.overlapping != null)
            {
                if (ghostObject.overlapping[ghostObject.overlapping.Length - 1].GetComponent<PlacedObject>().obj.type == ObjectType.Abstract)
                {
                    Destroy(ghostObject.overlapping[ghostObject.overlapping.Length - 1]);

                    MapLayoutManager.ReplaceTileInDictionary(GridLockedMousePos, go);
                }
                    
            }


            if (Spawning.ChangableColor) go.GetComponent<SpriteRenderer>().color = PlacingColor;

            go.name = Spawning.prefab.name;

            GameHandler.instance.Money -= Spawning.itemCost;

            if (Spawning.itemCost > 0 && !Input.GetButton("Fire3"))
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