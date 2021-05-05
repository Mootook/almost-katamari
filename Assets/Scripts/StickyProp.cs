using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    private MeshCollider _meshCollider;
    private Rigidbody _rb;
    private Mesh _mesh;
    private SimpleCameraFollow _camController;

    // size
    // in m
    // of object
    public float size;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _camController = Camera.main.GetComponent<SimpleCameraFollow>();

        _mesh = GetComponentInChildren<MeshFilter>().mesh;
        _meshCollider = GetComponentInChildren<MeshCollider>();
    }

    private void OnCollisionEnter(Collision collider)
    {

        if (
            collider.gameObject.tag == "Katamari" &&
            CanBeAbsorbed(collider.gameObject)
        )
        {
            StickToKatamari(collider.gameObject);
        }
    }

    /// <summary>
    /// Return whether or not this game object prop
    /// can be absorbed by given katamari based on comparative
    /// size.
    /// </summary>
    /// <param name="katamari">The Katamari</param>
    /// <returns>boolean indicating whether or not the katamari can absorb this game object prop.</returns>
    private bool CanBeAbsorbed(GameObject katamari)
    {
        KatamariController kController = katamari.GetComponent<KatamariController>();
        float katamariSize = kController.size;
        return katamariSize > size;
    }

    /// <summary>
    /// All the operations necessary to stick the
    /// game object to the katamari.
    /// 
    /// The Katamari has entered the upscaled collider on the prop game object
    /// if it is below a certain size relative to the katamari, 
    /// 1. destroy this prop instance's rigidbody
    /// 2. isTrigger on the collider = false;
    /// 3. re-parent to the katamari
    /// </summary>
    private void StickToKatamari(GameObject katamari)
    {
        foreach (Transform child in transform)
        {
            // scale down the mesh colliders
            // of picked up objects so that they dont
            // upset the sphere collider too much
            if (child.tag == "PropMesh")
                child.localScale /= 2;
        }
        Destroy(_rb);
        transform.SetParent(katamari.transform);

        KatamariController kController = katamari.GetComponent<KatamariController>();
        kController.Expand(size);
        // this only seems to
        // happen after certain size thresholds
        _camController.ExtendDistance();
    }
}
