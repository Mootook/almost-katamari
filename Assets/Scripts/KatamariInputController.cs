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
    private Vector2 _leftThrottle = Vector2.zero;

    /// <summary>
    /// The input as a normalized vector2
    /// from the right stick.
    /// </summary>
    private Vector2 _rightThrottle = Vector2.zero;

    /// <summary>
    /// The dot product of the left and right throttle input
    /// vectors.
    /// </summary>
    private float _dot = 0.0f;

    /// <summary>
    /// Potential force to be applied from inputs.
    /// </summary>
    [HideInInspector] public Vector3 nextForce = Vector3.zero;

    /// <summary>
    /// Reference to the camera controller to update rotation
    /// on valid input.
    /// </summary>
    private SimpleCameraFollow _camController;

    /// <summary>
    /// Camera transform.
    /// Needed to transform local movement vectors3 into 
    /// vectors respective of camera rotation
    /// </summary>
    Transform _playerInputSpace;

    private void Start()
    {
        _playerInputSpace = Camera.main.transform;
        _camController = Camera.main.GetComponent<SimpleCameraFollow>();
    }

    private void Update()
    {
        // Update the camera
        // with inputs first, potential overrides follow
        _camController.StickYToRotation(_leftThrottle.y, _rightThrottle.y);
        Vector3 localForce = Vector3.zero;
        if (_leftThrottle != Vector2.zero && _rightThrottle != Vector2.zero)
        {
            // this if both inputs have values
            // if the dot > 0, consider it movement
            // if the dot == 0, nothing
            // if the dot < 0, rotation
            _dot = Vector2.Dot(_leftThrottle, _rightThrottle);
            if (_dot > 0.0f)
            {
                // average out the normalized input vectors
                float avgX = (_leftThrottle.x + _rightThrottle.x) / 2;
                float avgY = (_leftThrottle.y + _rightThrottle.y) / 2;
                // gets the force upon the katamari
                // relative to the camera transform
                // so that forward respects camera rotation about y
                localForce = _playerInputSpace.TransformDirection(
                    new Vector3(avgX, 0.0f, avgY)
                );
                // unset any rotations that were going to be applied
                // if this frame's set of inputs
                // correspond to a movement,
                // instead just halt the rotation
                _camController.HaltRotation();
            }
            else if (_dot < 0.0f)
            {
                // the sticks are in opposite directions,
                // this just accelerates the rotation speed of the camera
                _camController.AccelerateRotation();
            }
        }
        // only set nextForce once, at end of update call
        nextForce = localForce;
    }

    #region Katamari Input Callbacks

    public void OnLeftThrottle(InputValue val)
    {
        _leftThrottle = val.Get<Vector2>();
    }

    public void OnRightThrottle(InputValue val)
    {
        _rightThrottle = val.Get<Vector2>();
    }

    #endregion

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        float fps = 1.0f / Time.smoothDeltaTime;
        GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + fps.ToString(), red);        
        GUI.Label(new Rect(0, 20, 100, 100), "Left Input: "  + _leftThrottle.ToString(), red);
        GUI.Label(new Rect(0, 40, 100, 100), "Right Input: "  + _rightThrottle.ToString(), red);
        GUI.Label(new Rect(0, 60, 100, 100), "Dot: "  + _dot.ToString(), red);
    }
}
