using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;

namespace Altimit.UI
{
    [AType]
    public class MeshInstance : Node3D
    {
        public MeshInstance() : base()
        {
        }
    }
}
#elif GODOT
namespace Altimit.UI
{
    [AType]
    public class MeshInstance : Node3D
    {

        public MeshInstance() : base()
        {
        }

        protected override Godot.Node GenerateGDNode()
        {
            var meshInstance = new Godot.MeshInstance3D();
            meshInstance.Mesh = new Godot.BoxMesh() { Size = (Godot.Vector3)Vector3.One };
            return meshInstance;
        }
    }
}
#else
namespace Altimit.UI
{
    [AType]
    public class MeshInstance : Node3D
    {

        public MeshInstance() : base()
        {
        }

        public Vector3 Position { get; set; }
    }
}
#endif