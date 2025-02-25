using System;
using System.Collections.Generic;

namespace Bubu
{
    public class ObjectPool<T> : IDisposable where T : class
    {
        private Queue<T> items;
        private Func<T> createFunc;
        private Action<T> returnAction;
        private Action<T> disposeAction;

        public ObjectPool(Func<T> createFunc, Action<T> returnAction = null, Action<T> disposeAction = null)
        {
            items = new Queue<T>();
            this.createFunc = createFunc;
            this.returnAction = returnAction;
            this.disposeAction = disposeAction;
        }

        public T Get()
        {
            return items.Count > 0 ? items.Dequeue() : createFunc.Invoke();
        }

        public void Return(T item)
        {
            returnAction?.Invoke(item);
            items.Enqueue(item);
        }

        public void Clear()
        {
            foreach (var item in items)
                disposeAction?.Invoke(item);

            items.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
