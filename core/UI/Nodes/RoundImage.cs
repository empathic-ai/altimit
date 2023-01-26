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
    [AType(true)]
    public class RoundImage : Image
    {
        public RoundImage() : base()
        {
            ImageType = ImageType.Sliced;
            Sprite = AUI.GetSprite("Circle");
        }
    }
}
#endif