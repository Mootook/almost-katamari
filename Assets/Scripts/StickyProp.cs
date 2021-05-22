using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    private Rigidbody _rb;
    private Mesh _mesh;
    private SimpleCameraFollow _camController;
    public bool isSticky = false;

    /// <summary>
    /// How much should the child colliders shrink once
    /// absorbed?
    /// </summary>
    private readonly int COLLIDER_SHRINK_SCALE = 4;

    // size
    // in m
    // of object
    public float size;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _camController = Camera.main.GetComponent<SimpleCameraFollow>();
    }

    private GameObject CompoundCollider()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "PropMesh")
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Katamari")
        {
            KatamariController kController = collider.gameObject.GetComponent<KatamariController>();
            if (CanBeAbsorbed(kController))
            {
                StickToKatamari(collider.gameObject);
            }
            else if (!isSticky)
            {
                // probably should expand this
                // calc the normals so we're not throwing items off
                // Vector3 collisionForce = collider.impulse / Time.fixedDeltaTime;
                // Debug.Log("Collision Magnitude" + collisionForce.magnitude);
                // kController.OnRejectedCollision();
            }
            // expand this else check here...
            // because some objects like the desk,
            // start as surfaces that are eventually
            // "pickup-able", I think it makes sense to
            // have thresholds throughout game where,
            // objects below a certain size are should not register with this check
            // for instance, !isSticky.
            // At certain thresholds we can loop
            // over game objects and make some more in a certain range sticky.

        }
    }

    /// <summary>
    /// Return whether or not this game object prop
    /// can be absorbed by given katamari based on comparative
    /// size.
    /// </summary>
    /// <param name="katamari">The Katamari</param>
    /// <returns>boolean indicating whether or not the katamari can absorb this game object prop.</returns>
    private bool CanBeAbsorbed(KatamariController katamari)
    {
        float katamariSize = katamari.size;
        return katamariSize > size;
    }

    /// <summary>
    /// Get the child container for compound colliders.
    /// </summary>
    /// <returns></returns>
    private Transform ChildColliderParent()
    {
        foreach (Transform child in transform)
            if (child.tag == "PropMesh")
                return child;
        return null;
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
        Destroy(_rb);
        KatamariController kController = katamari.GetComponent<KatamariController>();
        kController.OnPropPickup(this);
        // this only seems to
        // happen after certain size thresholds
        _camController.ExtendDistance();
        transform.SetParent(katamari.transform);
        // by shrinking the local position, once it's parented
        // we can move the collider more towards the Katamari's center.
        transform.localPosition /= 1.6f;
        Transform childCollder = ChildColliderParent();
        // scale down the mesh colliders
        // of picked up objects so that they don't
        // upset the sphere collider too much
        if (childCollder)
            childCollder.localScale /= COLLIDER_SHRINK_SCALE;
    }
}
