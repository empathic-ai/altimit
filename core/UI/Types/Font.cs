using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_64
using TMPro;
#endif

namespace Altimit.UI
{
    public class Font
    {
#if UNITY_64
        public TMP_FontAsset UFont;

        public Font(TMP_FontAsset uFont)
        {
            UFont = uFont;
        }
#endif
    }
}