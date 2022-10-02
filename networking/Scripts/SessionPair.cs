using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public struct SessionPair
    {
        public Session SessionA;
        public Session SessionB;

        public SessionPair(Session sessionA, Session sessionB)
        {
            SessionA = sessionA;
            SessionB = sessionB;
        }
    }
}
