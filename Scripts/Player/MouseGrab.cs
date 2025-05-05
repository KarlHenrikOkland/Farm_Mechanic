using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGrab : MonoBehaviour
{
    [Header("Object Pickup Settings")]
    public float pickupRange = 3f;
    public float holdDistance = 2f;
    public float zoomSpeed = 1f;
    public float minHoldDistance = 0.5f;
    public float maxHoldDistance = 5f;
    public float rotationSpeed = 100f;
    public Transform holdPoint;
    public LayerMask pickupLayer;

    [Header("Camera Reference")]
    public Camera playerCamera; // Assign in Inspector or auto-find

    private Rigidbody heldObject;
    private Collider[] heldColliders;
    private Transform cam;
    public static MouseGrab Instance { get; private set; }
    public static bool IsRotatingObject { get; private set; }

    void Start()
    {
        Instance = this;
        cam = playerCamera.transform;
    }
     void Update()
    {
        HandleObjectInteraction();
    }
    
    void HandleObjectInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
                TryPickupObject();
            else
                DropObject();
        }

        if (heldObject != null)
        {
            ScrollZoom();
            HoldObject();
            RotateHeldObject();
        }
        else
        {
            IsRotatingObject = false;
        }
    }

    void TryPickupObject()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    heldObject = rb;
                    heldColliders = heldObject.GetComponentsInChildren<Collider>();

                    foreach (var col in heldColliders)
                    {
                        if (!col.isTrigger)
                            col.enabled = col == heldObject.GetComponent<Collider>() ? true : false;
                    }
                }
            }
        }
    }

    void HoldObject()
    {
        Vector3 targetPos = cam.position + cam.forward * holdDistance;
        Vector3 direction = targetPos - heldObject.position;
        heldObject.velocity = direction * 10f;
    }

    void DropObject()
    {
        if (heldColliders != null)
        {
            foreach (var col in heldColliders)
                col.enabled = true;
        }

        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject.freezeRotation = false;
            heldObject.drag = 0f;
            heldObject = null;
            heldColliders = null;
        }

        IsRotatingObject = false;
    }

    void ScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            holdDistance += scroll * zoomSpeed;
            holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);
        }
    }

    void RotateHeldObject()
    {
        if (Input.GetMouseButton(1))
        {
            IsRotatingObject = true;
            float horizontal = Input.GetAxis("Mouse X");
            if (horizontal != 0f)
            {
                heldObject.transform.Rotate(Vector3.up, -horizontal * rotationSpeed * Time.deltaTime, Space.World);
            }
        }
        else
        {
            IsRotatingObject = false;
        }
    }

    public void ClearHeldObject()
    {
        if (heldColliders != null)
        {
            foreach (var col in heldColliders)
                col.enabled = true;
        }

        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject.freezeRotation = false;
            heldObject.drag = 0f;
            heldObject = null;
            heldColliders = null;
        }

        IsRotatingObject = false;
    }
    
}