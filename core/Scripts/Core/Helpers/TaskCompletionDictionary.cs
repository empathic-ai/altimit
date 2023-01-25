using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit
{
    /*
    public class TaskCompletionHashset<TKey>
    {
        Dictionary<TKey, TaskCompletionSource<object>> taskCompletionSourcesByKey = new Dictionary<TKey, TaskCompletionSource<object>>();


        public void SetResult(TKey key)
        {
            TaskCompletionSource<object> taskCompletionSource;
            if (taskCompletionSourcesByKey.TryGetValue(key, out taskCompletionSource))
            {
                taskCompletionSource.SetResult(null);
                taskCompletionSourcesByKey.Remove(key);
            }
        }

        public Task GetTask(TKey key)
        {
            return taskCompletionSourcesByKey.AddOrGet(key).Task;
        }
    }
    */
    public class TaskCompletionDictionary<TKey, TValue>
    {
        Dictionary<TKey, TaskCompletionSource<TValue>> taskCompletionSourcesByKey = new Dictionary<TKey, TaskCompletionSource<TValue>>();

        public void SetResult(TKey key, TValue value)
        {
            TaskCompletionSource<TValue> taskCompletionSource;
            if (taskCompletionSourcesByKey.TryGetValue(key, out taskCompletionSource))
            {
                taskCompletionSource.SetResult(value);
                taskCompletionSourcesByKey.Remove(key);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return taskCompletionSourcesByKey.ContainsKey(key);
        }

        public TaskCompletionSource<TValue> AddOrGetTaskCompletionSource(TKey key)
        {
            TaskCompletionSource<TValue> taskCompletionSource;
            if (!taskCompletionSourcesByKey.TryGetValue(key, out taskCompletionSource))
            {
                taskCompletionSource = new TaskCompletionSource<TValue>(TaskCreationOptions.RunContinuationsAsynchronously);
                taskCompletionSourcesByKey[key] = taskCompletionSource;
            }
            return taskCompletionSource;
        }

        public async Task<TValue> AddOrGetTask(TKey key, TimeSpan timespan)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CancellationToken ct = cts.Token;
                cts.CancelAfter(timespan);

                return await AddOrGetTask(key, ct);
            } catch (Exception e)
            {
                throw new TimeoutException("Task timed out!", e);
            }
        }

        public async Task<TValue> AddOrGetTask(TKey key, CancellationToken ct = default) {

            var tcs = AddOrGetTaskCompletionSource(key);

            using (ct.Register(() => {
                // this callback will be executed when token is cancelled
                tcs.TrySetCanceled();
            }))
            {
                return await tcs.Task;
            }
        }
    }
}
