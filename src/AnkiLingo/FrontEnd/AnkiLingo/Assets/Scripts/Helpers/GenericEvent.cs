using System;

namespace Assets.Scripts.Helpers
{
    public class GenericEvent<T>
    {
        public event EventHandler<T> OnEventCalled;
        public void CallEvent(T param) => OnEventCalled?.Invoke(this, param);
    }
}
