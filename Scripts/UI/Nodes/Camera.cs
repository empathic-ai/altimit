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
    public class Camera : Node3D
    {

        public Camera() : base()
        {
        }
    }
}
#endif