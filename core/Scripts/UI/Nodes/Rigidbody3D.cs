using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public class Rigidbody3D : Node3D
    {
        [AProperty]
        public Vector3 Velocity { get; set; }

        public Rigidbody3D() : base()
        {

        }
    }
}
