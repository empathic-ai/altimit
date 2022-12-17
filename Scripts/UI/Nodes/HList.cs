using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
#endif
namespace Altimit.UI
{
    [AType(true)]
    public class HList : Container
    {
#if UNITY
        [AProperty]
        public override Anchor Anchor
        {
            get
            {
                return (Anchor)horizontalLayoutGroup.childAlignment;
            }
            set
            {
                horizontalLayoutGroup.childAlignment = (TextAnchor)value;
            }
        }

        [AProperty]
        public override bool ExpandChildWidth
        {
            get
            {
                return horizontalLayoutGroup.childForceExpandWidth;
            }
            set
            {
                horizontalLayoutGroup.childForceExpandWidth = value;
            }
        }

        [AProperty]
        public override bool ExpandChildHeight
        {
            get
            {
                return horizontalLayoutGroup.childForceExpandHeight;
            }
            set
            {
                horizontalLayoutGroup.childForceExpandHeight = value;
            }
        }

        [AProperty]
        public override bool FitWidth
        {
            get
            {
                return (contentSizeFitter.horizontalFit == ContentSizeFitter.FitMode.PreferredSize);
            }
            set
            {
                var fitMode = value ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                contentSizeFitter.horizontalFit = fitMode;
            }
        }

        [AProperty]
        public override bool FitHeight
        {
            get
            {
                return (contentSizeFitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize);
            }
            set
            {
                var fitMode = value ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                contentSizeFitter.verticalFit = fitMode;
            }
        }

        [AProperty]
        public override float Padding
        {
            get
            {
                return horizontalLayoutGroup.padding.top;
            }
            set
            {
                horizontalLayoutGroup.padding = new RectOffset((int)value, (int)value, (int)value, (int)value);
            }
        }

        [AProperty]
        public override float Spacing
        {
            get
            {
                return horizontalLayoutGroup.spacing;
            }
            set
            {
                horizontalLayoutGroup.spacing = value;
            }
        }

        protected HorizontalLayoutGroup horizontalLayoutGroup { get; private set; }
        protected ContentSizeFitter contentSizeFitter { get; private set; }

        public HList() : base()
        {
            horizontalLayoutGroup = GameObject.AddComponent<HorizontalLayoutGroup>();
            contentSizeFitter = GameObject.AddComponent<ContentSizeFitter>();

            horizontalLayoutGroup.childControlHeight = true;
            horizontalLayoutGroup.childControlWidth = true;
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childForceExpandWidth = false;
        }
#endif
        public override Anchor Anchor { get; set; }
        public override bool ExpandChildWidth { get; set; }
        public override bool ExpandChildHeight { get; set; }
        public override bool FitWidth { get; set; }
        public override bool FitHeight { get; set; }
        public override float Padding { get; set; }
        public override float Spacing { get; set; }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.HBoxContainer();
        }
    }
}