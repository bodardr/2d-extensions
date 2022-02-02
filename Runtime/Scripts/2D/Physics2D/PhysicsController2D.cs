using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

[AddComponentMenu("Physics2D/2D Controller/Physics Controller 2D", 0)]
[RequireComponent(typeof(Rigidbody2D), typeof(PhysicsSolver2D))]
public class PhysicsController2D : MonoBehaviour
{
    private float groundFriction = 0.2f;

    private Vector2 inputVector;
    private Vector2 moveVector;
    private Vector2 velocity;

    private Rigidbody2D rb;
    private PhysicsSolver2D physicsSolver;

    private IInputOverridable overridable;
    private bool IsInputOverriden => overridable != null;

    private float currentFriction => PhysicsSolver.Grounded ? PhysicsSolver.GroundFriction : airFriction;
    
    [SerializeField]
    private float horizontalAcceleration = 100;

    [SerializeField]
    private float drag = 0.005f;

    [SerializeField]
    private Vector2 maxVelocity = new Vector2(20, 25);

    [Header("Friction")]
    [SerializeField]
    [Range(0, 1)]
    private float airFriction = 0.15f;

    [Header("Ground Snapping")]
    [SerializeField]
    private bool groundSnapping = true;

    [SerializeField]
    private float rotationSpeed = 10f;

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public float Drag
    {
        get => drag;
        set => drag = value;
    }

    public PhysicsSolver2D PhysicsSolver => physicsSolver;

    protected void Awake()
    {
        physicsSolver = GetComponent<PhysicsSolver2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (PhysicsSolver.IsKinematic)
            return;

        ApplyGravity();

        if (IsInputOverriden)
            moveVector = overridable.OverrideInputUpdate(moveVector);

        ApplyAcceleration();

        ApplyFriction();

        velocity = PhysicsSolver.Move(velocity, Time.fixedDeltaTime) / Time.fixedDeltaTime;

        if (!groundSnapping)
            return;

        var rot = Quaternion.identity;
        var curRot = transform.rotation;
        if (PhysicsSolver.Grounded)
        {
            var groundNormal = PhysicsSolver.GroundNormal;
            rot = Quaternion.Euler(curRot.eulerAngles.x, curRot.eulerAngles.y, Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg - 90f);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
    }

    private void ApplyGravity()
    {
        var groundNormal = PhysicsSolver.GroundNormal;
        groundNormal.y = -groundNormal.y;
        
        var gravity = (PhysicsSolver.Grounded ? groundNormal : Vector2.down) * Physics2D.gravity.magnitude *
                      Time.fixedDeltaTime;
        velocity += gravity;
    }

    private void ApplyAcceleration()
    {
        var inputForce = (IsInputOverriden ? moveVector.x : inputVector.x) * (horizontalAcceleration * Time.fixedDeltaTime);

        var dir = Vector2.right;

        if (PhysicsSolver.Grounded)
            dir = PhysicsSolver.GroundNormal.ToBinormal();

        velocity += dir * inputForce * currentFriction;
    }

    private void ApplyFriction()
    {
        if (PhysicsSolver.Grounded)
        {
            var dir = PhysicsSolver.GroundNormal.ToBinormal();
            velocity -= Vector2.Scale(velocity, dir * currentFriction);
        }
        else
        {
            velocity.x *= 1 - currentFriction;
        }
        
        if (!PhysicsSolver.Grounded && velocity.y < 0)
            velocity.y *= 1f - Drag;
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OverrideControl(IInputOverridable overridable)
    {
        this.overridable = overridable;
    }

    public void RegainControl()
    {
        overridable = null;
    }

    /// <summary>
    /// Utility Method to 
    /// </summary>
    public void HeadToPosition(Vector2 position)
    {
        if (IsInputOverriden)
        {
            Debug.LogWarning("Input is already overriden");
            return;
        }

        new WaitFor2DControllerArrival(this, position);
    }

    public void HeadToPosition(Transform tr)
    {
        HeadToPosition(tr.position);
    }
}