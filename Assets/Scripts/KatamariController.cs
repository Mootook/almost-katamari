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
    private Transform playerInputSpace;

    public float size = 1.0f;

    [Header("Movement")]
    [Tooltip("How much force to apply to katamari")]
    public float pushForce = 10.0f;
    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [Header("Climbing")]
    public float maxClimbSpeed = 500.0f;
    public float climbDelta = 2.0f;

    [SerializeField, Range(90, 180)]
    [Tooltip("The maximum angle of object that can be climb.")]
    float maxClimbAngle = 140f;

    [SerializeField, Range(0, 90)]
    [Tooltip("The maximum angle between an object and input to be considered climbing (lower = more difficult)")]
    public float maxInputClimbAngle = 25f;

    [SerializeField]
    [Tooltip("Which layers to allow climbing?")]
    LayerMask climbMask = -1;

    private Vector3 velocity;
    private Rigidbody connectedBody, previousConnectedBody;
    private Vector3 contactNormal, climbNormal, lastClimbNormal;
    private int groundContactCount, climbContactCount;
    private float minGroundDotProduct, minClimbDotProduct, minClimbInputDot;

    private bool OnGround => groundContactCount > 0;
    private bool Climbing => climbContactCount > 0 && Vector3.Dot(lastClimbNormal, input.nextForce) < minClimbInputDot;
    public Vector3 Center => rigidBody.worldCenterOfMass;
    public float Radius => sphereCollider.radius;
    private float maxClimbSpeedWithMass => maxClimbSpeed * rigidBody.mass;

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
        playerInputSpace = Camera.main.transform;

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
        rotationY += input.nextForce.y * Time.deltaTime * 80.0f;
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

        Vector3 forward = new Vector3(0, rotationY, 0);
        Vector3 torque = new Vector3(forwardInput * 500, input.nextForce.y * 1500, -lateralInput * 1500);

        rigidBody.AddTorque(Quaternion.Euler(forward) * torque);
    }

    private void ApplyInputForce()
    {
        float lateralInput = input.nextForce.x;
        float forwardInput = input.nextForce.z;

        Vector3 forward = new Vector3(0, rotationY, 0);
        Vector3 force = new Vector3(lateralInput * 100, ClimbingForce(), forwardInput * 100);

        rigidBody.AddForce(Quaternion.Euler(forward) * force);
    }


    float ClimbingForce()
    {
        float upwardVelocity = 0;
        if (Climbing)
        {
            katamariRenderer.material.SetColor("_Color", Color.red);
            upwardVelocity += 10 * rigidBody.mass;
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
        // connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
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
        int layer = collision.gameObject.layer;
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(Vector3.up, normal);
            if (upDot >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else if (
              upDot >= minClimbDotProduct &&
              (climbMask & (1 << layer)) != 0
          )
            {
                climbContactCount += 1;
                climbNormal += normal;
                lastClimbNormal = normal;
                connectedBody = collision.rigidbody;
            }
        }
    }

    #endregion

    bool CheckClimbing()
    {
        if (Climbing)
        {
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(Vector3.up, climbNormal);
                if (upDot >= minGroundDotProduct)
                {
                    climbNormal = lastClimbNormal;
                }
            }
            groundContactCount = 1;
            contactNormal = climbNormal;
            return true;
        }
        return false;
    }

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        GUI.Label(new Rect(0, 80, 100, 100), "Katamari Size: " + size, red);
    }

    public void OnPropPickup(StickyProp prop)
    {
        Expand(prop.size);
        stuckProps.Add(prop);
    }

    public void OnRejectedCollision()
    {
        //_rb.AddForce(Vector3.up * 50.0f);
    }

    public void Expand(float s)
    {
        sphereCollider.radius += 0.002f;
        // add the newly picked up object's
        // size to the katamari's
        size += s * 0.002f;
        // check size threshold here
    }

    // DEPRECATED
    private void UpdateVelocity()
    {
        // {
        //     float accel = pushForce * Time.deltaTime;
        //     velocity = rigidBody.velocity;
        //     Vector3 desiredVelocity = input.nextForce * pushForce;
        //     if (desiredVelocity != Vector3.zero)
        //     {
        //         velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, accel);
        //         velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, accel);
        //     }
        //     // TODO:
        //     // - [ ] If climbing, should update rotation about
        //     //       axis perpendicular to the normal?
        //     if (Climbing)
        //         velocity.y = Mathf.MoveTowards(velocity.y, maxClimbSpeed, climbDelta * Time.deltaTime);
        //     else
        //         velocity.y += gravity * Time.deltaTime;
    }
}
