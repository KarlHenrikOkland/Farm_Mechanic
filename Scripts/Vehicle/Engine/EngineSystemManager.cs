using System.Collections.Generic;
using UnityEngine;

public class EngineSystemManager : MonoBehaviour
{
    public PartDatabase partDatabase;

    private HashSet<string> attachedParts = new HashSet<string>();

    private bool batteryPowerAvailable = false;
    private float batteryLevel = 100f;
    private bool engineRunning = false;

    private float batteryDrainRate = 5f;      // Drain per second if no alternator
    private float alternatorRechargeRate = 10f; // Recharge per second if alternator attached

    private void Update()
    {
        if (engineRunning)
        {
            if (attachedParts.Contains("Alternator"))
            {
                batteryLevel = Mathf.Min(100f, batteryLevel + alternatorRechargeRate * Time.deltaTime);
            }
            else
            {
                batteryLevel -= batteryDrainRate * Time.deltaTime;
                if (batteryLevel <= 0f)
                {
                    batteryPowerAvailable = false;
                    StopEngine();
                    Debug.Log("Battery depleted. Engine stopped.");
                }
            }
        }
    }

    public void AddPart(string partID)
    {
        attachedParts.Add(partID);
        EvaluateEngineState();
    }

    public void RemovePart(string partID)
    {
        attachedParts.Remove(partID);
        EvaluateEngineState();
    }

    private void EvaluateEngineState()
    {
        batteryPowerAvailable = attachedParts.Contains("Battery") && batteryLevel > 0f;

        if (!batteryPowerAvailable)
        {
            StopEngine();
            return;
        }

        if (CanEngineStart())
        {
            StartEngine();
        }
        else
        {
            StopEngine();
        }
    }

    private bool CanEngineStart()
    {
        return attachedParts.Contains("Starter") &&
               attachedParts.Contains("FuelPump") &&
               attachedParts.Contains("IgnitionCoil") &&
               attachedParts.Contains("SparkPlugs") &&
               attachedParts.Contains("ECU");
    }

    private void StartEngine()
    {
        if (!engineRunning)
        {
            engineRunning = true;
            Debug.Log("Engine started.");
            // Insert animation/sound/logic here
        }
    }

    private void StopEngine()
    {
        if (engineRunning)
        {
            engineRunning = false;
            Debug.Log("Engine stopped.");
            // Insert shutdown effects
        }
    }

    public bool IsEngineRunning()
    {
        return engineRunning;
    }

    public float GetBatteryLevel()
    {
        return batteryLevel;
    }
}
