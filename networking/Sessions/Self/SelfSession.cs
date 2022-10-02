using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altimit.Serialization;
using System.Linq;

namespace Altimit.Networking
{
    public partial class SelfSession : Session  //ModuleContainer<ISession, ISessionModule>, ISession
    {
        public ReplicationSM Base => Get<ReplicationSM>();
        public ILogger Logger { get; private set; }

        public SelfSession(App app)
        {
            App = app;
            Logger = OS.Settings.GetLogger(App.Name + "/Temp-" + Guid.NewGuid().ToString().ToAbbreviatedString());
            AddModule<ReplicationSM>();
        }

        public async Task<T> Connect<T>() where T : ISessionModule
        {
            return (T)(await Connect(typeof(T)));
        }

        public async Task<ISessionModule> Connect(Type type)
        {
            return await Base.Connect(type);
        }

        /*
        public SelfSession(App app, bool isAuthority = false) {
            App = app;
            

            instanceDB.onInstanceAdded += OnInstanceAdded;

            instanceDB.onInstanceRemoved += OnInstanceRemoved;
            instanceDB.onSubInstancePassed += OnSubInstancePassed;
            instanceDB.onPropertyChanged += OnPropertyChanged;
            instanceDB.onMethodCalled += OnMethodCalled;


            instanceDB.onAssetAdded += OnAssetAdded;
            instanceDB.onAssetUpdated += OnAssetUpdated;

            Updater.Instance.AddUpdateable(this);
        }*/

        public App GetApp()
        {
            return App;
        }
        
        /*
        public void SetPeerAppID(Guid peerAppID)
        {
            PeerAppID = peerAppID;
            this.Logger.Name = App.Logger.Name + "/" + PeerAppID.ToString().ToAbbreviatedString();
        }*/

        public IPeerSession GetPeerSession()
        {
            return (IPeerSession)Peer;
        }


        /*
        public virtual void OnInstanceGroupAdded(object[] instances)
        {
            if (!isLocked)
            {
                var localInstanceReferences = new List<LocalInstanceReference>();
                foreach (var instance in instances)
                {
                    var instanceID = instanceDatabase.GetInstanceID(instance);
                    localInstanceReferences.Add(new LocalInstanceReference(instanceID, instance.GetType(), instanceDatabase.Localize(instance)));
                }
                PeerSession.RemoteAddInstanceGroup(localInstanceReferences.ToArray());
            }
        }*/


    }
}
