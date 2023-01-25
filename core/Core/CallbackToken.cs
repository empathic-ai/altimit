using System;

namespace Altimit
{
    public class CallbackToken
    {
        public bool IsCancelled = false;
        public Action onCancelled;

        public CallbackToken()
        {
        }

        public void Cancel()
        {
            IsCancelled = true;
            onCancelled?.Invoke();
        }
    }
}
