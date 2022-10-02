using System;
using System.Collections.Generic;

namespace Altimit
{
    public class GlobalAuthority
    {
        public object Global;
        public Action<object, int, bool> onAuthorityChanged;

        readonly List<int> authorizedProperties = new List<int>();

        public GlobalAuthority(object global)
        {
            Global = global;
        }

        public List<int> GetProperties()
        {
            return authorizedProperties;
        }

        public bool HasAuthority(int propertyID)
        {
            return authorizedProperties.Contains(propertyID);
        }

        public void SetAuthority(int propertyID, bool hasAuthority)
        {
            //Debug.Log(Global.GetType() + ", " + propertyID + ", " + hasAuthority);

            if (hasAuthority)
            {
                if (!authorizedProperties.Contains(propertyID))
                {
                    authorizedProperties.Add(propertyID);
                    onAuthorityChanged?.Invoke(Global, propertyID, true);
                }
            }
            else
            {
                if (authorizedProperties.Contains(propertyID))
                {
                    authorizedProperties.Remove(propertyID);
                    onAuthorityChanged?.Invoke(Global, propertyID, false);
                }
            }
        }   
    }
}
