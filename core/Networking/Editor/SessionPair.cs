using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.Networking
{
    public struct SessionPair
    {
        public ServiceProvider SessionA;
        public ServiceProvider SessionB;

        public SessionPair(ServiceProvider sessionA, ServiceProvider sessionB)
        {
            SessionA = sessionA;
            SessionB = sessionB;
        }
    }
}
