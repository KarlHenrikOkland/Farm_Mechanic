using UnityEngine;

public class EnterVehicle : MonoBehaviour
{
    [Header("Settings")]
    public float interactionRange = 3f;
    public LayerMask vehicleLayer;
    public KeyCode interactKey = KeyCode.E;
    public string driverSeatName = "DriverSeat";

    [Header("References")]
    public Camera playerCamera;
    public GameObject onFootPlayer;    // Player GameObject when walking
    public GameObject vehiclePlayer;   // Player GameObject while driving

    private GameObject currentVehicle;
    private Transform driverSeat;
    private bool isInVehicle = false;
    private Vector3 lastPosition;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (isInVehicle)
                ExitVehicle();
            else
                TryEnterVehicle();
        }
    }

    void TryEnterVehicle()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward,
                            out RaycastHit hit, interactionRange, vehicleLayer))
        {
            currentVehicle = hit.collider.transform.root.gameObject;
            driverSeat = FindDeepChild(currentVehicle.transform, driverSeatName);

            if (driverSeat == null)
            {
                Debug.LogError($"'{driverSeatName}' not found on {currentVehicle.name}");
                return;
            }

            // Save last known position before entering vehicle
            lastPosition = onFootPlayer.transform.position;

            // Save current world transform of vehiclePlayer
            Vector3 worldPos = vehiclePlayer.transform.position;
            Quaternion worldRot = vehiclePlayer.transform.rotation;
            Vector3 worldScale = vehiclePlayer.transform.lossyScale;

            // Disable on-foot player
            onFootPlayer.SetActive(false);

            // Enable vehicle player
            vehiclePlayer.SetActive(true);

            // Parent to driver seat (keep world transform)
            vehiclePlayer.transform.SetParent(driverSeat, worldPositionStays: true);


            isInVehicle = true;
        }
    }

    void ExitVehicle()
    {
        if (!isInVehicle) return;

        // Detach and disable vehicle player
        vehiclePlayer.transform.SetParent(null);
        vehiclePlayer.SetActive(false);

        // Reposition and enable on-foot player
        onFootPlayer.transform.position = lastPosition + Vector3.right; // Slight offset to avoid overlapping
        onFootPlayer.SetActive(true);

        isInVehicle = false;
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return child;

            var result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}