using UnityEngine;

[AddComponentMenu("UI/Screen-Space UI")]
public class ScreenSpaceUI : MonoBehaviour
{
    [SerializeField]
    private Transform target = null;

    [SerializeField]
    private Vector2 offset;

    private new Camera camera;
    private Canvas parentCanvas;
    private RectTransform rectTransform;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        camera = parentCanvas.worldCamera ? parentCanvas.worldCamera : Camera.main;
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform,
            camera.WorldToScreenPoint(target.position) + camera.ViewportToScreenPoint(offset),
            parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? camera : null, out var position);

        rectTransform.position = position;
    }

    public void SetOffset(Vector2 newOffset)
    {
        offset = newOffset;
    }
}