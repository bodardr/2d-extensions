using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MouseToViewportProcessor : InputProcessor<Vector2>
{
    private readonly Vector2 half = new Vector2(0.5f, 0.5f);
    private readonly Vector2 display = new Vector2(Display.main.systemWidth, Display.main.systemHeight);

#if UNITY_EDITOR
    static MouseToViewportProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<MouseToViewportProcessor>();
    }

    public override Vector2 Process(Vector2 value, InputControl control)
    {
        return value / display - half;
    }
}