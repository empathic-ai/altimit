using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altimit.Serialization;
using System.Linq;

namespace Altimit.Networking
{
    public partial class SelfSession : Session  //ModuleContainer<ISession, ISessionModule>, ISession
    {
        //public ReplicationSM Base => Get<ReplicationSM>();
        public ILogger Logger { get; private set; }

        public SelfSession(App app)
        {
            App = app;
            Logger = OS.Settings.GetLogger(App.Name + "/Temp-" + Guid.NewGuid().ToString().ToAbbreviatedString());
        }
        /*
        */
        public App GetApp()
        {
            return App;
        }
        
        public IPeerSession GetPeerSession()
        {
            return (IPeerSession)Peer;
        }

    }
}
