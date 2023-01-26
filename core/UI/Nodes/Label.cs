#if WEB
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

#if UNITY_64
using Altimit.UI.Unity;

namespace Altimit.UI
{
    [AType]
    public class Label : Control
    {
        [AProperty]
        public string Text
        {
            get
            {
                return textMeshProUGUI.text;
            }
            set
            {
                textMeshProUGUI.text = value;
            }
        }

        public Material Material
        {
            get
            {
                return new Material(textMeshProUGUI.material);
            }
            set
            {
                textMeshProUGUI.material = value.UMaterial;
            }
        }

        [AProperty]
        public Anchor Anchor
        {
            get
            {
                return UnityUIExtensions.AnchorFromTextAlignment(textMeshProUGUI.horizontalAlignment, textMeshProUGUI.verticalAlignment);
            }
            set
            {
                TMPro.HorizontalAlignmentOptions xAlignment;
                TMPro.VerticalAlignmentOptions yAlignment;

                UnityUIExtensions.TextAlignmentFromAnchor(value, out xAlignment, out yAlignment);

                textMeshProUGUI.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                textMeshProUGUI.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;
            }
        }

        protected TMPro.TextMeshProUGUI textMeshProUGUI { get; private set; }

        public Label() : base()
        {
            textMeshProUGUI = GameObject.AddComponent<TMPro.TextMeshProUGUI>();
        }
    }
}

#elif GODOT

namespace Altimit.UI
{
    [AType]
    public class Label : Control
    {
        public WrapMode WrapMode
        {
            get
            {
                return WrapModeExtensions.WrapModeFromGDAutoWrapMode(GDLabel.AutowrapMode);
            }
            set
            {
                GDLabel.AutowrapMode = WrapModeExtensions.GDAutoWrapModeFromWrapMode(value);
            }
        }

        [AProperty]
        public int FontSize
        {
            get
            {
                return GDLabel.GetThemeFontSize("font_size");
            }
            set
            {
                GDLabel.AddThemeFontSizeOverride("font_size", value);
            }
        }

        [AProperty]
        public string Text
        {
            get
            {
                return GDLabel.Text;
            }
            set
            {
                GDLabel.Text = value;
            }
        }

        public Color Color
        {
            get
            {
                return (Color)GDLabel.GetThemeColor("font_color");
            }
            set
            {
                GDLabel.AddThemeColorOverride("font_color", value);

            }
        }

        [AProperty]
        public Anchor TextAnchor
        {
            get
            {
                return AnchorExtensions.AnchorFromGDAlignment(GDLabel.HorizontalAlignment, GDLabel.VerticalAlignment);
            }
            set
            {
                var (horizontalAlignment, verticalALignment) = AnchorExtensions.GDAlignmentFromAnchor(value);
                GDLabel.HorizontalAlignment = horizontalAlignment;
                GDLabel.VerticalAlignment = verticalALignment;
            }
        }

        public Label() : base()
        {
            WrapMode = WrapMode.Word;
        }

        public Godot.Label GDLabel => GDNode as Godot.Label;

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Label();
        }
    }
}
#elif WEB
namespace Altimit.UI
{
    [AType]
    public class Label : Control
    {
        [AProperty]
        public int FontSize
        {
            get; set;
        }

        [Parameter]
        [AProperty]
        public string Text
        {
            get; set;
        }

        public Color Color
        {
            get; set;
        }

        [AProperty]
        public Anchor TextAnchor
        {
            get; set;
        }

        public Label() : base()
        {
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "p");
            builder.AddAttribute(1, "value", Microsoft.AspNetCore.Components.BindConverter.FormatValue(Text));
            builder.SetUpdatesAttributeName("value");
            builder.AddContent(2, Text);
            builder.CloseElement();
        }
    }
}
#else
namespace Altimit.UI
{
    [AType]
    public class Label : Control
    {
        [AProperty]
        public string Text
        {
            get; set;
        }

        public Color Color
        {
            get; set;
        }

        [AProperty]
        public Anchor TextAnchor
        {
            get; set;
        }

        public Anchor Anchor { get; internal set; }

        public Label() : base()
        {
        }
    }
}
#endif