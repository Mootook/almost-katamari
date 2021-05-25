using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatamariController : MonoBehaviour
{
    #region Members
    private KatamariInputController _input;
    private SphereCollider _sphereCollider;
    private Rigidbody _rb;
    private List<StickyProp> _stuckProps;
    private Transform _inputSpace;

    public float size = 1.0f;

    [Header("Movement")]
    [Tooltip("How much force to apply to katamari")]
    public float pushForce = 10.0f;
    [SerializeField, Range(0, 90)]
	float maxGroundAngle = 25f;

    [Header("Climbing")]
    public float maxClimbSpeed = 2.0f;
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

    [Header("Misc.")]
    [SerializeField, Range(-20, 0)]
    public float gravity = -9.81f;

    private Vector3 velocity;
    private Rigidbody connectedBody, previousConnectedBody;
    private bool OnGround => groundContactCount > 0;
    private Vector3 contactNormal, climbNormal, lastClimbNormal;
	private int groundContactCount, climbContactCount;
    private float minGroundDotProduct, minClimbDotProduct, minClimbInputDot;

    private bool Climbing => climbContactCount > 0 && Vector3.Dot(lastClimbNormal, _input.nextForce) < minClimbInputDot;
    public Vector3 Center => _rb.worldCenterOfMass;

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

    private void Start()
    {
       _input = GetComponent<KatamariInputController>(); 
       _rb = GetComponent<Rigidbody>();
       _sphereCollider = GetComponent<SphereCollider>();
       _stuckProps = new List<StickyProp>();
       _inputSpace = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
        _rb.velocity = velocity;
        ClearState();
    }

    private void UpdateVelocity()
    {
        float accel = pushForce * Time.deltaTime;
        velocity = _rb.velocity;
        Vector3 desiredVelocity = _input.nextForce * pushForce;
        if (desiredVelocity != Vector3.zero)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, accel);
            velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, accel);
        }
        // TODO:
        // - [ ] If climbing, should update rotation about
        //       axis perpendicular to the normal?
        if (Climbing)
            velocity.y = Mathf.MoveTowards(velocity.y, maxClimbSpeed, climbDelta * Time.deltaTime);
        else
            velocity.y += gravity *  Time.deltaTime;
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
            if (upDot >= minGroundDotProduct) {
				groundContactCount += 1;
				contactNormal += normal;
				connectedBody = collision.rigidbody;
			} else if (
                upDot >= minClimbDotProduct &&
                (climbMask & (1 << layer)) != 0 
            ) {
                climbContactCount += 1;
                climbNormal += normal;
                lastClimbNormal = normal;
                connectedBody = collision.rigidbody;
            }
        }
    }

    #endregion

	bool CheckClimbing () {
        if (Climbing) {
			if (climbContactCount > 1) {
				climbNormal.Normalize();
				float upDot = Vector3.Dot(Vector3.up, climbNormal);
				if (upDot >= minGroundDotProduct) {
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
        GUI.Label(new Rect(0, 80, 100, 100), "Katamari Size: "  + size, red);
    }

    /// <summary>
    /// Update list of props
    /// and expand size of sphere collider.
    /// </summary>
    /// <param name="prop">Prop being stuck to katamari</param>
    public void OnPropPickup(StickyProp prop)
    {
        Expand(prop.size);
        _stuckProps.Add(prop);
    }

    public void OnRejectedCollision()
    {
        //_rb.AddForce(Vector3.up * 50.0f);
    }

    public void Expand(float s)
    {
        _sphereCollider.radius += 0.002f;
        // add the newly picked up object's
        // size to the katamari's
        size += s * 0.002f;
        // check size threshold here
    }
}
