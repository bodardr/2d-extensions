using UnityEngine;

public static class MathExtensions
{
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