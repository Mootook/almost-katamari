using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    private KatamariInputController _input;
    private SphereCollider _sphereCollider;
    private Rigidbody _rb;
    public float pushForce = 10.0f;
    private List<StickyProp> _stuckProps;

    public Vector3 center {
        get { return _rb.worldCenterOfMass; }
        private set {}
    }

    // 1 == m
    // 0.1 == cm
    public float size = 1.0f;

    private void Start()
    {
       _input = GetComponent<KatamariInputController>(); 
       _rb = GetComponent<Rigidbody>();
       _sphereCollider = GetComponent<SphereCollider>();
       _stuckProps = new List<StickyProp>();
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
        GUI.Label(new Rect(0, 80, 100, 100), "Katamari Size: "  + size, red);
    }

    /// <summary>
    /// Update list of props
    /// and expand size of sphere collider.
    /// </summary>
    /// <param name="prop">Prop being stuck to katamari</param>
    public void OnPropPickup(StickyProp prop)
    {
        Expand(prop.size);
        _stuckProps.Add(prop);
    }

    public void OnRejectedCollision()
    {
        Debug.Log("Couldn't pick up Object! Moving at " + _rb.velocity);
    }

    public void Expand(float s)
    {
        _sphereCollider.radius += 0.008f;
        // add the newly picked up object's
        // size to the katamari's
        size += s / 2;
        // check size threshold here
    }
}
