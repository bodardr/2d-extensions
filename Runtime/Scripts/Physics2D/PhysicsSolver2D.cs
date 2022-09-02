using UnityEngine;

public abstract class PhysicsSolver2D : MonoBehaviour
{
    protected const float MIN_MOVE_DISTANCE = 0.001f;
    protected const float NORMAL_SLOPE_TOLERANCE = 0.25f;
    protected const float SHELL_RADIUS = 0.05f;

    private bool facingRight;

    private bool grounded;

    private bool onWall;
    protected bool wallFromRight;

    private Vector2 groundNormal = Vector2.zero;
    private float groundFriction;

    protected Rigidbody2D rb;

    [SerializeField]
    protected bool isKinematic;

    [SerializeField]
    protected LayerMask groundMask;

    public bool IsKinematic
    {
        get => isKinematic;
        set => isKinematic = value;
    }

    public bool Grounded
    {
        get => grounded;
        protected set
        {
            if (value == grounded)
                return;

            grounded = value;

            BroadcastMessage(grounded ? "OnGroundEnter" : "OnGroundExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    protected bool OnWall
    {
        get => onWall;
        set
        {
            if (value == onWall)
                return;

            onWall = value;

            if (onWall)
                BroadcastMessage("OnWallEnter", wallFromRight, SendMessageOptions.DontRequireReceiver);
            else
                BroadcastMessage("OnWallExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool FacingRight
    {
        get => facingRight;
        protected set => facingRight = value;
    }

    public Vector2 Position
    {
        get => rb.position;
        set
        {
            if (IsKinematic)
                MoveRaw(value);
        }
    }

    public Vector2 GroundNormal
    {
        get => groundNormal;
        protected set => groundNormal = value;
    }

    public float GroundFriction
    {
        get => groundFriction;
        protected set => groundFriction = value;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public abstract Vector2 Move(Vector2 displacement,
        float deltaTime = 1F);

    private void MoveRaw(Vector2 newValue)
    {
        rb.position = newValue;
    }
}