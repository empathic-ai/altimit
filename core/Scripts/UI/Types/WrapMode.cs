#if GODOT
using Godot;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public enum WrapMode
    {
        None = 1,
        Word = 2
    }

    public static class WrapModeExtensions
    {
#if GODOT
        public static WrapMode WrapModeFromGDAutoWrapMode(Godot.TextServer.AutowrapMode autowrapMode)
        {
            switch(autowrapMode)
            {
                case TextServer.AutowrapMode.Word:
                    return WrapMode.Word;
                default:
                    return WrapMode.None;
            }
            return WrapMode.None;
        }

        public static Godot.TextServer.AutowrapMode GDAutoWrapModeFromWrapMode(WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case WrapMode.Word:
                    return TextServer.AutowrapMode.Word;
                default:
                    return TextServer.AutowrapMode.Off;
            }
            return TextServer.AutowrapMode.Off;
        }
#endif
    }
}
