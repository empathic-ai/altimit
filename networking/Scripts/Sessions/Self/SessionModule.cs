using Altimit.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public class SessionModule<TAppModule, TSessionModule> : Module<ISession, ISessionModule>, ISessionModule<TSessionModule> where TAppModule : IAppModule where TSessionModule : ISessionModule
    {
        //public IAppModule AppModule => appModule;

        protected App App => Session.App;
        protected TAppModule appModule => App.Get<TAppModule>();
        protected ISession peerSession => Session.Peer;
        public SelfSession Session => (SelfSession)container;
        public TSessionModule Peer => Session.Peer.Get<TSessionModule>();
        protected ILogger Logger => Session.Logger;
    }

    public interface ISessionModule<T> : ISessionModule where T : ISessionModule
    {
        T Peer { get; }
    }

    public interface ISessionModule : IModule<ISession, ISessionModule>
    {
        //IAppModule AppModule { get; }
    }
}
