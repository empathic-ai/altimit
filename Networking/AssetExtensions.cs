using Altimit.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Altimit
{
    public static class AssetExtensions
    {
        public static async Task UpdateAsset<T>(this T instance, T value)
        {
            await (instance.GetDB()).UpdateAsset(instance.GetInstanceID(), value);
        }

        public static async Task SetOrUpdate<T, P>(this T instance, Expression<Func<T, AID>> propExp, P value)
        {
            if (instance.GetProperty(propExp).IsEmpty())
            {
                instance.SetInstance(propExp, value);
            } else
            {
                await (instance.GetDB()).UpdateAsset(instance.GetProperty(propExp), value);
            }
        }
    }
}
