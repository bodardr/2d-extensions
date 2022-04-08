using System.Collections;
using Bodardr.UI.Runtime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionCanvas : DontDestroyOnLoad<TransitionCanvas>
{
    private const string TRANSITION_CANVAS = "TRANSITION CANVAS";
    private static Image transitionImage;

    private void Awake()
    {
        gameObject.name = TRANSITION_CANVAS;

        var canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 99;

        gameObject.AddComponent<GraphicRaycaster>();

        var image = Instantiate(new GameObject("Image", typeof(RectTransform), typeof(Image)), transform);
        var rectTransform = image.GetComponent<RectTransform>();

        //Stretch
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;

        transitionImage = image.GetComponent<Image>();
        transitionImage.color = Color.clear;
        
        gameObject.SetActive(false);
    }

    public static IEnumerator FadeOut(float duration = 1)
    {
        Instance.gameObject.SetActive(true);
        
        yield return DOTween.ToAlpha(() => transitionImage.color, value => transitionImage.color = value, 1, duration)
            .From(0).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();
    }

    public static IEnumerator FadeIn(float duration = 1)
    {
        yield return DOTween.ToAlpha(() => transitionImage.color, value => transitionImage.color = value, 0, duration)
            .From(1).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();

        Instance.gameObject.SetActive(false);
    }
}