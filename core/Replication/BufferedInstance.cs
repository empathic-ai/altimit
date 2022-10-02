using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public class BufferedInstance
    {
        public Guid InstanceID;
        public List<Guid> MissingInstanceIDs = new List<Guid>();
        public object LocalInstance;

        public BufferedInstance(Guid id, List<Guid> missingInstanceIDs, object localInstance)
        {
            InstanceID = id;
            MissingInstanceIDs = missingInstanceIDs;
            LocalInstance = localInstance;
        }
    }
}
