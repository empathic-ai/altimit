using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Altimit
{
    // Gives access to global and local instances, which are used in combination to construct local instances into global instances
    // The local and global instances can come from any sort of location--such as on this local machine or on an external machine
    public interface IInstanceResolver
    {
        //Dictionary<AID, object> LocalInstancesByID { get; }
        Task<object> GetInstance(AID instanceID);
        bool HasInstanceID(AID instanceID);

        object AddInstance(AID instanceID, object instance);

        void SetProperty(ref object obj, string propertyName, object property);
    }

    /*
    public class InstanceResolver : IInstanceResolver
    {
        Func<AID, Task<object>> getInstance;
        Dictionary<AID, object> InstancesByID = new Dictionary<AID, object>();
        public Dictionary<AID, object> LocalInstancesByID { get; } = new Dictionary<AID, object>();

        public InstanceResolver(Func<AID, Task<object>> getInstance)
        {
            this.getInstance = getInstance;
        }

        public bool HasInstanceID(AID instanceID)
        {
            return InstancesByID.ContainsKey(instanceID);
        }

        public async Task<object> GetInstance(AID instanceID)
        {
            object instance;
            if (InstancesByID.TryGetValue(instanceID, out instance))
                return instance;
            return await getInstance(instanceID);
        }

        public object AddInstance(AID instanceID, object instance)
        {
            InstancesByID.Add(instanceID, instance);
            return instance;
        }

        public void SetProperty(ref object obj, string propertyName, object property)
        {
            obj.SetProperty(propertyName, property);
        }
    }
    */
}
