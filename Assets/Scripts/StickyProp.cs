using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    private SimpleCameraFollow _camController;

    public Rigidbody rigidBody;
    public float size;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
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

    public bool CanBeAbsorbed(KatamariController katamari)
    {
        // TODO:
        // get point of contact? for more specifity of whether or not
        // object should be picked up.
        // https://docs.unity3d.com/ScriptReference/Collision-contacts.html
        float katamariSize = katamari.size;
        return katamariSize > size;
    }

    private Transform ChildColliderParent()
    {
        foreach (Transform child in transform)
            if (child.tag == "PropMesh")
                return child;
        return null;
    }

    public void Stick(KatamariController katamari)
    {
        Destroy(rigidBody);
        transform.SetParent(katamari.gameObject.transform);
        float towardsCenterFactor = 1.4f;
        transform.localPosition /= towardsCenterFactor;
        Transform compoundCollider = ChildColliderParent();
        compoundCollider.gameObject.layer = LayerMask.NameToLayer("Absorbed");
        if (compoundCollider)
        {
            compoundCollider.position = (katamari.Center + transform.position) / 2;
            foreach (Transform collider in compoundCollider)
                collider.gameObject.layer = LayerMask.NameToLayer("Absorbed");
        }
        // TODO:
        // - [ ] Destroy colliders after some time or
        //       after some threshold (list length and index?).
    }

    private IEnumerator DestroyCollider()
    {
        yield return new WaitForSeconds(10);
        Transform collider = ChildColliderParent();
        if (collider)
            Destroy(collider.gameObject);
    }
}
