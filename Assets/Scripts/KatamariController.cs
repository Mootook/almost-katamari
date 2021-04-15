using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    private KatamariInputController _input;
    private Rigidbody _rb;
    public float pushForce = 10.0f;

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
}
