using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaycastPhysicsSolver2D : PhysicsSolver2D
{
    private const float RAY_DIST = 0.2f;
    
    private Collider2D col2D;
    private Bounds bounds;
    private List<Vector2> allPositions;

    protected int horizontalRayCount;

    /// <summary>
    /// Spacing between rays :
    /// x spacing is for vertical rays (vertical movement).
    /// y spacing is for horizontal rays (horizontal movement).
    /// </summary>
    protected Vector2 raySpacing;

    protected int verticalRayCount;

    [Header("WIP - Pixel Snapping")]
    [SerializeField]
    private bool pixelSnapping = false;
    
    [SerializeField]
    private int pixelsPerUnit = 16;
    
    public RaycastLocations RaycastPositions { get; } = new RaycastLocations();

    protected override void Awake()
    {
        base.Awake();
        col2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        SetupRaycastInfo();
    }

    public void UpdateRaycastPositions()
    {
        var rot = transform.rotation;
        transform.rotation = Quaternion.identity;

        bounds = col2D.bounds;

        var min = bounds.min;
        var max = bounds.max;

        var bottomLeft = min;
        var bottomRight = min;
        bottomRight.x = max.x;

        var topLeft = max;
        var topRight = max;
        topLeft.x = min.x;

        RaycastPositions.bottomRays = new Vector2[verticalRayCount];
        RaycastPositions.topRays = new Vector2[verticalRayCount];
        RaycastPositions.leftRays = new Vector2[horizontalRayCount];
        RaycastPositions.rightRays = new Vector2[horizontalRayCount];

        for (int i = 0; i < verticalRayCount; i++)
        {
            RaycastPositions.bottomRays[i] =
                transform.InverseTransformPoint(
                    col2D.ClosestPoint(Vector3.Lerp(bottomLeft, bottomRight, (float)i / (verticalRayCount - 1))));
            RaycastPositions.topRays[i] =
                transform.InverseTransformPoint(
                    col2D.ClosestPoint(Vector3.Lerp(topLeft, topRight, (float)i / (verticalRayCount - 1))));
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            RaycastPositions.leftRays[i] =
                transform.InverseTransformPoint(
                    col2D.ClosestPoint(Vector3.Lerp(bottomLeft, topLeft, (float)i / (horizontalRayCount - 1))));
            RaycastPositions.rightRays[i] =
                transform.InverseTransformPoint(
                    col2D.ClosestPoint(Vector3.Lerp(bottomRight, topRight, (float)i / (horizontalRayCount - 1))));
        }

        transform.rotation = rot;
        allPositions = RaycastPositions.bottomRays.Concat(RaycastPositions.topRays).Concat(RaycastPositions.leftRays)
            .Concat(RaycastPositions.rightRays).ToList();
    }

    private void SetupRaycastInfo()
    {
        Bounds bounds = col2D.bounds;
        bounds.Expand(SHELL_RADIUS * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        verticalRayCount = Mathf.RoundToInt(boundsWidth / RAY_DIST);
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / RAY_DIST);

        raySpacing = new Vector2(boundsWidth / (verticalRayCount - 1), boundsHeight / (horizontalRayCount - 1));
    }

    public class RaycastLocations
    {
        public Vector2[] topRays;
        public Vector2[] bottomRays;
        public Vector2[] leftRays;
        public Vector2[] rightRays;
    }
    
    public override Vector2 Move(Vector2 displacement, float deltaTime = 1)
    {
        deltaTime = Mathf.Max(Mathf.Epsilon, deltaTime);

        displacement *= deltaTime;

        var actualMovement = new Vector2(MoveHorizontal(displacement.x, out var walledThisFrame)
            , MoveVertical(displacement.y, out var groundedThisFrame, out var groundNormal));


        Grounded = groundedThisFrame;
        OnWall = walledThisFrame;
        
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
        float directionX = moveX > 0 ? 1 : -1;

        if (Mathf.Approximately(moveX, 0))
            directionX = 0;

        var positions = directionX > 0 ? RaycastPositions.rightRays : RaycastPositions.leftRays;
        var direction = transform.right * directionX;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            var ray = new Ray2D(transform.TransformPoint(positions[i]), direction);
            var hit = Physics2D.Raycast(ray.origin, -ray.direction * SHELL_RADIUS, Mathf.Abs(moveX) + SHELL_RADIUS,
                groundMask);

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, hit ? Color.red : Color.cyan);

            if (hit)
            {
                onWallThisFrame = true;
                wallFromRight = hit.normal.x < 0;

                var otherRb = hit.rigidbody;
                var addedInertia = 0f;

                if (otherRb && otherRb.bodyType == RigidbodyType2D.Dynamic)
                {
                    //Apply inertia to dynamic Rigidbodies.
                    addedInertia = (directionX * (hit.distance * rb.mass)) / otherRb.mass;
                    otherRb.velocity += Vector2.right * addedInertia / Time.fixedDeltaTime;
                }

                if (hit.distance <= MIN_MOVE_DISTANCE + SHELL_RADIUS)
                    return 0;

                moveX = Mathf.Min(Mathf.Abs(moveX), Mathf.Max(0, hit.distance + Mathf.Abs(addedInertia))) * directionX;
            }
        }

        OnWall = onWallThisFrame;

        return moveX;
    }

    private float MoveVertical(float movementY, out bool groundedThisFrame, out Vector2 groundNormal)
    {
        groundedThisFrame = false;

        var moveY = movementY;
        float directionY = Mathf.Sign(moveY);

        var positions = directionY > 0 ? RaycastPositions.topRays : RaycastPositions.bottomRays;
        var direction = transform.up * directionY;

        groundNormal = Vector2.zero;

        for (int i = 0; i < verticalRayCount; i++)
        {
            var ray = new Ray2D(transform.TransformPoint(positions[i]), direction);
            var hit = Physics2D.Raycast(ray.origin - ray.direction * SHELL_RADIUS, ray.direction,
                Mathf.Abs(moveY) + SHELL_RADIUS,
                groundMask);

            if (hit)
            {
                if (hit.normal.y > NORMAL_SLOPE_TOLERANCE)
                {
                    groundedThisFrame = true;
                    var groundMat = hit.collider.sharedMaterial;
                    groundNormal = hit.normal;
                }

                if (hit.distance <= MIN_MOVE_DISTANCE + SHELL_RADIUS)
                    return 0;

                moveY = Mathf.Min(Mathf.Abs(moveY), hit.distance) * directionY;
            }

            Debug.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, hit ? Color.red : Color.cyan);
        }

        return moveY;
    }
}