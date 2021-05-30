using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    #region Members
    private KatamariInputController input;
    private SphereCollider sphereCollider;
    private Rigidbody rigidBody;
    private List<StickyProp> stuckProps;

    public static event Action<StickyProp> propPickupEvent;

    public float density = 5.0f;

    public float massAdjustmentMultiplier = 0.25f;

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
    public float climbForceMultiplier = 35.0f;
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
    public float Radius => sphereCollider.radius;
    public Vector3 Center => rigidBody.worldCenterOfMass;
    public float Mass => rigidBody.mass;
    public float AdjustedMass => Mass * massAdjustmentMultiplier;

    private float torqueMultiplierWithMass => torqueMultiplier * rigidBody.mass;
    private float airborneForceMultiplier => forceMultiplier / 2.0f;

    private Renderer katamariRenderer;
    private Color defaultColor;

    public float rotationY;

    public SimpleCameraFollow cameraFollow;

    private float volume;

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

    void Start()
    {
        stuckProps = new List<StickyProp>();

        input = GetComponent<KatamariInputController>();
        rigidBody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        katamariRenderer = GetComponent<Renderer>();
        defaultColor = katamariRenderer.material.GetColor("_Color");

        cameraFollow.SetInitialParameters();

        volume = SphericalVolume();
        rigidBody.mass = SphericalMass();
    }

    private float CalculateRadius()
    {
        return Mathf.Pow((3 * volume) / (4 * Mathf.PI), (1.0f / 3.0f));
    }

    private float SphericalVolume()
    {
        return (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(sphereCollider.radius, 3);
    }

    private float SphericalMass()
    {
        return volume * density;
    }

    private void Update()
    {
        rotationY += input.nextForce.y * Time.deltaTime * rotationMultiplier;
    }

    private void FixedUpdate()
    {
        ApplyInputTorque();
        ApplyInputForce();

        AdjustVerticalVelocity();

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

        rigidBody.AddTorque(Forward() * torque);
    }

    private void ApplyInputForce()
    {
        float lateralInput = input.nextForce.x;
        float forwardInput = input.nextForce.z;

        float forceForContactState = OnGround ? forceMultiplier : airborneForceMultiplier;

        Vector3 force = new Vector3(
            lateralInput * forceForContactState,
            0.0f,
            forwardInput * forceForContactState
        );

        velocity = Forward() * force;
        rigidBody.AddForce(velocity);
    }

    private void AdjustVerticalVelocity()
    {
        Vector3 velocity = rigidBody.velocity;
        float gravity = -9.81f;
        float forceToVelocityFactor = 0.0001f;
        if (Climbing)
            velocity.y = climbForceMultiplier * forceToVelocityFactor;
        else
            velocity.y += gravity * Time.deltaTime;

        rigidBody.velocity = velocity;
    }

    private Quaternion Forward()
    {
        Vector3 forward = new Vector3(0, rotationY, 0);
        return Quaternion.Euler(forward);
    }

    private void ClearState()
    {
        groundContactCount = climbContactCount = 0;
        contactNormal = climbNormal = Vector3.zero;
    }


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
        if (prop && prop.CanBeAbsorbed(AdjustedMass))
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
        rigidBody.mass += prop.Mass;
        volume += prop.volume;
        sphereCollider.radius = CalculateRadius();

        prop.Stick(this);

        if (propPickupEvent != null)
            propPickupEvent(prop);
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

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        GUI.Label(new Rect(0, 0, 100, 100), "Is Climbing: " + Climbing, red);
    }

}
