using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using TMPro;
#endif

namespace Altimit.UI
{
    public class Font
    {
#if UNITY
        public TMP_FontAsset UFont;

        public Font(TMP_FontAsset uFont)
        {
            UFont = uFont;
        }
#endif
    }
}