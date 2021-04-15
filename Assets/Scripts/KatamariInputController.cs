using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The input controller for the katamari/player
/// calculates movements and rotations based on input's.
/// </summary>
public class KatamariInputController : MonoBehaviour
{
    /// <summary>
    /// The input as a normalized vector2
    /// from the left stick.
    /// </summary>
    public Vector2 leftThrottle = Vector2.zero;

    /// <summary>
    /// The input as a normalized vector2
    /// from the right stick.
    /// </summary>
    public Vector2 rightThrottle = Vector2.zero;

    /// <summary>
    /// Potential force to be applied from inputs.
    /// </summary>
    public Vector3 nextForce = Vector3.zero;

    /// <summary>
    /// The dot product of the left and right throttle input
    /// vectors.
    /// </summary>
    private float _dot;

    private void Update()
    {
        if (leftThrottle != Vector2.zero && rightThrottle != Vector2.zero)
        {
            // this if both inputs have values
            // if the dot > 0, consider it movement
            // if the dot == 0, nothing
            // if the dot < 0, rotation
            _dot = Vector2.Dot(leftThrottle, rightThrottle);
            nextForce = Vector3.zero;
            if (_dot > 0.0f)
            {
                Debug.Log("Movement");
                // average out the normalized input vectors
                float avgX = (leftThrottle.x + rightThrottle.x) / 2;
                float avgY = (leftThrottle.y + rightThrottle.y) / 2;
                nextForce = new Vector3(avgX, 0.0f, avgY);
            }
            else if (_dot < 0.0f)
            {
                // they're in opposite directions
                Debug.Log("Accelerated camera rotation");
            }
        }
        else if (leftThrottle != Vector2.zero)
        {
            // rightThrottle == Vector.zero
            // this is a rotation
        }
        else if (rightThrottle != Vector2.zero)
        {
            // leftThrottle == Vector2.zero
            // this is a rotation
        }
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

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        float fps = 1.0f / Time.smoothDeltaTime;
        GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + fps.ToString(), red);        
        GUI.Label(new Rect(0, 20, 100, 100), "Left Input: "  + leftThrottle.ToString(), red);
        GUI.Label(new Rect(0, 40, 100, 100), "Right Input: "  + rightThrottle.ToString(), red);
        GUI.Label(new Rect(0, 60, 100, 100), "Dot: "  + _dot.ToString(), red);
    }
}
