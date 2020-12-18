using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsController2D : RaycastController
{
    private const float MIN_MOVE_DISTANCE = 0.01f;
    private const float NORMAL_SLOPE_TOLERANCE = 0.65f;

    private Rigidbody2D rb;

    private Vector2 inputVector;
    private Vector2 velocity;
    private Vector2 position;

    private bool facingRight = true;
    private bool grounded;

    private bool onWall;
    private bool wallFromRight;

    private List<InputOverridable> overridables = new List<InputOverridable>(3);

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private bool isKinematic;

    [Header("Friction")]
    [SerializeField]
    private float groundFriction = 3;

    [SerializeField]
    private float airFriction = 2;

    [Header("Velocity Dampening")]
    [SerializeField]
    private float maxHorizontalVelocity = 12;

    [SerializeField]
    private float maxDownwardsVelocity = -15;

    [SerializeField]
    private float maxUpwardsVelocity = 15;


    [Header("Acceleration")]
    [SerializeField]
    private float horizontalAcceleration = 10f;

    public Vector2 Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    public Vector2 Position
    {
        get => position;
        set
        {
            var delta = value - position;

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

    public float MaxDownwardsVelocity
    {
        get => maxDownwardsVelocity;
        set => maxDownwardsVelocity = value;
    }

    public bool IsKinematic
    {
        get => isKinematic;
        set => isKinematic = value;
    }

    private bool IsInputOverriden => overridables.Count > 0;

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
            ApplyMovement();

        Move(velocity * Time.fixedDeltaTime);

        ApplyFriction();
        ApplyVelocityDampening();
    }

    private void Move(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > MIN_MOVE_DISTANCE)
            facingRight = delta.x > 0;

        MoveHorizontal(delta.x, out var moveX);
        MoveVertical(delta.y, out var moveY);

        var movement = new Vector2(moveX, moveY);
        rb.MovePosition(rb.position + movement);
        position = rb.position + movement;

        if (!isKinematic)
            velocity = movement / Time.fixedDeltaTime;
    }

    private void MoveHorizontal(float delta, out float moveX)
    {
        var onWallThisFrame = false;

        moveX = delta;
        float directionX = facingRight ? 1 : -1;

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
    }

    private void MoveVertical(float delta, out float moveY)
    {
        var groundedThisFrame = false;

        moveY = delta;
        float directionY = Mathf.Sign(moveY);

        for (int i = 0; i < verticalRayCount; i++)
        {
            var rayOrigin = directionY > 0 ? RaycastPositions.topLeft : RaycastPositions.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);

            Debug.DrawLine(rayOrigin, rayOrigin + Vector2.up * (directionY * (Mathf.Abs(moveY) + SHELL_RADIUS)));
            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, Mathf.Abs(moveY) + SHELL_RADIUS,
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
        }

        if (Mathf.Abs(moveY) < MIN_MOVE_DISTANCE)
            moveY = 0;

        Grounded = groundedThisFrame;
    }

    private Vector2 SubtractTillZero(Vector2 origin, Vector2 subtraction)
    {
        origin.x = Math.Min(0, origin.x - subtraction.x);
        origin.y = Math.Min(0, origin.y - subtraction.y);

        return origin;
    }

    private void ApplyGravity()
    {
        Vector2 gravity = Physics2D.gravity;
        velocity = Velocity + gravity * Time.deltaTime;
    }

    private void ApplyMovement()
    {
        velocity.x += horizontalAcceleration * (inputVector.x * Time.fixedDeltaTime);
    }

    private void ApplyFriction()
    {
        velocity.x -= Velocity.x * (Grounded ? groundFriction : airFriction) * Time.fixedDeltaTime;
    }

    private void ApplyVelocityDampening()
    {
        velocity.x = Mathf.Clamp(Velocity.x, -maxHorizontalVelocity, maxHorizontalVelocity);
        velocity.y = Mathf.Clamp(Velocity.y, maxDownwardsVelocity, maxUpwardsVelocity);
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
}