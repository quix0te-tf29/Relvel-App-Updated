using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script which allows the user to zoom in and out, and rotate the camera around a given object
/// </summary>
public class CameraRotateAroundObject : MonoBehaviour
{
    /// <summary>
    /// The Object around which the camera will rotate
    /// </summary>

    public GameObject[] cameraTargetableObjects;
    public Dropdown CameraController;
    private Transform cameraAnchorPoint;
    private float cameraSpeed = 1f;
    private float zoomSpeed = 10f;
    private Vector3 localRotation = new Vector3();
    private Vector3 LastRotatePosition;
    private float cameraDistance = 50f;
    private float pinchAmount;

    void Start()
    {
        cameraAnchorPoint = this.transform.parent;
        SetCameraTarget(0);
    }

    public void CameraControllerValueChanged()
    {
        SetCameraTarget(CameraController.value);
    }

    public void SetCameraTarget(int index)
    {
        cameraAnchorPoint.transform.position = cameraTargetableObjects[index].transform.position;
        cameraAnchorPoint.transform.parent = cameraTargetableObjects[index].transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(cameraAnchorPoint.transform);
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch t1 = Input.GetTouch(0);
            if (t1.deltaPosition.x != 0 || t1.deltaPosition.y != 0)
            {
                RotateCamera(t1.deltaPosition.x, t1.deltaPosition.y);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 touchOne = t1.position;
            Vector2 touchTwo = t2.position;

            float deltaPinch;

            if (t2.phase == TouchPhase.Began)
            {
                Debug.Log("TOUCH HAS BEGUN");
                deltaPinch = Vector2.Distance(touchOne, touchTwo);
                pinchAmount = deltaPinch;
            }
            
            //get the current distance beteen touch inputs
            deltaPinch = Vector2.Distance(touchOne, touchTwo);
            while (deltaPinch != pinchAmount)
            {
                float zoomFactor = (pinchAmount - deltaPinch) * Time.deltaTime;
                ZoomCamera(zoomFactor * -1, 1);
                pinchAmount = Vector2.Distance(touchOne, touchTwo);
            }

            if (t1.phase == TouchPhase.Ended || t2.phase == TouchPhase.Ended)
            {
                Debug.Log("TOUCH HAS BEEN RELEASED");
                //pinchAmount = 0;
            }
        }

    }

    void HandleMouse()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                RotateCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
        }
        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, 20);
    }


    void RotateCamera(float inputX, float inputY)
    {
        localRotation.x += inputX * cameraSpeed;
        localRotation.y += inputY * cameraSpeed;

        if (localRotation.y < 0f)
        {
            localRotation.y = 0f;
        }
        else if (localRotation.y > 90f)
        {
            localRotation.y = 90f;
        }

        Quaternion QT = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        cameraAnchorPoint.rotation = Quaternion.Lerp(cameraAnchorPoint.rotation, QT, Time.deltaTime * 10f);
    }

    void ZoomCamera(float scroll, float speed)
    {
        float scrollAmount = scroll * zoomSpeed;
        scrollAmount *= (cameraDistance * 0.3f);
        cameraDistance += scrollAmount * -1f;
        cameraDistance = Mathf.Clamp(cameraDistance, 50f, 3500f);

        if (Camera.main.transform.localPosition.z != cameraDistance * -1f)
        {
            Camera.main.transform.localPosition = new Vector3(0f, 0f, Mathf.Lerp(Camera.main.transform.localPosition.z, cameraDistance * -1f, Time.deltaTime * speed));
        }
    }

}
