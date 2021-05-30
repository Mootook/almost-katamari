using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The input controller for the katamari/player
/// calculates movements and rotations based on input's.
/// </summary>
public class KatamariInputController : MonoBehaviour
{
    private Vector2 leftThrottle = Vector2.zero;
    private Vector2 rightThrottle = Vector2.zero;
    private bool lockCursor = false;

    [HideInInspector] public Vector3 nextForce = Vector3.zero;

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private float InputToYRotation()
    {
        float yRotation = 0.0f;

        float leftVertical = leftThrottle.y;
        float rightVertical = rightThrottle.y;

        bool leftPush = leftVertical > rightVertical && leftVertical > 0.0f;
        bool leftPull = Mathf.Abs(leftVertical) > rightVertical && Mathf.Abs(leftVertical) > 0.0f;

        bool rightPush = rightVertical > leftVertical && rightVertical > 0.0f;
        bool rightPull = Mathf.Abs(rightVertical) > leftVertical && Mathf.Abs(rightVertical) > 0.0f;

        // clockwise = positive
        if (leftPush || rightPull)
        {
            yRotation = (leftVertical - rightVertical) / 2.0f;
        }
        else if (rightPush || leftPull)
        {
            yRotation = -Mathf.Abs((leftVertical - rightVertical)) / 2.0f;
        }
        return yRotation;
    }

    private Vector2 InputToFlatMovement()
    {
        Vector2 input = Vector2.zero;
        float inputDot = Vector3.Dot(leftThrottle, rightThrottle);

        bool inputsApproximatelySameDirection = inputDot > 0.0f;
        if (inputsApproximatelySameDirection)
        {
            float avgZ = (leftThrottle.y + rightThrottle.y) / 2.0f;
            float avgX = (leftThrottle.x + rightThrottle.x) / 2.0f;
            input.y = avgZ;
            input.x = avgX;
        }
        return input;
    }

    private void Update()
    {
        Vector2 flatMovement = InputToFlatMovement();
        float yRotation = InputToYRotation();

        nextForce = new Vector3(flatMovement.x, yRotation, flatMovement.y);
    }

    #region Katamari Input Callbacks

    public void OnLeftThrottle(InputValue val)
    {
        leftThrottle = val.Get<Vector2>();
    }

    public void OnRightThrottle(InputValue val)
    {
        rightThrottle = val.Get<Vector2>();
    }

    #endregion
}
