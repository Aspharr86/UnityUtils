using UniRx;
using UnityEngine;

namespace Bubu.UnityUtils.UI
{
    public class ClickEffectPlayer : MonoBehaviour
    {
        [SerializeField] private RectTransform clickEffectPrefab;

        private RectTransform rectTransform;

        private ObjectPool<RectTransform> clickEffectPool;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            clickEffectPool = new ObjectPool<RectTransform>(CreateClickEffect, ReturnClickEffect, null);

            Observable.EveryUpdate()
            .Where(OnPointerDown)
            .Subscribe(PlayClickEffect)
            .AddTo(this);
        }

        private RectTransform CreateClickEffect()
        {
            RectTransform effect = Instantiate(clickEffectPrefab, rectTransform);
            return effect;
        }

        private void ReturnClickEffect(RectTransform effect)
        {
            effect.gameObject.SetActive(false);
        }

        private bool OnPointerDown(long _)
        {
            return Input.GetMouseButtonDown(0);
        }

        private void PlayClickEffect(long _)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 localPoint);

            var effect = clickEffectPool.Get();
            effect.anchoredPosition = localPoint;
            effect.transform.SetAsLastSibling();
            effect.gameObject.SetActive(true);

            Observable.Timer(System.TimeSpan.FromSeconds(0.5))
            .Subscribe(_ => OnClickEffectStop(effect))
            .AddTo(this);
        }

        private void OnClickEffectStop(RectTransform effect)
        {
            clickEffectPool.Return(effect);
        }
    }
}
