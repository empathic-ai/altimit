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

        public void AddTask(TKey key)
        {
            taskCompletionSourcesByKey.AddOrGet(key);
        }

        public async Task<TValue> AddOrGetTask(TKey key, CancellationToken ct = default) {
            var tcs = taskCompletionSourcesByKey.AddOrGet(key);
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
