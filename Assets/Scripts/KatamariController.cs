using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    private KatamariInputController _input;
    private Rigidbody _rb;
    public float pushForce = 10.0f;

    /// <summary>
    /// Since the x, y, and z values of the katamari's
    /// local scale will all be the same,
    /// just return one of them to get the katamari's size.
    /// </summary>
    /// <value></value>
    public float size
    {
        get { return transform.localScale.x; }
        // this value is not writable
        set { return; }
    }

    private void Start()
    {
       _input = GetComponent<KatamariInputController>(); 
       _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_input.nextForce != Vector3.zero)
            _rb.AddForce(_input.nextForce * pushForce);
    }

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        GUI.Label(new Rect(0, 80, 100, 100), "Bounds Size: "  + transform.localScale, red);
    }
}
