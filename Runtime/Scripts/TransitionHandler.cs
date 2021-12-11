using System;
using System.Collections;
using Bodardr.UI.Runtime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


    public class TransitionHandler : DontDestroyOnLoad<TransitionHandler>
    {
        private const string TRANSITION_CANVAS = "TRANSITION CANVAS";

        private Image transitionImage;
        
        private void Awake()
        {
            gameObject.name = TRANSITION_CANVAS;
            
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var image = Instantiate(new GameObject("Image", typeof(RectTransform), typeof(Image)), transform);
            var rectTransform = image.GetComponent<RectTransform>();
            
            //Stretch
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            
            transitionImage = image.GetComponent<Image>();
            transitionImage.color = Color.clear;
        }

        public void ChangeScene(string sceneName, Action loadCallback = null, float easeDuration = 1f)
        {
            StartCoroutine(ChangeSceneCoroutine(sceneName, loadCallback, easeDuration));
        }

        public IEnumerator ChangeSceneCoroutine(string sceneName, Action loadCallback = null, float easeDuration = 1f)
        {
            yield return DOTween.ToAlpha(() => transitionImage.color, value => transitionImage.color = value, 1, easeDuration)
                .From(0).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();

            yield return SceneManager.LoadSceneAsync(sceneName);
            
            loadCallback?.Invoke();
            
            yield return DOTween.ToAlpha(() => transitionImage.color, value => transitionImage.color = value, 0, easeDuration)
                .From(1).SetEase(Ease.InOutSine).SetUpdate(true).WaitForCompletion();
        }
    }
