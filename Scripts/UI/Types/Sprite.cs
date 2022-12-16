using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public class Sprite
    {
#if UNITY_5_3_OR_NEWER
        public UnityEngine.Sprite USprite;
        
        public Sprite(UnityEngine.Sprite uSprite) {
            USprite = uSprite;
        }
#endif
    }
}
