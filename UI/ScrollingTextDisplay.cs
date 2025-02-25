using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Bubu.UnityUtils.UI
{
    public class ScrollingTextDisplay : MonoBehaviour
    {
        [Min(0f)][SerializeField] private float fadeDuration;
        public float FadeDuration
        {
            get => fadeDuration;
            set => fadeDuration = Mathf.Max(0f, value);
        }
        [Min(0f)][SerializeField] private float speed;
        public float Speed
        {
            get => speed;
            set => speed = Mathf.Max(0f, value);
        }
        [Min(0f)][SerializeField] private float interval;
        public float Interval
        {
            get => interval;
            set => interval = Mathf.Max(0f, value);
        }

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private TextMeshProUGUI textMeshProUGUI;

        private Queue<string> textQueue = new Queue<string>();

        public bool IsPlaying { get; private set; }

        private Subject<Unit> onPlayCompleteSubject = new Subject<Unit>();

        public IObservable<Unit> OnPlayCompleteAsObservable => onPlayCompleteSubject.AsObservable();

        private void Awake()
        {
            rectTransform = transform.Find("ScrollingTextDisplay").GetComponent<RectTransform>();
            canvasGroup = rectTransform.Find("CanvasGroup").GetComponent<CanvasGroup>();
            textMeshProUGUI = rectTransform.Find("CanvasGroup/Text (TMP)").GetComponent<TextMeshProUGUI>();
        }

        public void EnqueueText(string text)
        {
            textQueue.Enqueue(text);

            if (!IsPlaying)
            {
                IsPlaying = true;
                BeginPlay();
            }
        }

        private void BeginPlay()
        {
            canvasGroup.DOFade(1f, FadeDuration)
            .OnComplete(Play);
        }

        private void Play()
        {
            string text = textQueue.Dequeue();
            textMeshProUGUI.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(textMeshProUGUI.rectTransform);
            textMeshProUGUI.rectTransform
            .DOAnchorPosX(-(rectTransform.rect.width + textMeshProUGUI.rectTransform.rect.width), Speed)
            .SetDelay(Interval)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .OnComplete(OnPlayComplete);
        }

        private void OnPlayComplete()
        {
            onPlayCompleteSubject.OnNext(Unit.Default);

            textMeshProUGUI.text = string.Empty;
            textMeshProUGUI.rectTransform.anchoredPosition = Vector2.zero;

            if (textQueue.Count > 0)
                Play();
            else
                EndPlay();
        }

        private void EndPlay()
        {
            canvasGroup.DOFade(0f, FadeDuration)
            .OnComplete(OnEndPlayComplete);
        }

        private void OnEndPlayComplete()
        {
            if (textQueue.Count > 0)
                BeginPlay();
            else
                IsPlaying = false;
        }

        private void OnDestroy()
        {
            onPlayCompleteSubject.OnCompleted();
        }
    }
}
