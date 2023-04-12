using UnityEngine;
using Mirror;

public class ThirdPersonController : NetworkBehaviour, IJerkable
{
    [SerializeField] private float moveSpeed = 10f;  // The character's movement speed
    [SerializeField] private float turnSpeed = 3f;  // The character's turn speed
    [SerializeField] private float jumpForce = 10f; // The force applied when the character jumps
    [SerializeField] private float jumpForwardSpeed = 5f; // The forward speed applied when the character jumps
    [SerializeField] private float lookSensitivity = 2f; // The sensitivity of the mouse for looking around
    [SerializeField] private float groundCheckDistance = 0.1f; // The distance from the character's center to check for ground

    [SerializeField] private float jerkDistance = 10f; // The jerk distance
    [SerializeField] private float jerkCooldown = 2f; // The jerk cooldown in seconds

    private Rigidbody rb;
    private Camera cam;
    private bool isGrounded = true;
    private float lookHorizontal = 0f;
    private float lookVertical = 0f;

    [SyncVar(hook = nameof(OnJerkCooldownTimerChanged))] private float jerkCooldownTimer;
    [SyncVar(hook = nameof(OnScoreChanged))]  public int score;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Confined; // Lock the cursor to the game window
    }

    private void Start()
    {
        if (!isLocalPlayer) Destroy(cam.gameObject);
        else ScoreManager.instance.SetCamera(cam);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        // timers
        if (isJerking()) CmdChangeJerkCooldownTime(jerkCooldownTimer - Time.deltaTime);

        // Get input from the horizontal and vertical axis
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        movement = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * movement;
        movement.Normalize();

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJerking())
        {
            // Add upward and forward force to the character
            rb.AddForce(transform.up * jumpForce + transform.forward * jumpForwardSpeed, ForceMode.Impulse);
            isGrounded = false;
        }

        // Handle jerking
        if (Input.GetMouseButtonDown(0) && !isJerking())
        {
            Jerk();
        }

        // Apply movement to the character
        if (isGrounded && !isJerking())
        {
            rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);
        }

        // Rotate the character towards the movement direction
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Handle looking around with the mouse
        float lookHorizontalDelta = Input.GetAxis("Mouse X") * lookSensitivity;
        float lookVerticalDelta = Input.GetAxis("Mouse Y") * lookSensitivity;
        lookHorizontal += lookHorizontalDelta;
        lookVertical -= lookVerticalDelta;
        lookVertical = Mathf.Clamp(lookVertical, -90f, 90f);
        transform.rotation = Quaternion.Euler(0f, lookHorizontal, 0f);
        cam.transform.localRotation = Quaternion.Euler(lookVertical, 0f, 0f);

        isGrounded = CheckGround();
    }

    private bool CheckGround()
    {
        // Check if the character is grounded
        Ray groundCheckRay = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(groundCheckRay, out hitInfo, groundCheckDistance)) return true;
        else return false;
    }

    public void Jerk()
    {
        // Set the jerk cooldown timer
        CmdChangeJerkCooldownTime(jerkCooldown);

        // Calculate the dash direction based on the character's forward direction
        Vector3 dashDirection = transform.forward;

        float force = jerkDistance;
        if (isGrounded) force *= 2f;

        // Apply the dash force to the character
        rb.AddForce(dashDirection.normalized * force, ForceMode.Impulse);
    }


    private bool isJerking()
    {
        return jerkCooldownTimer > 0;
    }

    public void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();

        if (damagable != null)
        {
            if (damagable.DamagableNow() && isJerking())
            {
                CmdChangeScore(score + 1);
                damagable.TakeDamage();
            }
        } 
    }

    [Command]
    private void CmdChangeJerkCooldownTime(float newTimer)
    {
        jerkCooldownTimer = newTimer;
    }

    [Command]
    private void CmdChangeScore(int newScore)
    {
        score = newScore;
    }

    private void OnJerkCooldownTimerChanged(float oldTimer, float newTimer)
    {
        jerkCooldownTimer = newTimer;
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        score = newScore;
        ScoreManager.instance.UpdateInfo();
    }

    public void OnRestart()
    {
        CmdChangeScore(0);
    }
}