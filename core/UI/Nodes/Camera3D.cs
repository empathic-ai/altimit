using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_64
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;

namespace Altimit.UI
{
    [AType]
    public class Camera : Node3D
    {
        protected UnityEngine.Camera camera { get; private set; }

        public Camera() : base()
        {
            camera = GameObject.AddComponent<UnityEngine.Camera>();
        }
    }
}
#elif GODOT
namespace Altimit.UI
{
    [AType]
    public class Camera3D : Node3D
    {

        public Camera3D() : base()
        {
        }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Camera3D();
        }
    }
}
#else
namespace Altimit.UI
{
    [AType]
    public class Camera3D : Node3D
    {

        public Camera3D() : base()
        {
        }
    }
}
#endif