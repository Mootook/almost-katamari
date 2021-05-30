using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    public Rigidbody rigidBody;

    public float density = 1.0f;
    public float Mass => rigidBody.mass;
    public float volume
    {
        get;
        private set;
    }
    public string propName
    {
        get;
        private set;
    }
    public float absorbedToCenterDivisor = 1.2f;

    private readonly string COMPOUND_COLLIDER_TAG = "PropMesh";
    private readonly string ABSORBED_LAYER = "Absorbed";

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        SetInitialMass();
    }

    private void SetInitialMass()
    {
        Vector3 size = LargestColliderSize();
        volume = ColliderVolume(size);
        rigidBody.mass = ColliderMass(size);
    }

    private Vector3 LargestColliderSize()
    {
        GameObject compoundCollider = CompoundCollider();
        BoxCollider[] colliders = compoundCollider.GetComponentsInChildren<BoxCollider>();
        Vector3 largestBoxColliderSize = Vector3.zero;
        foreach (BoxCollider collider in colliders)
        {
            if (collider.size.magnitude > largestBoxColliderSize.magnitude)
                largestBoxColliderSize = collider.size;
        }
        // TODO:
        // - [ ] Adjust box colliders of prefabs
        //       to set collider and not transform size.
        return largestBoxColliderSize;
    }

    private float ColliderMass(Vector3 size)
    {
        // assumes cube
        float volume = ColliderVolume(size);
        return volume * density;
    }

    private float ColliderVolume(Vector3 size)
    {
        return size.x * size.y * size.z;
    }

    private GameObject CompoundCollider()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == COMPOUND_COLLIDER_TAG)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public bool CanBeAbsorbed(float adjustedKatamariMass)
    {
        Debug.Log(
            propName +
            " has mass of " +
            Mass +
            " and katamari's determination is " +
            adjustedKatamariMass
        );
        return Mass < adjustedKatamariMass;
    }

    private Transform ChildColliderParent()
    {
        foreach (Transform child in transform)
            if (child.tag == COMPOUND_COLLIDER_TAG)
                return child;
        return null;
    }

    private LayerMask AbsorbedLayerMask()
    {
        return LayerMask.NameToLayer(ABSORBED_LAYER);
    }

    public void Stick(KatamariController katamari)
    {
        Destroy(rigidBody);
        transform.SetParent(katamari.gameObject.transform);
        MoveTowardsParent();

        Transform compoundCollider = ChildColliderParent();
        compoundCollider.gameObject.layer = AbsorbedLayerMask();
        if (compoundCollider)
            SetLayerForAbsorbedColliders(compoundCollider);

        // TODO:
        // - [ ] Destroy colliders after some time or
        //       after some threshold (list length and index?).
    }

    private void MoveTowardsParent()
    {
        transform.localPosition /= absorbedToCenterDivisor;
    }

    private void SetLayerForAbsorbedColliders(Transform compoundCollider)
    {
        // compoundCollider.position = (katamari.Center + transform.position) / 2;
        foreach (Transform collider in compoundCollider)
            collider.gameObject.layer = AbsorbedLayerMask();
    }

    private IEnumerator DestroyCollider()
    {
        yield return new WaitForSeconds(10);
        Transform collider = ChildColliderParent();
        if (collider)
            Destroy(collider.gameObject);
    }
}
