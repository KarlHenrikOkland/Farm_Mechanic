using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyManager : MonoBehaviour
{
    [SerializeField] private PartDatabase partDatabase;
    private List<string> attachedPartIDs = new List<string>();

    public bool TryAttachPart(string partID)
    {
        if (partDatabase == null)
        {
            Debug.LogError("No part database assigned!");
            return false;
        }

        if (attachedPartIDs.Contains(partID))
        {
            Debug.Log("Part already attached");
            return false;
        }

        if (!partDatabase.IsPartCompatible(partID, attachedPartIDs))
        {
            return false;
        }

        var part = partDatabase.GetPart(partID);
        if (part == null || part.prefab == null)
        {
            Debug.LogError("Invalid part or missing prefab");
            return false;
        }

        // Instantiate and attach the part
        var newPart = Instantiate(part.prefab, transform);
        newPart.name = part.partName;
        
        // Position the part appropriately (you might want to customize this)
        newPart.transform.localPosition = Vector3.zero;
        newPart.transform.localRotation = Quaternion.identity;

        attachedPartIDs.Add(partID);
        Debug.Log($"Successfully attached {part.partName}");
        return true;
    }

    public bool DetachPart(string partID)
    {
        if (!attachedPartIDs.Contains(partID))
        {
            Debug.Log("Part not attached");
            return false;
        }

        // Find the part in children and destroy it
        foreach (Transform child in transform)
        {
            var part = child.GetComponent<EnginePartComponent>();
            if (part != null && part.PartID == partID)
            {
                Destroy(child.gameObject);
                attachedPartIDs.Remove(partID);
                Debug.Log($"Detached part: {partID}");
                return true;
            }
        }

        Debug.LogWarning("Part ID found but no matching GameObject");
        attachedPartIDs.Remove(partID);
        return false;
    }

    public bool HasPart(string partID)
    {
        return attachedPartIDs.Contains(partID);
    }

    public List<string> GetAttachedParts()
    {
        return new List<string>(attachedPartIDs);
    }
}