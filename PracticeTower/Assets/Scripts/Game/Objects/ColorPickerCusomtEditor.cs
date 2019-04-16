using UnityEngine;
using UnityEditor;
namespace LowEngine
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ColorPicker))]
    public class ColorPickerCusomtEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ColorPicker picker = (ColorPicker)target;

            DrawDefaultInspector();

            if (picker.colorsPallete != null)
            {
                picker.textureSize = new Vector2(picker.colorsPallete.width, picker.colorsPallete.height);
            }
        }
    }
#endif
}