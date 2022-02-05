using DG.Tweening;
using UnityEngine;

[AddComponentMenu("UI/Screen-Space UI")]
public class ScreenSpaceUI : MonoBehaviour
{
    private new Camera camera;

    private Canvas parentCanvas;

    private RectTransform rectTransform;
    private Vector3 screenPosition;

    [SerializeField]
    private UpdateType updateType;

    [SerializeField]
    private Transform target = null;

    [SerializeField]
    private Vector2 viewportOffset;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    public Vector3 ScreenPosition => screenPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        camera = parentCanvas.worldCamera ? parentCanvas.worldCamera : Camera.main;
    }

    private void Update()
    {
        if (updateType != UpdateType.Normal)
            return;

        UpdatePosition();
    }

    private void FixedUpdate()
    {
        if (updateType != UpdateType.Fixed)
            return;

        UpdatePosition();
    }

    private void LateUpdate()
    {
        if (updateType != UpdateType.Late)
            return;

        UpdatePosition();
    }


    public void UpdatePosition()
    {
        if (!target)
            return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform,
            camera.WorldToScreenPoint(target.position) + camera.ViewportToScreenPoint(viewportOffset),
            parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? camera : null, out screenPosition);

        rectTransform.position = screenPosition;
    }

    public void SetOffset(Vector2 newOffset)
    {
        viewportOffset = newOffset;
    }
}