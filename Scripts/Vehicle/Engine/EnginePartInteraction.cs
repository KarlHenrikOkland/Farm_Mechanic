using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePartInteraction : MonoBehaviour
{
    public string acceptedPartID;
    public PartDatabase partDatabase;
    public Material ghostMaterial;

    private GameObject currentPart;
    private GameObject ghostPreview;
    public GameObject positionedPart;

    private void OnTriggerEnter(Collider other)
    {
        var idComponent = other.GetComponent<PartIdentifier>();
        if (idComponent != null && idComponent.partID == acceptedPartID)
        {
            currentPart = other.gameObject;

            if (ghostMaterial != null)
            {
                ghostPreview = Instantiate(currentPart, transform.position, transform.rotation);
                ApplyGhostMaterial(ghostPreview);
                
                // Copy the exact transform properties from positionedPart to ghostPreview
                if (positionedPart != null)
                {
                    ghostPreview.transform.position = positionedPart.transform.position;
                    ghostPreview.transform.rotation = positionedPart.transform.rotation;
                    ghostPreview.transform.localScale = positionedPart.transform.localScale;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentPart)
        {
            currentPart = null;

            if (ghostPreview != null)
            {
                Destroy(ghostPreview);
                ghostPreview = null;
            }
        }
    }

    private void Update()
    {
        if (currentPart == null) return;
        if (!Input.GetMouseButtonDown(0)) return;

        var partID = currentPart.GetComponent<PartIdentifier>()?.partID;
        if (partID != acceptedPartID) return;

        // Optional: validate compatibility (disabled if no attached part tracking)
        var dummyList = new System.Collections.Generic.List<string>(); // Empty for now
        if (!partDatabase.IsPartCompatible(partID, dummyList)) return;

        AttachPart(partID);
    }

    private void AttachPart(string partID)
    {
        // Snap position to slot (optional)
        currentPart.transform.position = transform.position;
        currentPart.transform.rotation = transform.rotation;

        // Clear from MouseGrab (which re-enables colliders)
        if (MouseGrab.Instance != null)
        {
            MouseGrab.Instance.ClearHeldObject();
        }

        // Manually destroy held object (since MouseGrab won't if we cleared it early)
        Destroy(currentPart);
        currentPart = null;

        if (ghostPreview != null)
        {
            Destroy(ghostPreview);
            ghostPreview = null;
        }

        

        positionedPart.SetActive(true);
        
        // Disable THIS object's collider to prevent further interactions
        var thisCollider = GetComponent<Collider>();
        if (thisCollider != null)
        {
            thisCollider.enabled = false;
        }
        
        Debug.Log($"Attached part: {partID}");
    }

    private void ApplyGhostMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.material = ghostMaterial;
        }

        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }

        foreach (var collider in obj.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }
}