using System;
using System.Collections.Generic;
using System.Linq;

/*
namespace Altimit
{
    
    public class InstanceAuhorities
    {
        public Action<object, int, bool> onFlagChanged;
        bool defaultFlag;
        Dictionary<object, InstanceFlags> globalFlaggers = new Dictionary<object, InstanceFlags>();

        public InstanceAuhorities(bool defaultFlag = false)
        {
            this.defaultFlag = defaultFlag;
        }

        public IEnumerable<InstanceFlags> GetAll()
        {
            return globalFlaggers.Select(x=>x.Value);
        }

        public InstanceFlags AddOrGetFlagger(object globalClass)
        {
            InstanceFlags globalAuthority;
            if (!globalFlaggers.TryGetValue(globalClass, out globalAuthority))
            {
                //Debug.Log(global == null);
                globalAuthority = new InstanceFlags(globalClass, defaultFlag);
                globalAuthority.onFlagChanged += onFlagChanged;
                globalFlaggers[globalClass] = globalAuthority;
            }
            return globalAuthority;
        }

        public void RemoveFlagger(object global)
        {
            InstanceFlags globalFlagger;
            if (globalFlaggers.TryGetValue(global, out globalFlagger))
            {
                globalFlagger.onFlagChanged -= onFlagChanged;
                globalFlaggers.Remove(global);
            }
        }

        public void SetFlag(object global, bool flag)
        {
            AddOrGetFlagger(global).SetFlag(flag);
        }

        public void SetFlag(object global, bool flag, params string[] propertyNames)
        {
            var authority = AddOrGetFlagger(global);
            foreach (var propertyName in propertyNames)
                authority.SetFlag(global.GetPropertyInfo(propertyName).ID, flag);
        }

        public void SetFlag(object global, int propertyID, bool flag)
        {
            AddOrGetFlagger(global).SetFlag(propertyID, flag);
        }

        public bool GetFlag(object global, int propertyID)
        {
            return AddOrGetFlagger(global).GetFlag(propertyID);
        }
    }
}*/
   
/*
public class GlobalFlaggers<T>
    {
        public Action<object, int, T> onObserveTypeChanged;
        T defaultObserveType;
        Dictionary<object, GlobalFlagger<T>> globalFlaggers = new Dictionary<object, GlobalFlagger<T>>();

        public GlobalFlaggers()
        {
            this.defaultObserveType = default(T);
        }

        public GlobalFlaggers(T defaultObserveType)
        {
            this.defaultObserveType = defaultObserveType;
        }

        public IEnumerable<GlobalFlagger<T>> GetAll()
        {
            return globalFlaggers.Select(x=>x.Value);
        }

        public GlobalFlagger<T> AddOrGetFlagger(object globalClass)
        {
            GlobalFlagger<T> globalAuthority;
            if (!globalFlaggers.TryGetValue(globalClass, out globalAuthority))
            {
                //Debug.Log(global == null);
                globalAuthority = new GlobalFlagger<T>(globalClass, defaultObserveType);
                globalAuthority.onObserveTypeChanged += onObserveTypeChanged;
                globalFlaggers[globalClass] = globalAuthority;
            }
            return globalAuthority;
        }

        public void RemoveFlagger(object global)
        {
            GlobalFlagger<T> globalFlagger;
            if (globalFlaggers.TryGetValue(global, out globalFlagger))
            {
                globalFlagger.onObserveTypeChanged -= onObserveTypeChanged;
                globalFlaggers.Remove(global);
            }
        }

        public void SetFlag(object global, T flag)
        {
            AddOrGetFlagger(global).SetFlag(flag);
        }

        public void SetFlag(object global, T flag, params string[] propertyNames)
        {
            var authority = AddOrGetFlagger(global);
            foreach (var propertyName in propertyNames)
                authority.SetFlag(global.GetPropertyInfo(propertyName).ID, flag);
        }

        public void SetFlag(object global, int propertyID, T flag)
        {
            AddOrGetFlagger(global).SetFlag(propertyID, flag);
        }

        public T GetFlag(object global, int propertyID)
        {
            return AddOrGetFlagger(global).GetFlag(propertyID);
        }
    }
    */
