using System;

namespace Altimit
{
    [AType]
    public struct Response<T>
    {
        [AProperty]
        public T Value;
        [AProperty]
        public string Error;

        public Response(T result, string error = null) {
            Value = result;
            Error = error;
        }

        public bool IsError()
        {
            return !string.IsNullOrEmpty(Error);
        }

        public void ThrowIfError()
        {
            if (IsError())
                OS.Logger.LogError(Error);
        }
    }
}
