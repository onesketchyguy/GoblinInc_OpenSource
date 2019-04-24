﻿using LowEngine.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Entities;

namespace LowEngine
{
    public enum ObjectType { Abstract, Table, Ground, Wall }

    public class ObjectPlacingManager : MonoBehaviour
    {
        public static SaveManager.SavableObject.WorldObject Spawning;
        GhostObject ghostObject;
        List<GameObject> Ghosts = new List<GameObject>();
        [Range(0.1f, 0.9f)]
        public float ghostAlpha = 0.5f;

        public static Sprite ghostSprite;

        Vector2 MousePos => (Utilities.GridLockedMousePosition());

        public Texture2D bulldozer;
        public static bool bullDozing;

        public static Color PlacingColor;

        ColorPicker colorPicker
        {
            get
            {
                return FindObjectOfType<ColorPicker>();
            }
        }

        private GameObject worldObjectParent;

        private Vector3 start = Vector3.forward;
        private Vector2 lastPos;
        Vector3[] positions = null;
        bool positionExists(Vector3 pos)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (pos == positions[i]) return true;
            }

            return false;
        }

        private void Update()
        {
            //Handle UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //When user mouse is over the UI we want to show the cursor.
                Cursor.visible = true;

                //If the user requests a new item, clear the existing one.
                if (Spawning != null || bullDozing == true)
                {
                    //Continue following the cursor so that the ghost doesn't just sit there.
                    if (ghostObject != null)
                    {
                        CursorManager cursorManager = CursorManager.instance;

                        Texture2D texture = Modding.ModLoader.GetTexture(Spawning.spriteName);

                        if (cursorManager != null && texture != null)
                            cursorManager.UpdateCursor(texture);

                        ghostObject.transform.position = MousePos;
                    }

                    if (Input.GetButtonDown("Fire1"))
                        ClearObject();
                }

                //Stop reading.
                return;
            }

            //If we are working, do not show the cursor.
            //Cursor.visible = Spawning == null && bullDozing == false;

            if (colorPicker != null) PlacingColor = colorPicker.SelectedColor;

            if (Spawning != null)
            {
                CursorManager.instance.UpdateCursor();

                HandlePlacing();
            }

            if (bullDozing)
            {
                HandleBulldozing();
            }
        }

        void HandlePlacing()
        {
            GameHandler.gameState = GameHandler.GameState.Placing;

            bool okayToPlace = (GameHandler.instance.Money >= Spawning.pVal);

            if (ghostSprite == null)
                ghostSprite = Modding.ModLoader.GetSprite(Spawning.spriteName);

            if (ghostObject == null)
            {
                CreateGhost(Spawning.name, Spawning);

                SpriteRenderer spr = ghostObject.gameObject.AddComponent<SpriteRenderer>();

                if (spr.sprite == null)
                {
                    spr.sprite = ghostSprite;
                }
            }
            else
            {
                if (ghostObject.ghostData.okayToPlace == false) okayToPlace = false;
                ghostObject.enabled = true;
            }

            if (lastPos != MousePos)
            {
                ghostObject.transform.position = MousePos;

                lastPos = MousePos;

                ghostObject.CheckForCollisions();

                if (start != Vector3.forward)
                {
                    ghostObject.gameObject.SetActive(false);

                    positions = PointGenerator.GetPoints(start, MousePos).ToArray();

                    foreach (var position in positions)
                    {
                        CreateGhost(position, Spawning.name, Spawning, ghostSprite);
                    }
                }
                else
                {
                    ghostObject.gameObject.SetActive(true);

                    positions = new Vector3[] { MousePos };
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0 && Spawning.rotatable)
            {
                int rounded = Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10);

                Mathf.Clamp(rounded, -1, 1);

                RotatePlacing(90 * rounded);
            }

            if (Input.GetButtonDown("Fire1") && okayToPlace)
            {
                start = MousePos;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (positions != null)
                {
                    if  (Ghosts.Count > 0)
                    {
                        foreach (var ghost in Ghosts)
                        {
                            if (ghost != null)
                            {
                                GhostObject obj = ghost.GetComponent<GhostObject>();

                                obj.CheckForCollisions();

                                if (obj.ghostData.okayToPlace)
                                {
                                    SpawnObject(ghost.transform.position);
                                }
                            }
                        }

                        foreach (var item in Ghosts)
                        {
                            Destroy(item);
                        }

                        Ghosts = new List<GameObject>();
                    }
                    else
                    {
                        if (ghostObject.ghostData.okayToPlace)
                        {
                            SpawnObject(ghostObject.transform.position);
                        }
                    }

                    if (Spawning.pVal > 0 && !Input.GetButton("Fire3"))
                    {
                        ClearObject();
                    }
                }
                else
                {
                    Debug.Log("Positions array empty.");
                }

                start = Vector3.forward;
            }
        }

        void HandleBulldozing()
        {
            if (ghostObject == null)
            {
                Cursor.SetCursor(bulldozer, MousePos, CursorMode.Auto);

                CreateGhost();
            }

            ghostObject.enabled = true;
            ghostObject.transform.position = MousePos;

            if (lastPos != MousePos)
            {
                ghostObject.transform.position = MousePos;

                lastPos = MousePos;

                ghostObject.CheckForCollisions();
            }

            if (ghostObject.ghostData.overlapping != null)
            {
               // spr.color = new Color(0, 0, 0, ghostAlpha);

                if (ghostObject.ghostData.overlapping[ghostObject.ghostData.overlapping.Length - 1] != null && ghostObject.ghostData.overlapping[ghostObject.ghostData.overlapping.Length - 1].GetComponent<PlacedObject>() != null)
                {
                    if (ghostObject.ghostData.overlapping[ghostObject.ghostData.overlapping.Length - 1].GetComponent<PlacedObject>().objectData.type != ObjectType.Abstract)
                    {
                     //   spr.color = new Color(1, 0, 0, ghostAlpha);

                        if (Input.GetButton("Fire1"))
                        {
                            foreach (var obj in ghostObject.ghostData.overlapping)
                            {
                                if (obj.GetComponent<PlacedObject>() == null || obj.GetComponent<PlacedObject>().objectData == null) continue;

                                if (obj.GetComponent<PlacedObject>().objectData.pVal > 0)
                                {
                                    GameHandler.instance.Money += obj.GetComponent<PlacedObject>().objectData.pVal;
                                }
                            }

                            Destroy(ghostObject.ghostData.overlapping[ghostObject.ghostData.overlapping.Length - 1].gameObject);
                        }
                    }
                }
            }
        }

        private void CreateGhost(Vector3 position, string objectName, SaveManager.SavableObject.WorldObject placing, Sprite sprite)
        {
            UpdateGhostCount();

            void CreateObject()
            {
                Debug.Log($"Placing ghost at {position}");

                var Ghost = new GameObject($"{objectName}.GhostObject").AddComponent<GhostObject>();
                SpriteRenderer spr = Ghost.gameObject.AddComponent<SpriteRenderer>();

                spr.sprite = sprite;
                spr.color = Color.white - new Color(0, 0, 0, ghostAlpha);
            
                Ghost.ghostData.placing = placing;
                spr.sortingOrder = 100;
                Ghost.transform.position = position;

                Ghosts.Add(Ghost.gameObject);

            }

            if (Ghosts.Count > 0)
            {
                for (int i = 0; i < Ghosts.Count; i++)
                {
                    GameObject item = Ghosts[i];

                    if (item == null) continue;

                    if (positionExists(Ghosts[i].transform.position) == false)
                    {
                        Destroy(Ghosts[i].gameObject);
                    }

                    if (item.transform.position == position)
                    {
                        item.GetComponent<GhostObject>().CheckForCollisions();

                        return;
                    }
                    else
                    {
                        CreateObject();
                        return;
                    }
                }
            }
            else
            {
                CreateObject();
            }
        }

        void UpdateGhostCount()
        {
            foreach (var item in FindObjectsOfType<GhostObject>())
            {
                if (Ghosts.Contains(item.gameObject) == false)
                {
                    Ghosts.Add(item.gameObject);
                }
            }

            while (Ghosts.Count > positions.Length)
            {
                for (int i = 0; i < Ghosts.Count; i++)
                {
                    GameObject item = Ghosts[i];

                    if (item == null)
                    {
                        Ghosts.Remove(item);

                        return;
                    }

                    if (positionExists(item.transform.position) == false)
                    {
                        Destroy(item);

                        Ghosts.Remove(item);

                        return;
                    }
                }

                if (Ghosts.Count > positions.Length)
                {
                    Invoke("UpdateGhostCount", 0.1f);
                    break;
                }
            }
        }

        private void CreateGhost(string objectName = "BullDozer", SaveManager.SavableObject.WorldObject placing = null)
        {
            if (ghostObject != null)
            {
                //destroy the last ghostObject
                Destroy(ghostObject);
            }

            ghostObject = new GameObject($"{objectName}.GhostObject").AddComponent<GhostObject>();
            ghostObject.ghostData.placing = placing;
            ghostObject.enabled = false;
        }

        void SetSpawningObject(SaveManager.SavableObject.WorldObject obj)
        {
            ClearObject();

            Spawning = obj;
        }

        private void SpawnObject(Vector3 spawnPoint)
        {
            if (Spawning == null || ghostObject == null || ghostObject.ghostData.okayToPlace == false)
            {
                return;
            }

            Debug.Log($"Spawning {Spawning.name} at {spawnPoint}");

            if (GameHandler.instance.Money < Spawning.pVal)
            {
                NotificationManager.instance.ShowNotification($"You don't have enought money for that!\n${Spawning.pVal}");

                return;
            }


            AudioManager.instance.PlayBuildSound(ghostObject.transform.position);

            if (worldObjectParent == null)
            {
                worldObjectParent = GameObject.Find("Building");

                if (worldObjectParent == null)
                    worldObjectParent = new GameObject("Building");
            }

            Color newColor = (Spawning.ChangableColor) ? PlacingColor : Spawning.color;

            GameObject spawnedObject = Constructor.GetObject(Constructor.CloneObjectData(Spawning, spawnPoint, ghostObject.transform.rotation, newColor), worldObjectParent.transform);

            if (ghostObject.ghostData.overlapping != null)
            {
                if (ghostObject.ghostData.overlapping[ghostObject.ghostData.overlapping.Length - 1].objectData.type == ObjectType.Abstract)
                {
                    MapLayoutManager.ReplaceTileInDictionary(spawnPoint, spawnedObject);
                }
            }

            GameHandler.instance.Money -= Spawning.pVal;

            Debug.Log($"Spawned {Spawning.name} at {spawnPoint}");
        }

        void RotatePlacing(float degrees)
        {
            ghostObject.transform.Rotate(Vector3.forward * degrees);
        }

        public void ClearObject()
        {
            Spawning = null;

            bullDozing = false;

            if (ghostObject != null)
                Destroy(ghostObject.gameObject);

            ghostObject = null;
            ghostSprite = null;

            GameHandler.gameState = GameHandler.GameState.Default;

            CursorManager.instance.UpdateCursor();
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