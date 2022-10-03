using Altimit.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Altimit.Networking
{
    public class Session : ModuleContainer<ISession, ISessionModule>, ISession
    {
        protected override ISession container => this;
        public ISession Peer { get; set; }

        public App App { get; set; }
    }
}
