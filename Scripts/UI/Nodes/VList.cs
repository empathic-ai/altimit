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
    public class VList : List
    {

#if UNITY
        [AProperty]
        public override Anchor Anchor
        {
            get
            {
                return (Anchor)verticalLayoutGroup.childAlignment;
            }
            set
            {
                verticalLayoutGroup.childAlignment = (TextAnchor)value;
            }
        }

        [AProperty]
        public override bool ExpandChildWidth
        {
            get
            {
                return verticalLayoutGroup.childForceExpandWidth;
            }
            set
            {
                verticalLayoutGroup.childForceExpandWidth = value;
            }
        }

        [AProperty]
        public override bool ExpandChildHeight
        {
            get
            {
                return verticalLayoutGroup.childForceExpandHeight;
            }
            set
            {
                verticalLayoutGroup.childForceExpandHeight = value;
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
                return verticalLayoutGroup.padding.top;
            }
            set
            {
                verticalLayoutGroup.padding = new RectOffset((int)value, (int)value, (int)value, (int)value);
            }
        }

        [AProperty]
        public override float Spacing
        {
            get
            {
                return verticalLayoutGroup.spacing;
            }
            set
            {
                verticalLayoutGroup.spacing = value;
            }
        }

        protected VerticalLayoutGroup verticalLayoutGroup { get; private set; }
        protected ContentSizeFitter contentSizeFitter { get; private set; }

        public VList() : base()
        {
            verticalLayoutGroup = GameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            contentSizeFitter = GameObject.AddComponent<ContentSizeFitter>();

            verticalLayoutGroup.childControlHeight = true;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childForceExpandWidth = false;
        }
#endif
        public override Anchor Anchor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool ExpandChildWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool ExpandChildHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool FitWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override bool FitHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float Padding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override float Spacing { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
