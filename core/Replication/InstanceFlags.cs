using System;
using System.Collections.Generic;
/*
namespace Altimit
{
    
    public class InstanceFlags
    {
        public object Global;
        public Action<object, int, bool> onFlagChanged;
        readonly List<int> propertiesFlagged = new List<int>();
        bool defaultFlag;
        //readonly Dictionary<int, bool> propertyFlaggers = new Dictionary<int, bool>();

        public InstanceFlags(object global, bool defaultFlag = false)
        {
            Global = global;
            this.defaultFlag = defaultFlag;
        }

        public void SetFlag(int propertyID, bool flag)
        {
            bool hasChanged = false;
            if ((!defaultFlag && flag) || (defaultFlag && !flag))
            {
                if (!propertiesFlagged.Contains(propertyID))
                {
                    propertiesFlagged.Add(propertyID);
                    hasChanged = true;
                }
            } else
            {
                propertiesFlagged.Remove(propertyID);
                hasChanged = true;
            }

            if(hasChanged)
                onFlagChanged?.Invoke(Global, propertyID, flag);
        }

        public void SetFlag(bool flag)
        {
            foreach (var propertyInfo in Global.GetClassInfo().IDPropertyInfos)
            {
                SetFlag(propertyInfo.Key, flag);
            }
        }

        public bool GetFlag(int propertyID)
        {
            var flag = propertiesFlagged.Contains(propertyID);
            return defaultFlag ? !flag : flag;
        }
    }
}*/

/*
public class GlobalFlagger<T>
    {
        public object Global;
        public Action<object, int, T> onObserveTypeChanged;

        readonly Dictionary<int, T> propertyFlaggers = new Dictionary<int, T>();

        public GlobalFlagger(object global, T defaultFlag)
        {
            Global = global;
            foreach (var propertyInfo in Global.GetClassInfo().PropertyInfos)
            {
                propertyFlaggers.Add(propertyInfo.ID, defaultFlag);
            }
        }

        public void SetFlag(int propertyID, T flag)
        {
            propertyFlaggers[propertyID] = flag;
        }

        public void SetFlag(T flag)
        {
            foreach (var propertyFlagger in propertyFlaggers)
            {
                propertyFlaggers[propertyFlagger.Key] = flag;
            }
        }

        public T GetFlag(int propertyID)
        {
            return propertyFlaggers[propertyID];
        }
    }
    */
