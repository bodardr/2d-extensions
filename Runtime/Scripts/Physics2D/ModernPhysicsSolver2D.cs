using UnityEngine;

public class ModernPhysicsSolver2D : PhysicsSolver2D
{
    private ContactFilter2D contactFilter2D;
    private RaycastHit2D[] hitArray = new RaycastHit2D[15];
    private Collider2D[] colliderArray = new Collider2D[10];
    
    protected override void Awake()
    {
        base.Awake();
        contactFilter2D = new ContactFilter2D { layerMask = groundMask, useLayerMask = true };
    }

    public override Vector2 Move(Vector2 displacement, float deltaTime = 1F)
    {
        var initialPos = Position;

        if (deltaTime < Mathf.Epsilon)
            return Vector2.zero;

        Vector2 delta = displacement;
        delta *= deltaTime;

        var deltaMagnitude = delta.magnitude;
        
        if(deltaMagnitude < MIN_MOVE_DISTANCE)
            return Vector2.zero;
        
        var deltaDir = delta.normalized;
        var num = rb.Cast(deltaDir, contactFilter2D, hitArray, deltaMagnitude);
        
        float minDist = deltaMagnitude;

        rb.position += deltaDir * minDist;

        num = rb.OverlapCollider(contactFilter2D, colliderArray);

        for (int i = 0; i < num; i++)
        {
            var dist = rb.Distance(colliderArray[i]);

            var otherRb = colliderArray[i].attachedRigidbody;
            
            if (otherRb && otherRb.bodyType != RigidbodyType2D.Static)
            {
                otherRb.AddForce(dist.normal * minDist * rb.mass, ForceMode2D.Impulse);
            }

            if (dist.isValid && dist.isOverlapped)
            {
                Debug.DrawLine(dist.pointA, dist.pointB, Color.yellow, 0.5f);
                rb.position += dist.normal * dist.distance;
            }
        }
        
        var groundedThisFrame = false;
        var walledThisFrame = false;

        var displacementX = Vector2.Dot(delta, Vector2.right);
        num = rb.Cast(Vector2.right, contactFilter2D, hitArray, Mathf.Sign(displacementX) * SHELL_RADIUS);

        for (int i = 0; i < num; i++)
        {
            var hit = hitArray[i];

            if (Mathf.Abs(Vector2.Dot(hit.normal, Vector2.right)) > 0.95f)
            {
                walledThisFrame = true;
                wallFromRight = hit.normal.x < 0;
                break;
            }
        }

        num = rb.Cast(-transform.up, contactFilter2D, hitArray, SHELL_RADIUS * 2);

        for (int i = 0; i < num; i++)
        {
            var hit = hitArray[i];

            if (hit.normal.y > NORMAL_SLOPE_TOLERANCE)
            {
                groundedThisFrame = true;
                GroundNormal = hit.normal;

                if (hit.collider.sharedMaterial)
                    GroundFriction = hit.collider.sharedMaterial.friction;
                else if (hit.rigidbody && hit.rigidbody.sharedMaterial)
                    GroundFriction = hit.rigidbody.sharedMaterial.friction;

                break;
            }
        }

        Grounded = groundedThisFrame;
        OnWall = walledThisFrame;

        var resolvedDisplacement = Position - initialPos;
        Debug.DrawLine(initialPos, Position, Color.blue, 1);

        if (!Mathf.Approximately(resolvedDisplacement.x, 0))
            FacingRight = resolvedDisplacement.x > 0;

        return resolvedDisplacement;
    }
}