using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
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
        // TODO: Fill in
        [AProperty]
        public string Text
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public Anchor Anchor { get; internal set; }

        protected override Godot.Label GenerateGDNode()
        {
            return new Godot.Label();
        }
    }
}

#endif