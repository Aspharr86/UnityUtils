using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bubu.UnityUtils.UniRx
{
    [DisallowMultipleComponent]
    public class ObservablePointerHoldTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("The threshold time in seconds to trigger OnPointerHold.")]
        [Min(0f)][SerializeField] private float thresholdTime;
        public float ThresholdTime
        {
            get => thresholdTime;
            set => thresholdTime = Mathf.Max(0f, value);
        }

        private bool isHolding;
        private float elapsedTime;
        private IDisposable disposable;

        Subject<Unit> onPointerHold;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            elapsedTime = 0f;

            disposable = Observable.EveryUpdate()
            .TakeUntilDisable(this)
            .Subscribe(OnPointerDownUpdate);
        }

        private void OnPointerDownUpdate(long _)
        {
            if (isHolding)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= ThresholdTime)
                {
                    if (onPointerHold != null) onPointerHold.OnNext(Unit.Default);
                    isHolding = false;
                    elapsedTime = 0f;
                    disposable?.Dispose();
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            elapsedTime = 0f;
            disposable?.Dispose();
        }

        public IObservable<Unit> OnPointerHoldAsObservable()
        {
            return onPointerHold ?? (onPointerHold = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            if (onPointerHold != null)
            {
                onPointerHold.OnCompleted();
            }
            disposable?.Dispose();
        }
    }
}
