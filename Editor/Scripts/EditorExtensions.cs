using UnityEditor;
using UnityEngine;

public static class EditorExtensions
{
    public static Vector2 MouseTo2DFlatPos(Vector2 mousePosition)
    {
        var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane plane = new Plane(Vector3.back, Vector3.zero);
        plane.Raycast(ray, out var dist);
        return ray.GetPoint(dist);
    }
}