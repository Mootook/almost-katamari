using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraFollow : MonoBehaviour
{

    /// <summary>
    /// The transform of the game object to follow and orbit.
    /// </summary>
    [SerializeField]
    Transform target = default;

    /// <summary>
    /// Store the target transform.position
    /// each frame to calculate differences in movement
    /// between frames.
    /// </summary>
    private Vector3 _targetPoint;

    /// <summary>
    /// Distance behind the game object.
    /// </summary>
    [SerializeField, Range(1f, 20f)]
    public float distanceZ = 6f;

    public float distanceY = 1f;

    /// <summary>
    /// Space within viewport that acts as a 
    /// "deadzone" so that the camera isn't following
    /// the small, micro movements, katamari must break forceRadius
    /// in order to trigger camera movement.
    /// </summary>
    [SerializeField, Min(0f)]
    private float forceRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    private float focusCentering = 0.5f;

    /// <summary>
    /// Angles on the x, y
    /// that are used to orbit about the target gameObject
    /// orbitAngle.y is what is set by rotation inputs from the player.
    /// </summary>
    private Vector2 orbitAngles = new Vector2(0f, 0f);

    /// <summary>
    /// The un-accelerated rotation speed about the game object.
    /// </summary>
    [SerializeField, Range(1f, 360f)]
    float baseRotationSpeed = 90f;

    /// <summary>
    /// The speed at which the camera "orbits" the katarmi
    /// this defaults to baseRotationSpeed but can be increased
    /// by a potential acceleration if input axis are opposites.
    /// </summary>
    float rotationSpeed;

    /// <summary>
    /// Possible rotation
    ///     - 0 means no rotation
    ///     - 1 means clockwise
    ///     - -1 means counter-clockwise
    /// </summary>
    private float rotationDir = 0;

    /// <summary>
    /// Set defaults.
    /// </summary>
    private void Awake()
    {
        rotationSpeed = baseRotationSpeed;
        _targetPoint = target.position;
    }

    /// <summary>
    /// Do all of the camera position/rotation updates
    /// in LateUpdate to ensure target's most up to date position
    /// is referenced for calculations and tracking.
    /// </summary>
    private void LateUpdate()
    {
        UpdateTarget();
        ManualRotation();
        Quaternion lookRotation = Quaternion.Euler(orbitAngles);
        // set the local position
        Vector3 lookDirection = lookRotation * Vector3.forward;
        // Add one on the y to keep the camera off the ground
        Vector3 localPosition = (_targetPoint - lookDirection * distanceZ);
        localPosition.y += distanceY;
        transform.SetPositionAndRotation(localPosition, lookRotation);
    }

    private void UpdateTarget()
    {
        // update target properties
        Vector3 nextTarget = target.position;
        // force radius allows for a "deadzone"
        // with the camera follow
        // if the distance between the target on the last frame and current frame
        // is larger than the parameter,
        // Lerp into this new target point,
        // if not, no movement.
        if (forceRadius > 0f)
        {
            float distance = Vector3.Distance(nextTarget, _targetPoint);
            // amount to interpolate
            float t = 1f;
            // recenter the camera to create a 
            // 'catching up' effect
            if (distance > 0.01f && focusCentering > 0f)
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            if (distance > forceRadius)
                t = Mathf.Min(t, forceRadius / distance);
            _targetPoint = Vector3.Lerp(nextTarget, _targetPoint, t);
        }
        else
            _targetPoint = nextTarget;
    }

    /// <summary>
    /// Read off of the rotationDir set
    /// by the input controller to determine if rotation is in order.
    /// Needs to be greater than scoped epsilon.
    /// </summary>
    private void ManualRotation()
    {
        const float e = 0.001f;
        if (rotationDir > e || rotationDir < -e)
            orbitAngles.y += rotationSpeed * Time.unscaledDeltaTime * rotationDir;
    }

    /// <summary>
    /// Convert the y values of the input on the sticks
    /// into an integer used for rotating camera.
    /// Positive on ly means clockwise (pushing with left hand),
    /// negative on ly means counter clockwise (pulling with left hand)
    /// 
    /// Positive on ry mean counter-clockwise (pushing with right hand)
    /// negative on ry means clock wise (pulling with right hand)
    /// 
    /// </summary>
    /// <param name="ly"></param>
    /// <param name="ry"></param>
    public void StickYToRotation(float ly, float ry)
    {
        const float e = 0.2f;
        if (ly > e || ly < -e)
            rotationDir = ly;
        else if (ry > e || ry < -e)
            // invert the ry as it behaves as the inverse of ly for rotationDir
            rotationDir = -ry;
        else
            rotationDir = 0;
        // this gets upset if it's an accelerated
        // rotation by the Katamari's update call
        // that reads potential for inverted input axis
        rotationSpeed = baseRotationSpeed;
    }

    /// <summary>
    /// Accelerate the rotation if the vectors of the input sticks
    /// are in opposite directions (dot ~< 0)
    /// </summary>
    public void AccelerateRotation()
    {
        rotationSpeed = baseRotationSpeed * 1.75f;
    }

    /// <summary>
    /// Halt the rotation, if katamari is moving.
    /// </summary>
    public void HaltRotation ()
    {
        rotationDir = 0;
    }

    public void ExtendDistance()
    {
        // add to distance Y
        // here
    }
}
