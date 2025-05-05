using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnginePartDatabase", menuName = "Engine/Part Database")]
public class PartDatabase : ScriptableObject
{
    [System.Serializable]
    public class EnginePart
    {
        public string partID;
        public string partName;
        public GameObject prefab;

        [TextArea] public string functionDescription;  // NEW: Describes what the part does
        public List<string> requiredParts;
        public List<string> incompatibleParts;

        public bool IsFunctional(List<string> attachedParts)
        {
            foreach (var required in requiredParts)
            {
                if (!attachedParts.Contains(required))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public List<EnginePart> allParts = new List<EnginePart>();
    private Dictionary<string, EnginePart> partDictionary;

    private void OnEnable()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        partDictionary = new Dictionary<string, EnginePart>();
        foreach (var part in allParts)
        {
            partDictionary[part.partID] = part;
        }
    }

    public EnginePart GetPart(string partID)
    {
        if (partDictionary == null || partDictionary.Count != allParts.Count)
        {
            BuildDictionary();
        }

        partDictionary.TryGetValue(partID, out EnginePart part);
        return part;
    }

    public bool IsPartCompatible(string partID, List<string> attachedParts)
    {
        var part = GetPart(partID);
        if (part == null) return false;

        foreach (var required in part.requiredParts)
        {
            if (!attachedParts.Contains(required))
            {
                Debug.Log($"Missing required part: {required}");
                return false;
            }
        }

        foreach (var incompatible in part.incompatibleParts)
        {
            if (attachedParts.Contains(incompatible))
            {
                Debug.Log($"Incompatible with attached part: {incompatible}");
                return false;
            }
        }

        return true;
    }

    public bool IsPartFunctional(string partID, List<string> attachedParts)
    {
        var part = GetPart(partID);
        return part != null && part.IsFunctional(attachedParts);
    }
}