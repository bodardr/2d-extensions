using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextMeshProLinkClicker : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var index = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);

        if (index < 0)
            return;
        
        var link = text.textInfo.linkInfo[index];
        Application.OpenURL(link.GetLinkID());
    }
}
