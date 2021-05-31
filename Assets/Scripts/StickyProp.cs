using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyProp : MonoBehaviour
{
    private Rigidbody rigidBody;

    public float density = 1.0f;
    public float Mass => rigidBody.mass;
    public float volume
    {
        get;
        private set;
    }
    public string propName;
    public float absorbedToCenterDivisor = 1.2f;

    private readonly string COMPOUND_COLLIDER_TAG = "PropMesh";
    private readonly string ABSORBED_LAYER = "Absorbed";

    // lower allows prop to be picked up earlier
    // otherwise it must match the katamari's full mass;
    public float massComparisonMultiplier = 0.25f;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private float CalculateMass()
    {
        List<Vector3> colliderSizes = ColliderSizes();
        foreach (Vector3 size in colliderSizes)
            volume += ColliderVolume(size);
        return ColliderMass();
    }

    private List<Vector3> ColliderSizes()
    {
        List<Vector3> colliderSizes = new List<Vector3>();
        BoxCollider[] colliders = AllColliders();

        foreach (BoxCollider collider in colliders)
            colliderSizes.Add(collider.size);
        return colliderSizes;
    }

    private BoxCollider[] AllColliders()
    {
        GameObject compoundCollider = CompoundCollider();
        BoxCollider[] colliders = compoundCollider.GetComponentsInChildren<BoxCollider>();
        if (colliders == null || colliders.Length == 0)
        {
            BoxCollider singularCollider = compoundCollider.GetComponent<BoxCollider>();
            colliders = new BoxCollider[] { singularCollider };
        }
        return colliders;
    }

    private Vector3 LargestColliderSize()
    {
        BoxCollider[] colliders = AllColliders();
        Vector3 largestBoxColliderSize = Vector3.zero;

        foreach (BoxCollider collider in colliders)
        {
            if (collider.size.magnitude > largestBoxColliderSize.magnitude)
                largestBoxColliderSize = collider.size;
        }
        return largestBoxColliderSize;
    }

    private float ColliderMass()
    {
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

    public bool CanBeAbsorbed(float katamariMass)
    {
        return Mass < katamariMass * massComparisonMultiplier;
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
