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
    public class Listener : Node3D
    {
        protected UnityEngine.AudioListener listener { get; private set; }

        public Listener() : base()
        {
            listener = GameObject.AddComponent<UnityEngine.AudioListener>();
        }
    }
}
#endif