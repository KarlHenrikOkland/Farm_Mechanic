using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Movement settings
    public float walkSpeed = 5.0f;
    public float strafeSpeed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 15.0f;

    // Noclip settings
    public float noclipWalkSpeed = 10.0f;
    public float noclipStrafeSpeed = 10.0f;
    public float noclipSprintMultiplier = 3.0f;
    private bool isNoclip = false;

    // Jump settings
    public float velocityThreshold = 1.0f;

    // State
    private bool isSprinting = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isNoclip)
        {
            Vector3 move = GetInputMovement();
            rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isNoclip)
            Jump();
    }

    void Update()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.N))
            ToggleNoclip();

        if (isNoclip)
            Noclip();
    }

    Vector3 GetInputMovement()
    {
        float fwd = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        if (isSprinting)
            fwd *= sprintMultiplier;

        Vector3 move = transform.forward * fwd * walkSpeed;
        move += transform.right * strafe * strafeSpeed;

        return move;
    }

    public void Jump()
    {
        if (rb.velocity.y < velocityThreshold)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Noclip()
    {
        float fwd = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        if (isSprinting)
            fwd *= noclipSprintMultiplier;

        Vector3 cameraFwd = Camera.main.transform.forward.normalized;
        Vector3 move = cameraFwd * fwd * noclipWalkSpeed;
        move += transform.right * strafe * noclipStrafeSpeed;

        if (Input.GetKey(KeyCode.Space))
            move += Vector3.up * noclipWalkSpeed;
        if (Input.GetKey(KeyCode.LeftControl))
            move -= Vector3.up * noclipWalkSpeed;

        transform.position += move * Time.deltaTime;
    }

    void ToggleNoclip()
    {
        isNoclip = !isNoclip;

        if (isNoclip)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
    }
}