using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    private KatamariInputController _input;
    private SphereCollider _sphereCollider;
    private Rigidbody _rb;
    public float pushForce = 10.0f;

    // 1 == m
    // 0.1 == cm
    public float size = 1.0f;

    private void Start()
    {
       _input = GetComponent<KatamariInputController>(); 
       _rb = GetComponent<Rigidbody>();
       _sphereCollider = GetComponent<SphereCollider>();
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

    public void Expand(float s)
    {
        _sphereCollider.radius += 0.005f;
        // add the newly picked up object's
        // size to the katamari's
        size += s / 2;
    }
}
