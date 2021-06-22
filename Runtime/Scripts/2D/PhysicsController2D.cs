using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController2D : RaycastController
{
    private const float MIN_MOVE_DISTANCE = 0.01f;
    private const float NORMAL_SLOPE_TOLERANCE = 0.65f;

    [SerializeField]
    private Vector2 velocity;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool isKinematic;

    [Header("Friction")]
    [Range(0, 1)]
    [SerializeField]
    private float groundFriction = 0.25f;

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
    private float horizontalAcceleration = 10f;

    private bool facingRight = true;
    private bool grounded;

    private Vector2 inputVector;

    private bool onWall;

    private List<InputOverridable> overridables = new List<InputOverridable>(3);

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
        set => facingRight = value;
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

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        Debug.Log(deltaTime);
        velocity = Move(velocity, deltaTime) / deltaTime;

        Debug.Log($"Velocity : {velocity}");
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
    }

    private Vector2 Move(Vector2 displacement, float deltaTime = 1)
    {
        deltaTime = Mathf.Max(Mathf.Epsilon, deltaTime);

        displacement *= deltaTime;

        var actualMovement = new Vector2(MoveHorizontal(displacement.x)
            , MoveVertical(displacement.y));

        if (Mathf.Abs(actualMovement.x) > MIN_MOVE_DISTANCE)
            FacingRight = actualMovement.x > 0;

        rb.MovePosition(Position + actualMovement);

        return actualMovement;
    }

    private float MoveHorizontal(float deltaX)
    {
        var onWallThisFrame = false;

        var moveX = deltaX;
        float directionX = FacingRight ? 1 : -1;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var rayOrigin = directionX > 0 ? RaycastPositions.bottomRight : RaycastPositions.bottomLeft;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            Debug.DrawLine(rayOrigin, rayOrigin + Vector2.right * (directionX * (Mathf.Abs(moveX) + SHELL_RADIUS)));
            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, Mathf.Abs(moveX) + SHELL_RADIUS,
                groundMask);

            if (hit)
            {
                if (hit.distance - SHELL_RADIUS < MIN_MOVE_DISTANCE)
                {
                    moveX = (hit.distance - SHELL_RADIUS) * directionX;
                    wallFromRight = hit.normal.x < 0;
                    onWallThisFrame = true;
                    break;
                }

                moveX -= moveX - (hit.distance - SHELL_RADIUS) * directionX;
            }
        }

        if (Mathf.Abs(moveX) < MIN_MOVE_DISTANCE)
            moveX = 0;

        OnWall = onWallThisFrame;

        return moveX;
    }

    private float MoveVertical(float movementY)
    {
        var groundedThisFrame = false;

        var moveY = movementY;
        float directionY = Mathf.Sign(moveY);

        var rayOrigin = directionY > 0 ? RaycastPositions.topLeft : RaycastPositions.bottomLeft;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Debug.DrawLine(rayOrigin, rayOrigin + Vector2.up * (moveY + SHELL_RADIUS));
            var hit = Physics2D.Raycast(rayOrigin, new Vector2(0, directionY), Mathf.Abs(moveY) + SHELL_RADIUS,
                groundMask);

            if (hit)
            {
                if (hit.normal.y > NORMAL_SLOPE_TOLERANCE)
                    groundedThisFrame = true;

                if (hit.distance - SHELL_RADIUS < MIN_MOVE_DISTANCE)
                {
                    moveY = 0;
                    break;
                }

                moveY -= moveY - (hit.distance - SHELL_RADIUS) * directionY;
            }

            rayOrigin += Vector2.right * (verticalRaySpacing * i);
        }

        if (Mathf.Abs(moveY) < MIN_MOVE_DISTANCE)
            moveY = 0;

        Grounded = groundedThisFrame;

        return moveY;
    }

    private void ApplyGravity()
    {
        velocity += Physics2D.gravity * Time.fixedDeltaTime;
    }

    private void ApplyHorizontalMovement()
    {
        velocity.x += horizontalAcceleration * Time.fixedDeltaTime * inputVector.x;
    }

    private void ApplyFriction()
    {
        velocity.x -= velocity.x * (Grounded ? groundFriction : airFriction) * Time.fixedDeltaTime;
    }

    private void ApplyVelocityDampening()
    {
        velocity.x = Mathf.Clamp(velocity.x, -maxHorizontalVelocity, maxHorizontalVelocity);
        velocity.y = Mathf.Clamp(velocity.y, YVelocityRange.x, YVelocityRange.y);
    }

    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    public void OverrideControl(InputOverridable overridable, bool removeOtherOverridables = false)
    {
        if (removeOtherOverridables)
        {
            overridables.ForEach(x => x.OnControlRegained());
            overridables.Clear();
        }

        overridables.Add(overridable);
    }

    public void RegainControl(InputOverridable overridable)
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