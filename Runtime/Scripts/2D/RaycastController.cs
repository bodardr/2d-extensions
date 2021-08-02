using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    private const float RAY_DIST = 0.25f;
    protected const float SHELL_RADIUS = 0.01f;

    private BoxCollider2D boxCollider;

    protected int horizontalRayCount;

    /// <summary>
    /// Spacing between rays :
    /// x spacing is for vertical rays (vertical movement).
    /// y spacing is for horizontal rays (horizontal movement).
    /// </summary>
    protected Vector2 raySpacing;

    protected int verticalRayCount;

    public RaycastLocations RaycastPositions { get; } = new RaycastLocations();

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        SetupRaycastInfo();
    }

    public void UpdateRaycastPositions()
    {
        var bounds = boxCollider.bounds;
        bounds.Expand(SHELL_RADIUS * -2);

        RaycastPositions.bottomLeft = bounds.min;
        RaycastPositions.topRight = bounds.max;
        RaycastPositions.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        RaycastPositions.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    }

    private void SetupRaycastInfo()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(SHELL_RADIUS * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        verticalRayCount = Mathf.RoundToInt(boundsWidth / RAY_DIST);
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / RAY_DIST);

        raySpacing = new Vector2(boundsWidth / (verticalRayCount - 1), boundsHeight / (horizontalRayCount - 1));
    }

    public class RaycastLocations
    {
        public Vector2 topLeft,
            topRight,
            bottomLeft,
            bottomRight;
    }
}