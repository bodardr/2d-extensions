using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Physics2D/2D Controller/Physics Controller 2D", 0)]
[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController2D : RaycastController
{
    private const float MIN_MOVE_DISTANCE = 0.001f;
    private const float NORMAL_SLOPE_TOLERANCE = 0.65f;

    [SerializeField]
    private float currentAccelerationTime;

    [SerializeField]
    private Vector2 velocity;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool isKinematic;

    [Header("Friction")]
    [SerializeField]
    [Range(0, 1)]
    private float airFriction = 0.15f;

    [Header("Velocity Dampening")]
    [SerializeField]
    private float maxHorizontalVelocity = 12;

    [SerializeField]
    private Vector2 yVelocityRange = new Vector2(-15, 15);

    [Header("Acceleration")]
    [SerializeField]
    private AnimationCurve accelerationCurve;

    [SerializeField]
    private float airHorizontalAcceleration = 10f;

    [Header("WIP - Pixel Snapping")]
    [SerializeField]
    private bool pixelSnapping = false;

    [SerializeField]
    private int pixelsPerUnit = 16;

    private bool facingRight = true;

    private bool grounded;
    private float groundFriction = 0.2f;

    private Vector2 inputVector;
    private bool onWall;

    private readonly List<IInputOverridable> overridables = new List<IInputOverridable>(3);

    private Rigidbody2D rb;
    private bool wallFromRight;

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Vector2 Position
    {
        get => rb.position;
        set
        {
            var delta = value - Position;

            UpdateRaycastPositions();

            Move(delta);
        }
    }

    public bool Grounded
    {
        get => grounded;
        set
        {
            if (value == grounded)
                return;

            grounded = value;

            BroadcastMessage(grounded ? "OnGroundEnter" : "OnGroundExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    public bool OnWall
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

    public bool IsKinematic
    {
        get => isKinematic;
        set => isKinematic = value;
    }

    private bool IsInputOverriden => overridables.Count > 0;

    public bool FacingRight
    {
        get => facingRight;
        private set => facingRight = value;
    }

    public Vector2 YVelocityRange
    {
        get => yVelocityRange;
        set => yVelocityRange = value;
    }

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (IsKinematic)
            return;

        UpdateRaycastPositions();

        ApplyGravity();

        if (!IsInputOverriden)
            ApplyHorizontalMovement();

        ApplyFriction();
        ApplyVelocityDampening();

        velocity = Move(velocity, Time.fixedDeltaTime) / Time.fixedDeltaTime;
    }

    private Vector2 Move(Vector2 displacement, float deltaTime = 1)
    {
        deltaTime = Mathf.Max(Mathf.Epsilon, deltaTime);

        displacement *= deltaTime;

        var actualMovement = new Vector2(MoveHorizontal(displacement.x, out var walledThisFrame)
            , MoveVertical(displacement.y, out var groundedThisFrame));

        Grounded = groundedThisFrame;
        OnWall = walledThisFrame;

        if (Mathf.Abs(actualMovement.x) > 0)
            FacingRight = actualMovement.x > 0;

        var newPosition = Position + actualMovement;

        if (pixelSnapping)
        {
            newPosition.x = Mathf.Round(newPosition.x * pixelsPerUnit) / pixelsPerUnit;
            newPosition.y = Mathf.Round(newPosition.y * pixelsPerUnit) / pixelsPerUnit;
        }

        rb.position = newPosition;

        return actualMovement;
    }

    private float MoveHorizontal(float deltaX, out bool onWallThisFrame)
    {
        onWallThisFrame = false;

        var moveX = deltaX;
        float directionX = Mathf.Sign(moveX);

        var rayOrigin = directionX > 0 ? RaycastPositions.bottomRight : RaycastPositions.bottomLeft;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var hit = Physics2D.Raycast(rayOrigin, new Vector2(directionX, 0), Mathf.Abs(moveX) + SHELL_RADIUS,
                groundMask);

            if (hit)
            {
                onWallThisFrame = true;
                wallFromRight = hit.normal.x < 0;

                var otherRb = hit.rigidbody;
                var addedInertia = Vector2.zero;

                if (otherRb && otherRb.bodyType == RigidbodyType2D.Dynamic)
                {
                    //Apply inertia to dynamic Rigidbodies.
                    addedInertia = Vector2.right * (directionX * (hit.distance * rb.mass)) / otherRb.mass;
                    otherRb.velocity += addedInertia / Time.fixedDeltaTime;
                }

                if (hit.distance - SHELL_RADIUS < MIN_MOVE_DISTANCE)
                {
                    return 0;
                }

                moveX = Mathf.Min(Mathf.Abs(moveX),
                    Mathf.Max(0, hit.distance - SHELL_RADIUS + addedInertia.x * directionX)) * directionX;
            }

            Debug.DrawLine(rayOrigin,
                (Vector3) rayOrigin + Vector3.right * SHELL_RADIUS +
                new Vector3(hit.distance - SHELL_RADIUS, 0, 0) * directionX,
                hit ? Color.red : Color.cyan);

            if (Mathf.Abs(moveX) < MIN_MOVE_DISTANCE)
                return 0;

            rayOrigin.y += raySpacing.y;
        }


        OnWall = onWallThisFrame;

        return moveX;
    }

    private float MoveVertical(float movementY, out bool groundedThisFrame)
    {
        groundedThisFrame = false;

        var moveY = movementY;
        float directionY = Mathf.Sign(moveY);

        var rayOrigin = directionY > 0 ? RaycastPositions.topLeft : RaycastPositions.bottomLeft;

        for (int i = 0; i < verticalRayCount; i++)
        {
            var hit = Physics2D.Raycast(rayOrigin, new Vector2(0, directionY), Mathf.Abs(moveY) + SHELL_RADIUS,
                groundMask);

            if (hit)
            {
                if (hit.normal.y > NORMAL_SLOPE_TOLERANCE)
                {
                    groundedThisFrame = true;
                    var groundMat = hit.collider.sharedMaterial;
                    groundFriction = groundMat ? groundMat.friction : 0.2f;
                }

                moveY = Mathf.Min(Mathf.Abs(moveY), Mathf.Max(0, hit.distance - SHELL_RADIUS)) * directionY;

                if (Mathf.Abs(moveY) < MIN_MOVE_DISTANCE)
                    return 0;
            }

            Debug.DrawLine(rayOrigin,
                (Vector3) rayOrigin + Vector3.down * SHELL_RADIUS +
                new Vector3(0, hit.distance - SHELL_RADIUS, 0) * directionY,
                hit ? Color.red : Color.cyan);

            rayOrigin.x += raySpacing.x;
        }

        return moveY;
    }

    private void ApplyGravity()
    {
        velocity += Physics2D.gravity * Time.fixedDeltaTime;
    }

    private void ApplyHorizontalMovement()
    {
        //Update acceleration curve.
        var inputDelta = Mathf.Abs(inputVector.x - currentAccelerationTime);

        if (Mathf.Approximately(inputVector.x, 0))
        {
            currentAccelerationTime = 0;
            return;
        }

        currentAccelerationTime =
            Mathf.Lerp(currentAccelerationTime, inputVector.x, Mathf.Min(inputDelta * 2, 1) * Time.fixedDeltaTime);

        velocity.x +=
            (Grounded ? accelerationCurve.Evaluate(Mathf.Abs(currentAccelerationTime)) : airHorizontalAcceleration) *
            inputVector.x * Time.fixedDeltaTime;
    }

    private void ApplyFriction()
    {
        velocity.x -= velocity.x * (Grounded ? groundFriction : airFriction);
    }

    private void ApplyVelocityDampening()
    {
        velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalVelocity / Time.fixedDeltaTime,
            maxHorizontalVelocity / Time.fixedDeltaTime);
        velocity.y = Mathf.Clamp(velocity.y, YVelocityRange.x / Time.fixedDeltaTime,
            YVelocityRange.y / Time.fixedDeltaTime);
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OverrideControl(IInputOverridable overridable, bool removeOtherOverridables = false)
    {
        if (removeOtherOverridables)
        {
            overridables.ForEach(x => x.OnControlRegained());
            overridables.Clear();
        }

        overridables.Add(overridable);
    }

    public void RegainControl(IInputOverridable overridable)
    {
        if (overridables.Remove(overridable))
            overridable.OnControlRegained();
    }

    private Vector2 SubtractTillZero(Vector2 origin, Vector2 subtraction)
    {
        origin.x = Math.Max(0, origin.x - subtraction.x);
        origin.y = Math.Max(0, origin.y - subtraction.y);

        return origin;
    }
}