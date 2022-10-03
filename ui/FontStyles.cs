using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [Flags]
    public enum FontStyles
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        LowerCase = 8,
        UpperCase = 16,
        SmallCaps = 32,
        Strikethrough = 64,
        Superscript = 128,
        Subscript = 256,
        Highlight = 512
    }

}
