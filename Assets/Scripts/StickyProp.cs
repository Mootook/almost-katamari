using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    /// <summary>
    /// Should this prop be able to be absorb by the katamari?
    /// </summary>
    public bool isSticky;

    private MeshCollider _meshCollider;
    private SphereCollider _sphereCollider;
    private Rigidbody _rb;

    private Mesh _mesh;

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _rb = GetComponent<Rigidbody>();

        _mesh = GetComponentInChildren<MeshFilter>().mesh;
        _meshCollider = GetComponentInChildren<MeshCollider>();
    }

    private void OnCollisionEnter(Collision collider)
    {

        if (
            collider.gameObject.tag == "Katamari" &&
            CanBeAbsorbed(collider.gameObject) &&
            isSticky
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
        return true;
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
            if (child.tag == "PropMesh")
                child.localScale /= 2;
        }
        Destroy(_rb);
        transform.SetParent(katamari.transform);

        KatamariController kController = katamari.GetComponent<KatamariController>();
        kController.Expand();

        // Lengthen the distance to camera here as well
    }
}
