using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float lerpness = 8f;
    public float mouseBoundsSpeed = 1f;
    public float mouseBoundsSize = 6f;
    public float minAngle = 45f;
    public float maxAngle = 75f;
    public float minZoom = 5f;
    public float maxZoom = 30f;
    public Transform cameraRoot;
    public KeyCode dragKey = KeyCode.Mouse2;

    private float zoomLevel = 0f;
    private Vector3 position = Vector2.zero;
    private Vector3 lastMousePosition;
    private Vector3 dragVelocity;
    private float angle;
    private static CameraManager instance;

    public static bool Dragging { get; private set; }

    public static float Rotation { get; set; }

    private float ZoomMultiplier
    {
        get
        {
            return ((zoomLevel + minZoom) / (maxZoom + minZoom));
        }
    }


    private float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    private void Awake()
    {
        zoomLevel = (minZoom + maxZoom) / 2f;
        instance = this;
    }

    private void OnEnable()
    {
        instance = this;
    }

    public static void Focus()
    {
        if (!instance) instance = FindObjectOfType<CameraManager>();
        Focus(Config.Temporary.selectedTile);
    }

    public static void Focus(TileObject tile)
    {
        if (!instance) instance = FindObjectOfType<CameraManager>();

        if (tile)
        {
            instance.position = tile.transform.position;
        }
    }

    private void Update()
    {
        if (CanvasManager.Layer == "InGame")
        {
            zoomLevel -= Input.GetAxis("Mouse ScrollWheel") * 12f;
            zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
            angle = Remap(zoomLevel, minZoom, maxZoom, minAngle, maxAngle);

            if (Input.GetKeyDown(dragKey))
            {
                lastMousePosition = Input.mousePosition;
            }

            if(Input.GetKey(KeyCode.F))
            {
                Focus();
            }

            DragCamera();
            if (!Input.GetKey(dragKey)) CheckScreenBounds();
        }
        
        cameraRoot.position = new Vector3(position.x, 0f, position.z);
        cameraRoot.eulerAngles = new Vector3(0f, Rotation, 0f);

        Vector3 angleVector = new Vector3(0f, Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad));
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, angleVector * zoomLevel, Time.deltaTime * lerpness);
        Camera.main.transform.localEulerAngles = Vector3.Lerp(Camera.main.transform.localEulerAngles, new Vector3(angle, 0f, 0f), Time.deltaTime * lerpness);
    }

    private void LateUpdate()
    {
        if(Input.GetKeyUp(dragKey))
        {
            Dragging = false;
        }
    }

    private void DragCamera()
    {
        if (Input.GetKey(dragKey))
        {
            Vector3 newVelocity = lastMousePosition - Input.mousePosition;
            newVelocity *= 0.05f * ZoomMultiplier;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                dragVelocity = Vector3.zero;
                Rotation += Input.GetAxis("Mouse X") * 3f;
            }
            else
            {
                dragVelocity = newVelocity;
            }

            if (dragVelocity.magnitude > 0.01f)
            {
                Dragging = true;
            }
        }
        else
        {
            dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, Time.deltaTime * lerpness);
        }

        position += cameraRoot.TransformVector(new Vector3(dragVelocity.x, 0, dragVelocity.y));
        lastMousePosition = Input.mousePosition;
    }

    private void CheckScreenBounds()
    {
        Vector3 moveDirection = Vector3.zero;
        if (Input.mousePosition.x < mouseBoundsSize && Input.mousePosition.x > 0)
        {
            moveDirection -= cameraRoot.right;
        }
        if (Input.mousePosition.x > Screen.width - mouseBoundsSize && Input.mousePosition.x < Screen.width)
        {
            moveDirection += cameraRoot.right;
        }
        if (Input.mousePosition.y < mouseBoundsSize && Input.mousePosition.y > 0)
        {
            moveDirection -= cameraRoot.forward;
        }
        if (Input.mousePosition.y > Screen.height - mouseBoundsSize && Input.mousePosition.y < Screen.height)
        {
            moveDirection += cameraRoot.forward;
        }

        position += moveDirection.normalized * Time.deltaTime * mouseBoundsSpeed * 10f * ZoomMultiplier;
    }
}
