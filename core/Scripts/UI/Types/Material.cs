using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public class Material
    {
#if UNITY
        public UnityEngine.Material UMaterial;

        public Material(UnityEngine.Material uMaterial)
        {
            UMaterial = uMaterial;
        }
#endif
    }
}
