using UnityEngine;

public static class MathExtensions
{
    public static float Remap(float value, float minA, float maxA, float minB, float maxB)
    {
        return Mathf.Lerp(minB, maxB, Mathf.InverseLerp(minA, maxA, value));
    }

    public static Bounds PointsToBounds(Vector2 firstPoint, Vector2 lastPoint)
    {
        Vector2 average = (firstPoint + lastPoint) / 2;
        Vector2 size = (lastPoint - firstPoint);
        size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        return new Bounds(average, size);
    }

    public static BoundsInt PointsToBounds(Vector2Int firstPoint, Vector2Int lastPoint)
    {
        Vector2 delta = lastPoint - firstPoint;
        Vector2Int size = new Vector2Int((int) Mathf.Abs(delta.x), (int) Mathf.Abs(delta.y));

        var min = Vector2Int.Min(firstPoint, lastPoint);
        return new BoundsInt(min.x, min.y, 0, size.x, size.y, 0);
    }

    public static bool Intersection(BoundsInt a, BoundsInt b, out BoundsInt intersection)
    {
        var min = new Vector2Int(Mathf.Max(a.min.x, b.min.x), Mathf.Max(a.min.y, b.min.y));
        var max = new Vector2Int(Mathf.Min(a.max.x, b.max.x), Mathf.Min(a.max.y, b.max.y));

        var size = max - min;

        intersection = new BoundsInt(min.x, min.y, 0, size.x, size.y, 0);

        return !(min.x >= max.x || min.y >= max.y);
    }

    public static float RandomValueWithinRange(this Vector2 range) => Random.Range(range.x, range.y);

    public static float RandomValueWithinRange(this Vector2Int range) => Random.Range(range.x, range.y);

    public static float FindTopDownAngle(Vector3 delta)
    {
        var vector = Vector3.ProjectOnPlane(delta.normalized,Vector3.up );
        return Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;
    }

    public static Vector3 GetTopDownVectorForAngle(float angle)
    {
        var rad = angle * Mathf.Deg2Rad;
        
        var x = Mathf.Cos(rad);
        var z = Mathf.Sin(rad);

        return new Vector3(x, 0, z);
    }

    public static float GetJumpForce(float jumpHeight)
    {
        return Mathf.Sqrt(-2 * Physics2D.gravity.y * jumpHeight);
    }

    /// <summary>Rotates a vector along degrees.</summary>
    /// <see>
    ///     <see>https://answers.unity.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html</see>
    /// </see>
    public static Vector2 Rotated(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector2 ToBinormal(this Vector2 v)
    {
        return new Vector2(v.y, -v.x);
    }

    public static Vector3 ClampToBounds(this Vector3 pos, Bounds bounds)
    {
        var clampedPos = Clamp(pos, bounds.min, bounds.max);
        return Vector3Int.RoundToInt(clampedPos);
    }

    public static Vector3 ClampToBounds(this Bounds bounds, Vector3 pos)
    {
        var clampedPos = Clamp(pos, bounds.min, bounds.max);
        return Vector3Int.RoundToInt(clampedPos);
    }

    public static Vector3Int ClampToBoundsInt(this Vector3Int pos, BoundsInt bounds)
    {
        pos.Clamp(bounds.min, bounds.max);
        return pos;
    }

    public static Vector3Int ClampToBoundsInt(this BoundsInt bounds, Vector3Int pos)
    {
        pos.Clamp(bounds.min, bounds.max);
        return pos;
    }

    private static Vector3 Clamp(Vector3 pos, Vector3 boundsMIN, Vector3 boundsMAX)
    {
        pos.x = Mathf.Clamp(pos.x, boundsMIN.x, boundsMAX.x);
        pos.y = Mathf.Clamp(pos.y, boundsMIN.y, boundsMAX.y);
        pos.z = Mathf.Clamp(pos.z, boundsMIN.z, boundsMAX.z);

        return pos;
    }

    public static bool IsSuperiorOrEqual(Vector3Int value, Vector3Int against)
    {
        var delta = value - against;
        return delta.x >= 0 && delta.y >= 0 && delta.z >= 0;
    }

    public static bool IsSuperior(Vector3Int value, Vector3Int against)
    {
        var delta = value - against;
        return delta.x > 0 && delta.y > 0 && delta.z > 0;
    }

    public static bool IsSuperiorOrEqual(Vector2Int value, Vector2Int against)
    {
        var delta = value - against;
        return delta.x >= 0 && delta.y >= 0;
    }

    public static bool IsSuperior(Vector2Int value, Vector2Int against)
    {
        var delta = value - against;
        return delta.x > 0 && delta.y > 0;
    }

    public static bool Contains(BoundsInt bounds, Vector3Int point)
    {
        return IsSuperiorOrEqual(point, bounds.min) && IsSuperior(bounds.max, point);
    }

    public static bool Contains(BoundsInt bounds, Vector2Int point)
    {
        return IsSuperiorOrEqual(point, (Vector2Int) bounds.min) && IsSuperior((Vector2Int) bounds.max, point);
    }
}