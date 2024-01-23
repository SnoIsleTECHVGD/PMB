using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool isActive = true;

    private Vector2 currentMouseLook;
    private Vector2 appliedMouseDelta;

    private Quaternion startPlayerRot;

    public float sensitivity = 1f;
    public float smoothing = 1.3f;

    public float minClamp, maxClamp;

    private Transform playerObj;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        playerObj = transform.root;
        startPlayerRot = playerObj.rotation;


    }

    void Update()
    {
        HandleEnable();
        if(isActive)
        {
            HandleMovement();
        }
    }


    void HandleEnable()
    {
        if (!isActive)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            return;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    void HandleMovement()
    {

        //Camera movement stuff
        Vector2 b = Vector2.Scale(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")), Vector2.one * this.sensitivity * this.smoothing);
        this.appliedMouseDelta = Vector2.Lerp(this.appliedMouseDelta, b, 1f / this.smoothing);
        this.currentMouseLook += this.appliedMouseDelta;
        //Clamped values (90 &-90f is standard fps)
        this.currentMouseLook.y = Mathf.Clamp(this.currentMouseLook.y, minClamp, maxClamp);

        base.transform.localRotation = Quaternion.AngleAxis(-this.currentMouseLook.y, Vector3.right);
        playerObj.localRotation = Quaternion.AngleAxis(currentMouseLook.x, Vector3.up) * startPlayerRot;
    }
}
