using UnityEngine;

public class ScreenSpaceUI : MonoBehaviour
{
    private Canvas parentCanvas;
    private RectTransform rectTransform;
    private new Camera camera;

    [SerializeField]
    private Transform target = null;

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
            camera.WorldToScreenPoint(target.position),
            parentCanvas.renderMode == RenderMode.ScreenSpaceCamera ? camera : null, out var position);

        rectTransform.position = position;
    }
}
