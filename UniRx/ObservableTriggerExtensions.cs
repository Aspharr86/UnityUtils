using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bubu.UnityUtils.UniRx
{
    public static class ObservableTriggerExtensions
    {
        public static IObservable<Unit> OnPointerHoldAsObservable(this UIBehaviour component, float thresholdTime)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            var trigger = GetOrAddComponent<ObservablePointerHoldTrigger>(component.gameObject);
            trigger.ThresholdTime = thresholdTime;
            return GetOrAddComponent<ObservablePointerHoldTrigger>(component.gameObject).OnPointerHoldAsObservable();
        }

        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}
