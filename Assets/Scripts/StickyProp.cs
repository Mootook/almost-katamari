using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    /// <summary>
    /// Should this prop be able to be absorb by the katamari?
    /// </summary>
    public bool isSticky;

    private BoxCollider _boxCollider;
    private SphereCollider _sphereCollider;
    private Rigidbody _rb;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _sphereCollider = GetComponent<SphereCollider>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (
            collider.tag == "Katamari" &&
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
        _boxCollider.size = new Vector3(1, 1, 1);
        _boxCollider.isTrigger = false;
        _sphereCollider.isTrigger = false;
        Destroy(_rb);
        transform.SetParent(katamari.transform);
    }
}
