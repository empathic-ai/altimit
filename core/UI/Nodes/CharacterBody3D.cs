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
    public class CharacterBody3D : Node3D
    {
    }
}
#elif GODOT
namespace Altimit.UI
{
    [AType]
    public class CharacterBody3D : Node3D
    {

        public CharacterBody3D() : base()
        {
        }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.CharacterBody3D();
        }
    }
}
#else
namespace Altimit.UI
{
    [AType]
    public class CharacterBody3D : Node3D
    {
    }
}
#endif