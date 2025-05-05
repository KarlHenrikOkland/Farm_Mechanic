using UnityEngine;

public class MouseLookCamera : MonoBehaviour
{
    public Camera theCam;
    public float sensitivity = 1;
    public float smoothing = 2;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    public float turnSpeed = 4.0f;
    public float pickupDistance = 5.0f;
    public float zRotation = 0f;

    private float rotX;
    private float rotY;
    private Transform character;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseLook();
    }

    void LateUpdate()
    {
        // Reset Z rotation to 0 while keeping X and Y rotations
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            0
        );
    }

    void MouseLook()
    {
        // Skip camera control while rotating held object
        if (MouseGrab.IsRotatingObject)
            return;

        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;

        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        Vector3 camRot = theCam.transform.eulerAngles;
        camRot.x = -rotX;
        theCam.transform.eulerAngles = camRot;

        transform.Rotate(Vector3.up * y);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}