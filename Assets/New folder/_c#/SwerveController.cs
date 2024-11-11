using UnityEngine;

public class SwerveController : MonoBehaviour
{
    [SerializeField] private float swerveSpeed = 0.5f; // Adjust for desired sensitivity
    [SerializeField] private float maxSwerveAmount = 1.5f; // Adjust for desired maximum swerve
    [SerializeField] private float smoothing = 0.05f; // Smoothing factor for the lerp
    [SerializeField] private float maxRotationAngle = 15f; // Maximum rotation angle
    [SerializeField] private float rotationSpeed = 5f; // Speed of returning to original rotation
    [SerializeField] private float forwardSpeed = 5f; // Speed of forward movement
    [SerializeField] private float clampXMin = -2f; // Minimum X position
    [SerializeField] private float clampXMax = 2f; // Maximum X position

    private float lastFrameFingerPositionX;
    private float moveFactorX;
    private float targetPositionX;
    private Quaternion originalRotation;

    public Animator animator;
   public  bool canWork;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
     
    }

    private bool ArenaReached;

    private void Update()
    {
        if (!canWork)
            return;

#if UNITY_EDITOR
        HandleInput();
#else
        HandleTouchInput();
#endif
        Swerve();
        Rotate();
        MoveForward();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastFrameFingerPositionX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            moveFactorX = (Input.mousePosition.x - lastFrameFingerPositionX) * swerveSpeed;
            lastFrameFingerPositionX = Input.mousePosition.x;
        }
        else
        {
            moveFactorX = 0f;
        }

        // Keyboard arrow key controls
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveFactorX = -swerveSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveFactorX = swerveSpeed;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                lastFrameFingerPositionX = touch.position.x;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                moveFactorX = (touch.position.x - lastFrameFingerPositionX) * swerveSpeed;
                lastFrameFingerPositionX = touch.position.x;
            }
            else
            {
                moveFactorX = 0f;
            }
        }
    }

    private void Swerve()
    {
        float swerveAmount = moveFactorX * Time.deltaTime;
        swerveAmount = Mathf.Clamp(swerveAmount, -maxSwerveAmount, maxSwerveAmount);

        // Calculate the target position
        targetPositionX = transform.position.x + swerveAmount;

        // Smoothly move towards the target position
        Vector3 newPosition = new Vector3(Mathf.Lerp(transform.position.x, targetPositionX, smoothing),
            transform.position.y, transform.position.z);

        // Clamp the X position
        newPosition.x = Mathf.Clamp(newPosition.x, clampXMin, clampXMax);

        transform.position = newPosition;
    }

    private void Rotate()
    {
        float rotationFactor = Mathf.Clamp(moveFactorX, -1, 1);
        float targetRotationY = rotationFactor * maxRotationAngle;

        Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);

        if (moveFactorX == 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
    }
}
