using Altimit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    [AType]
    public interface IReplicationSM : ISessionModule<IReplicationSM>
    {
        /*
        [AMethod("bcfc591e-a9e1-40b0-9b2f-a7cd0b547552")]
        public void SetPeerInitialized();
        [AMethod("d186512c-c1ea-476b-bdd6-e77a493086ac")]
        public Task<Guid> GetAppID();
        */

        [AMethod]
        public Task OnConnection(Type moduleType);

        [AMethod]
        //[return: Ignore]
        public Task AddOrTrackInstance(AID instanceID);

        [AMethod]
        Task AddInstance(AID instanceID, Type type, object[] args);

        [AMethod]
        void RetrieveInstance(AID instanceID);
        [AMethod]
        void OnboardInstance(object instance);

        //[AMethod]
        //void AddOrTrackInstance(AID instanceID, Type instanceType, object[] constructorArgs);

        //[AMethod("bb931c9a-5bac-4af9-b038-a87bdd7243bb")]
        //void RemoteAddInstanceGroup([ForceValue(true)] LocalInstanceReference[] localInstanceReferences);

        [AMethod]
        void ChangeProperty(object instance, string propertyName, object value);

        [AMethod]
        void RemoveInstance(AID instanceID);

        [AMethod]
        void AddAsset(AID instanceID, AssetMetadata assetMetadata, byte[] bytes);

        [AMethod]
        void UpdateAsset(AID instanceID, byte[] bytes);

        [AMethod]
        void RemoveAsset(AID instanceID);
        [AMethod]
        void AddElement(object instance, int index, object element);
        [AMethod]
        void RemoveElement(object instance, int index, object element);

        [AMethod]
        void CallMethod(object instance, string methodName, Type[] methodTypes, params object[] args);

        [AMethod]
        void SetAuthority(object instance, bool hasAuthority);
        [AMethod]
        void SetAuthority(object instance, string propertyName, bool hasAuthority);
    }
}
