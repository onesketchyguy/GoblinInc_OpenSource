using UnityEngine;

namespace LowEngine
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager instance
        {
            get
            {
                return FindObjectOfType<CursorManager>();
            }
        }

        private void Start()
        {
            UpdateCursor();
        }

        public Texture2D DefaultCursor;

        public void UpdateCursor(Texture2D image = null)
        {
            if (image == null)
            {
                Cursor.SetCursor(DefaultCursor, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(image, Vector2.zero, CursorMode.Auto);
            }
        }
    }
}