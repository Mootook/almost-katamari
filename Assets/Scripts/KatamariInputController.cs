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

    [HideInInspector] public Vector3 nextForce = Vector3.zero;

    private float InputToYRotation()
    {
        float yRotation = 0.0f;

        float leftVertical = leftThrottle.y;
        float rightVertical = rightThrottle.y;

        float deadZoneEpsilon = 0.05f;

        bool leftPush = leftVertical > deadZoneEpsilon;
        bool leftPull = leftVertical < -deadZoneEpsilon;

        bool rightPush = rightVertical > deadZoneEpsilon;
        bool rightPull = rightVertical < -deadZoneEpsilon;

        // TODO:
        // - [ ] Make sure we're not moving
        //       so we don't rotate unnecessarily
        if ((leftPush && !rightPush) || (rightPull && !leftPull))
            yRotation = (leftVertical - rightVertical) / 2.0f;
        else if ((rightPush && !leftPush) || (leftPull && !rightPull))
            yRotation = -Mathf.Abs((leftVertical - rightVertical)) / 2.0f;

        return yRotation;
    }

    private Vector2 InputToFlatMovement()
    {
        Vector2 input = Vector2.zero;
        float inputDot = Vector2.Dot(leftThrottle, rightThrottle);

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
