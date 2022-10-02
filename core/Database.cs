using System;
using System.Collections.Generic;
using System.Linq;

namespace Altimit
{
    /*
    public class Database
    {
        public Dictionary<Type, Globe> globes = new Dictionary<Type, Globe>();
        public Action<Type, object> onAdd;

        public Database()
        {
        }

        public void Add<T>(T o) where T : IID
        {
            GetGlobe<T>().Add(o, o.ID, false);
        }

        public bool Has<T>(int id)
        {
            return GetGlobe<T>().Has(id);
        }

        public T Get<T>(int id)
        {
            return GetGlobe<T>().Get<T>(id, true);
        }

        public int GetID<T>(T o)
        {
            return GetGlobe<T>().GetInstanceID(o);
        }

        public object Update(Type type, object o, bool setFromCopy = true)
        {
            return typeof(Database)
                .GetMethods().Single(x=>x.Name == "Update" && x.IsGenericMethod)
                .MakeGenericMethod(type)
                .Invoke(this, new object[] { o, setFromCopy });
        }

        public T Update<T>(T o, bool setFromCopy = true) where T : IID
        {
            object global;
            if (GetGlobe<T>().TryGet(o.ID, out global))
            {
                if (setFromCopy)
                {
                    o.CopyTo((T)global);
                } else
                {
                    ((T)global).CopyTo(o);
                }
                return (T)global;
            } else
            {
                Add(o);
                return o;
            }
        }

        public Globe GetGlobe<T>()
        {
            Globe globe;
            if (!globes.TryGetValue(typeof(T), out globe))
            {
                globe = new Globe(typeof(T).Name + " Table");
                globe.onGlobalAdded += global=>onAdd?.Invoke(typeof(T), global);
                globes.Add(typeof(T), globe);
            }
            return globe;
        }
    }*/
}
