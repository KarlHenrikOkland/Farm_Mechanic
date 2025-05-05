using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform driverSeat;
    public Camera fpsCamera;
    public Camera vehicleCamera;
    public MonoBehaviour PlayerMovement; // Reference to player movement script

    [Header("Settings")]
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    private GameObject _player;
    private bool _isPlayerInside = false;
    private MonoBehaviour _vehicleControlScript;

    void Start()
    {
        _vehicleControlScript = GetComponent<MonoBehaviour>(); // Replace with specific type if you have one

        if (_vehicleControlScript != null)
            _vehicleControlScript.enabled = true;
        else
            Debug.LogWarning("Vehicle control script not found on vehicle!");

        if (driverSeat == null)
            Debug.LogWarning("Driver seat is not assigned!");
    }

    void Update()
    {
        if (_isPlayerInside && Input.GetKeyDown(interactKey))
        {
            ExitVehicle();
        }
    }

    public void TryEnterVehicle(GameObject player)
    {
        if (_isPlayerInside) return;

        _player = player;

        // Parent and reset position
        _player.transform.SetParent(driverSeat);
        _player.transform.localPosition = Vector3.zero;
        _player.transform.localRotation = Quaternion.identity;

        if (PlayerMovement != null)
            PlayerMovement.enabled = false;

        _isPlayerInside = true;

        if (_vehicleControlScript != null)
            _vehicleControlScript.enabled = true;

        if (fpsCamera != null)
            fpsCamera.enabled = false;

        if (vehicleCamera != null)
            vehicleCamera.enabled = true;
    }

    private void ExitVehicle()
    {
        _player.transform.SetParent(null);
        _player.transform.position = driverSeat.position + driverSeat.forward * 2f;

        if (PlayerMovement != null)
            PlayerMovement.enabled = true;

        _isPlayerInside = false;

        if (_vehicleControlScript != null)
            _vehicleControlScript.enabled = false;

        if (fpsCamera != null)
            fpsCamera.enabled = true;

        if (vehicleCamera != null)
            vehicleCamera.enabled = false;
    }
}