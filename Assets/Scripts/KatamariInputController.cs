using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KatamariInputController : MonoBehaviour
{
    public void OnLeftThrottle(InputValue val)
    {
        Debug.Log("On Left Throttle" + val.Get<Vector2>());
    }

    public void OnRightThrottle(InputValue val)
    {
        Debug.Log("On Right Throttle" + val.Get<Vector2>());
    }
}
