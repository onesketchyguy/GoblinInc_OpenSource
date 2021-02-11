using UnityEngine;
using UnityEngine.EventSystems;

namespace LowEngine
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Vector3 MouseStart;
        private Vector3 MovedTo;

        private float baseMoveSpeed = 1;
        private float maxMoveSpeed = 10;
        private float moveSpeed;

        private float currentZoom;
        private float minZoom = 2;
        private float maxZoom = 20;

        private float zoomSpeed = 10;

        private Vector2 MaxCamPos;
        public Vector2 currentMaxCam;

        private Transform _transform;

        private void Start()
        {
            Vector3 stored = FindObjectOfType<MapLayoutManager>().PlayAreaSize / 2;

            maxZoom = stored.x * stored.y;

            if (maxZoom < 5) maxZoom = 5;

            currentZoom = 1;

            _transform = transform;

            MaxCamPos = _transform.position + stored;
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            float camCap = maxZoom - currentZoom;

            currentMaxCam = new Vector2(camCap + MaxCamPos.x, camCap + MaxCamPos.y);

            if (GameHandler.gameState == GameHandler.GameState.Default)
            {
                currentZoom = GetComponent<Camera>().orthographicSize;

                currentZoom += Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
                currentZoom += Input.GetAxisRaw("AlternateZoom") * (0.1f * zoomSpeed);

                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

                GetComponent<Camera>().orthographicSize = currentZoom;
            }

            var moving = Input.GetMouseButton(2) || (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);

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
                    var moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    MovedTo = _transform.position + (moveDirection * 10);

                    moveSpeed += 10 * Time.deltaTime;
                }

                MovedTo.z = -10;

                Mathf.Clamp(moveSpeed, baseMoveSpeed, maxMoveSpeed);
            }

            MovedTo.x = Mathf.Clamp(MovedTo.x, -currentMaxCam.x, currentMaxCam.x);
            MovedTo.y = Mathf.Clamp(MovedTo.y, -currentMaxCam.y, currentMaxCam.y);

            _transform.position = Vector3.MoveTowards(_transform.position, MovedTo, moveSpeed * Time.deltaTime);
        }
    }
}