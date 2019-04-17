using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LowEngine.TimeManagement
{
    public class DayDisplay : MonoBehaviour
    {
        public Text PayDayDisplay;
        public Text Title;
        public Text Day;

        public Image checkBox;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DayDisplay))]
    public class DayDisplayCustomtEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DayDisplay obj = (DayDisplay)target;

            foreach (var textComp in obj.GetComponentsInChildren<Text>())
            {
                if (textComp.name.ToLower().Contains("title")) obj.Title = textComp;
                if (textComp.name.ToLower().Contains("day")) obj.Day = textComp;
                if (textComp.name.ToLower().Contains("pay")) obj.PayDayDisplay = textComp;
            }

            foreach (var item in obj.GetComponentsInChildren<Image>())
            {
                if (item.name.ToLower().Contains("check")) obj.checkBox = item;
            }

            DrawDefaultInspector();
        }
    }
#endif
}