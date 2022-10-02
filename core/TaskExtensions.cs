using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Altimit
{
    public class TaskExtensions
    {
        public static async Task<object> DynamicTask<T>(Task task)
        {
            var newTask = task as Task<T>;
            return await newTask;
        }
    }
}
