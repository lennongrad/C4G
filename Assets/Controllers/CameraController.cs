using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cameraObject;
    public WorldController worldController; //debug only

    public float dragPanModifier = .02f;
    public float keyPanModifier = .2f;
    public float rotateModifier = .75f;
    public float zoomModifier = .01f;

    float goalScrollDistance;
    float goalScrollSpeed;

    Vector3 lastPosition;
    bool wasInWindowLast;

    Vector3 keyPan;
    bool holdingPan;
    bool holdingRotate;

    TileController tileHovered;

    // Start is called before the first frame update
    void Start()
    {
        goalScrollDistance = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        ClickDragUpdate();
        
        // not rotating with mouse, so go to nearest 90 degree angle (aligned with squares)
        if(!holdingRotate)
        {
            while (transform.rotation.y < 0)
            {
                transform.Rotate(new Vector3(0, 1f, 0f), 360, Space.World);
            }

            if (transform.eulerAngles.y % 90 >= 45)
            {
                transform.Rotate(new Vector3(0, 1f, 0f), 10, Space.World);
            }
            else if (transform.eulerAngles.y % 90 < 45 && transform.eulerAngles.y % 90 > 10)
            {
                transform.Rotate(new Vector3(0, 1f, 0f), -10, Space.World);
            }
            else if (transform.eulerAngles.y % 90 > 1)
            {
                transform.Rotate(new Vector3(0, 1f, 0f), -1, Space.World);
            }
        }

        // process scroll wheel differences
        transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(transform.position.y, goalScrollDistance, ref goalScrollSpeed, .15f), transform.position.z);

        // move camera with keyboard
        transform.Translate(keyPan * keyPanModifier, Space.Self);

        DetectHover();
    }

    void ClickDragUpdate()
    {
        // if mouse is outside window, dont need to update clicking and dragging functions of mouse
        if(GetMouseOutsideWindow())
        {
            wasInWindowLast = false;
            return;
        }

        // if mouse wasnt in the window last frame, we will have "garbage data" for last position, so just update and use next frame
        if(!wasInWindowLast)
        {
            wasInWindowLast = true;
            lastPosition = GetMousePosition();
            return;
        }

        // if we got this far, we are in the window
        wasInWindowLast = true;

        Vector3 diff = lastPosition - GetMousePosition();
        lastPosition = GetMousePosition();

        // drag the map under the camera with middle mouse button
        if (holdingPan)
        {
            transform.Translate(diff * dragPanModifier, Space.Self);
        }

        // rotate the map around camera with right mouse button
        if (holdingRotate)
        {
            transform.Rotate(new Vector3(0, 1f, 0f), diff.x * rotateModifier, Space.World);
        }

        // zoom up and down with scroll wheel
        goalScrollDistance += -Mouse.current.scroll.ReadValue().y * zoomModifier;

        lastPosition = GetMousePosition();
    }

    void DetectHover()
    {
        RaycastHit hit;
        Ray ray = cameraObject.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit))
        {
            tileHovered = hit.transform.parent.GetComponent<TileController>();
            tileHovered.Hover();
        }
        else
        {
            tileHovered = null;
        }
    }

    // used in update
    Vector3 GetMousePosition()
    {
        return new Vector3(Mathf.Clamp(Mouse.current.position.ReadValue().x, 60, Screen.width - 60) - 60, 0, Mathf.Clamp(Mouse.current.position.ReadValue().y, 60, Screen.height - 60) - 60);
    }

    bool GetMouseOutsideWindow()
    {

        return Mouse.current.position.ReadValue().x < 60 || Mouse.current.position.ReadValue().x > Screen.width - 60 || Mouse.current.position.ReadValue().y < 60 || Mouse.current.position.ReadValue().y > Screen.height - 60;
    }

    // called by input manager
    void OnSelect(InputValue value)
    {
        if(tileHovered != null)
        {
            worldController.Click(tileHovered);
        }
    }

    void OnPan(InputValue value)
    {
        keyPan = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
    }

    void OnPanDrag(InputValue value)
    {
        holdingPan = value.isPressed;
    }

    void OnRotateDrag(InputValue value)
    {
        holdingRotate = value.isPressed;
    }

    void OnRotateRight()
    {
        transform.Rotate(new Vector3(0, 1f, 0f), 50, Space.World);
    }

    void OnRotateLeft()
    {
        transform.Rotate(new Vector3(0, 1f, 0f), -50, Space.World);
    }
}
