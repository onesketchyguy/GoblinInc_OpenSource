using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowEngine
{
    public static class Utilities
    {
        public static Vector3 GetMousePosition()
        {
            Vector3 moPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            return moPos;
        }

        public static Vector3 ToViewportSpace(Vector3 worldpoint)
        {
            return Camera.main.WorldToViewportPoint(new Vector3(worldpoint.x, worldpoint.y, Camera.main.nearClipPlane));
        }

        public static Vector3 ToScreenSpace(Vector3 worldpoint)
        {
            return Camera.main.WorldToScreenPoint(new Vector3(worldpoint.x, worldpoint.y, Camera.main.nearClipPlane));
        }

        public static Vector2 GridLockedMousePosition()
        {
            Vector2 moPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            moPos.x = Mathf.Round(moPos.x);
            moPos.y = Mathf.Round(moPos.y);

            return moPos;
        }

        public static Color GetRandomColor()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            return new Color(r, g, b, 1);
        }

        public static T[] Add<T>(T toAdd, T[] array)
        {
            var old = array;
            array = new T[old.Length + 1];

            for (int i = 0; i < old.Length; i++)
            {
                array[i] = old[i];
            }

            array[old.Length] = toAdd;
            return array;
        }
    }
}