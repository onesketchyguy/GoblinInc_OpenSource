using UnityEngine;

namespace LowEngine
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Vector3 MouseStart;
        private Vector3 MovedTo;

        float baseMoveSpeed = 1;
        float maxMoveSpeed = 10;
        float moveSpeed;

        private float currentZoom;
        float minZoom = 2;
        float maxZoom = 20;

        float zoomSpeed = 10;

        bool moving;

        private Vector2 MaxCam;
        public Vector2 currentMaxCam;

        private void Start()
        {
            Vector3 stored = FindObjectOfType<MapLayoutManager>().PlayAreaSize;

            MaxCam = transform.position + stored;
        }

        void Update()
        {
            float camCap = maxZoom - currentZoom;

            currentMaxCam = new Vector2(camCap + MaxCam.x, camCap + MaxCam.y);

            if (GameHandler.gameState == GameHandler.GameState.Default)
            {
                currentZoom = GetComponent<Camera>().orthographicSize;

                currentZoom += Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;

                currentZoom += Input.GetAxisRaw("AlternateZoom") * (0.1f * zoomSpeed);

                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                GetComponent<Camera>().orthographicSize = currentZoom;
            }

            moving = Input.GetMouseButton(2) || (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

            if (moving == false)
            {
                MouseStart = Utilities.GetMousePosition();

                moveSpeed = Mathf.MoveTowards(moveSpeed, 0, 50 * Time.deltaTime);
            }
            else
            {
                if (Input.GetMouseButton(2))
                {
                    moveSpeed = Vector3.Distance(MouseStart, Utilities.GetMousePosition()) * baseMoveSpeed;

                    MovedTo = Utilities.GetMousePosition();
                }
                else
                {
                    MovedTo = transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                    moveSpeed += 3 * Time.deltaTime;
                }

                MovedTo.z = -10;

                Mathf.Clamp(moveSpeed, baseMoveSpeed, maxMoveSpeed);
            }

            MovedTo.x = Mathf.Clamp(MovedTo.x, -currentMaxCam.x, currentMaxCam.x);
            MovedTo.y = Mathf.Clamp(MovedTo.y, -currentMaxCam.y, currentMaxCam.y);

            transform.position = Vector3.MoveTowards(transform.position, MovedTo, moveSpeed * Time.deltaTime);
        }
    }
}