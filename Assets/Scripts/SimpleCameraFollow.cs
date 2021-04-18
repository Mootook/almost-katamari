using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraFollow : MonoBehaviour
{

    [SerializeField]
    Transform target = default;

    [SerializeField, Range(1f, 20f)]
    private float distance = 8f;

    [SerializeField, Min(0f)]
    private float forceRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    private float focusCentering = 0.5f;

    private Vector3 _targetPoint;

    private Vector2 orbitAngles = new Vector2(0f, 0f);

    [SerializeField, Range(1f, 360f)]
    float baseRotationSpeed = 90f;

    float rotationSpeed;

    /// <summary>
    /// Possible rotation
    ///     - 0 means no rotation
    ///     - 1 means clockwise
    ///     - -1 means counter-clockwise
    /// </summary>
    private float rotationDir = 0;

    private void Awake()
    {
        rotationSpeed = baseRotationSpeed;
        _targetPoint = target.position;
    }

    private void LateUpdate()
    {
        UpdateTarget();
        ManualRotation();
        Quaternion lookRotation = Quaternion.Euler(orbitAngles);
        // set the local position
        Vector3 lookDirection = lookRotation * Vector3.forward;
        // Add one on the y to keep the camera off the ground
        Vector3 localPosition = (_targetPoint - lookDirection * distance);
        localPosition.y += 1;
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

    private void ManualRotation()
    {
        const float e = 0.001f;
        if (rotationDir > e || rotationDir < -e)
            orbitAngles.y += rotationSpeed * Time.unscaledDeltaTime * rotationDir;
    }

    /// <summary>
    /// Convert the y values of the input on the sitcks.
    /// Into an integer used for rotating camerea.
    /// Positve on ly means clockwise (pushing with left hand),
    /// negative on ly means counter clockwise (pulling with left hand)
    /// 
    /// Positive on ry mean counter-clockwise (pushing with right hand)
    /// negative on ry means clock wise (pulling with right hand)
    /// 
    /// </summary>
    /// <param name="ly"></param>
    /// <param name="ry"></param>
    public void StickYToRotation (float ly, float ry, bool accelerated = false)
    {
        const float e = 0.001f;
        if (ly > e || ly < -e)
            rotationDir = ly;
        else if (ry > e || ry < -e)
            rotationDir = -ry;
        else
            rotationDir = 0;

        rotationSpeed = (accelerated) ? (baseRotationSpeed * 1.75f) : baseRotationSpeed;
    }

    /// <summary>
    /// Halt the rotation, if katamari is moving.
    /// </summary>
    public void HaltRotation ()
    {
        rotationDir = 0;
    }
}
