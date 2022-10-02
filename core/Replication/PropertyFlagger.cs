using System;
using System.Collections.Generic;

namespace Altimit
{
    public class FlaggerMask
    {
        public Action<int> onSetFlag;
        int propertyID;
        readonly List<object> flaggers = new List<object>();
        bool defaultFlag;

        public FlaggerMask(int propertyID, bool defaultFlag = false)
        {
            this.propertyID = propertyID;
            this.defaultFlag = defaultFlag;
        }

        public List<object> GetFlaggers()
        {
            return flaggers;
        }

        public void SetFlag(object flagger, bool flag)
        {
            bool hasChanged = false;
            if ((!defaultFlag && flag) || (defaultFlag && !flag))
            {
                if (!flaggers.Contains(flagger))
                {
                    flaggers.Add(flagger);
                    hasChanged = true;
                }
            } else
            {
                flaggers.Remove(flagger);
                hasChanged = true;
            }
            if (hasChanged)
                onSetFlag?.Invoke(propertyID);
        }
    }
}
