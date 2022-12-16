using Altimit.Serialization;
using System;
using System.Threading.Tasks;
using Altimit;
using System.Threading;
using System.Reflection;

namespace Altimit.Networking
{
    public abstract class SessionSocket<TSelfSession, TPeerSession> : ISessionSocket where TSelfSession : ISelfSession where TPeerSession : ISession
    {
        Func<TSelfSession> selfSessionFactory;
        protected Database instanceDatabase;
        protected ILogger logger;
        public AList<ISelfSession> Sessions { get; } = new AList<ISelfSession>();

        public SessionSocket(Database instanceDatabase, Func<TSelfSession> selfSessionFactory, ILogger logger)
        {
            this.instanceDatabase = instanceDatabase;
            this.logger = logger;
            this.selfSessionFactory = selfSessionFactory;
        }

        public async Task<TSelfSession> AddSession(ISocketPeer peer)
        {
            var selfSession = selfSessionFactory();
            //var peerSession = SessionModuleFactory.Generate<TPeerSession>(instanceDatabase, peer, null);

            //await selfSession.Init(peerSession);

            Sessions.Add(selfSession);

            return selfSession;
        }

        public virtual void RemoveSession(ISelfSession session)
        {
            Sessions.Remove(session);
            //session.Dispose();
        }

        public virtual void Dispose()
        {
            while (Sessions.Count > 0)
                RemoveSession(Sessions[0]);
        }
    }
}