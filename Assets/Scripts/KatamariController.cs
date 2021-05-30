using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    #region Members
    private KatamariInputController input;
    private SphereCollider sphereCollider;
    private Rigidbody rigidBody;
    private List<StickyProp> stuckProps;

    public float size = 1.0f;

    [Header("Movement")]
    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;
    [SerializeField]
    private float rotationMultiplier = 80.0f;
    [SerializeField]
    private float torqueMultiplier = 750.0f;
    [SerializeField]
    private float forceMultiplier = 250.0f;

    [Header("Climbing")]
    public float climbForceMultiplier = 5.0f;
    [SerializeField, Range(90, 180)]
    [Tooltip("The maximum angle of object that can be climb.")]
    float maxClimbAngle = 180.0f;
    [SerializeField, Range(0, 90)]
    [Tooltip("The maximum angle between an object and input to be considered climbing (lower = more difficult)")]
    public float maxInputClimbAngle = 25f;

    [SerializeField]
    [Tooltip("Which layers to allow climbing?")]
    LayerMask climbMask = -1;

    private Vector3 velocity;
    private Vector3 contactNormal, climbNormal, lastClimbNormal;

    private int groundContactCount, climbContactCount;
    private float minGroundDotProduct, minClimbDotProduct, minClimbInputDot;

    private bool OnGround => groundContactCount > 0;
    private bool Climbing => climbContactCount > 0 && Vector3.Dot(lastClimbNormal, velocity) < minClimbInputDot;
    // private bool Climbing => climbContactCount > 0;
    public float Radius => sphereCollider.radius;
    public Vector3 Center => rigidBody.worldCenterOfMass;

    private float torqueMultiplierWithMass => torqueMultiplier * rigidBody.mass;
    private float airborneForceMultiplier => forceMultiplier / 2.0f;

    private Renderer katamariRenderer;
    private Color defaultColor;

    public float rotationY;

    public SimpleCameraFollow cameraFollow;

    #endregion

    private void OnValidate()
    {
        // https://catlikecoding.com/unity/tutorials/movement/physics/
        // for explanation of getting dot product from angle.
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
        // get the negative, as this is a "maximum" allowance being
        // created
        minClimbInputDot = -Mathf.Cos(maxInputClimbAngle * Mathf.Deg2Rad);
    }

    #region LifeCycle

    void Start()
    {
        stuckProps = new List<StickyProp>();

        input = GetComponent<KatamariInputController>();
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        katamariRenderer = GetComponent<Renderer>();
        defaultColor = katamariRenderer.material.GetColor("_Color");

        cameraFollow.SetInitialParameters();
    }

    private void Update()
    {
        rotationY += input.nextForce.y * Time.deltaTime * rotationMultiplier;
    }


    private void FixedUpdate()
    {
        Vector3 forward = new Vector3(0, rotationY, 0);
        ApplyInputTorque();
        ApplyInputForce();

        ClearState();
    }

    private void ApplyInputTorque()
    {
        float forwardInput = input.nextForce.z;
        float lateralInput = input.nextForce.x;

        Vector3 torque = new Vector3(
            forwardInput * torqueMultiplierWithMass,
            input.nextForce.y * torqueMultiplierWithMass,
            -lateralInput * torqueMultiplierWithMass
        );

        Vector3 forward = new Vector3(0, rotationY, 0);
        rigidBody.AddTorque(Quaternion.Euler(forward) * torque);
    }

    private void ApplyInputForce()
    {
        float lateralInput = input.nextForce.x;
        float forwardInput = input.nextForce.z;

        float forceForContactState = OnGround ? forceMultiplier : airborneForceMultiplier;

        Vector3 force = new Vector3(
            lateralInput * forceForContactState,
            ClimbingForce(),
            forwardInput * forceForContactState
        );

        Vector3 forward = new Vector3(0, rotationY, 0);
        velocity = Quaternion.Euler(forward) * force;
        rigidBody.AddForce(velocity);
    }

    float ClimbingForce()
    {
        float upwardVelocity = 0.0f;
        if (Climbing)
        {
            katamariRenderer.material.SetColor("_Color", Color.red);
            upwardVelocity += climbForceMultiplier * rigidBody.mass;
        }
        else
        {
            katamariRenderer.material.SetColor("_Color", defaultColor);
        }
        return upwardVelocity;
    }


    private void ClearState()
    {
        groundContactCount = climbContactCount = 0;
        contactNormal = climbNormal = Vector3.zero;
    }

    #endregion

    #region Colllision

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        bool didStick = AttemptStick(collision);
        if (AttemptStick(collision))
            return;

        int layer = collision.gameObject.layer;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(Vector3.up, normal);

            if (CollisionIsClimbable(upDot, layer))
                UpdateStateForClimb(normal);
            else if (CollisionIsGround(upDot))
                UpdateStateForGround(normal);
        }
    }

    private bool AttemptStick(Collision collision)
    {
        bool collisionIsGround = collision.gameObject.layer == 6;
        if (collisionIsGround)
            return false;

        Transform collisionTransform = collision.transform;
        StickyProp prop = collisionTransform.GetComponent<StickyProp>();
        bool didStick = false;
        if (prop && prop.CanBeAbsorbed(this))
        {
            StickProp(prop);
            didStick = true;
        }
        else if (ShouldDetachRandomProp(collision))
            DetachRandomProp();

        return didStick;
    }

    private void StickProp(StickyProp prop)
    {
        // TODO:
        // - [ ] Add to the mass
        // - [ ] Add to radius

        prop.Stick(this);
    }

    private bool ShouldDetachRandomProp(Collision collision)
    {
        // TODO:
        // - [ ] Get velocity of collision
        // - [ ] Returns its comparative value against some threshold
        return false;
    }

    private void DetachRandomProp()
    {
        // TODO:
    }

    private void UpdateStateForGround(Vector3 collisionNormal)
    {
        groundContactCount += 1;
        contactNormal += collisionNormal;
    }

    private void UpdateStateForClimb(Vector3 collisionNormal)
    {
        climbContactCount += 1;
        climbNormal += collisionNormal;
        lastClimbNormal = collisionNormal;
    }

    private bool CollisionIsClimbable(float collisionUpDot, int collisionLayer)
    {
        // TODO:
        // - [ ] Check yVelocity here to make sure katamari
        //       can't snap to climb while falling.
        return collisionUpDot >= minClimbDotProduct && (climbMask & (1 << collisionLayer)) != 0;
    }

    private bool CollisionIsGround(float collisionUpDot)
    {
        return collisionUpDot >= minGroundDotProduct;
    }

    #endregion
}
