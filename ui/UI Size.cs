using Altimit.UI;


namespace Altimit.UI
{
    public partial class AUI
    {
        public static Node FlexibleWidth(this Node node, bool includePreferred = true, bool isInfinite = true)
        {
#if UNITY_5_3_OR_NEWER
             return node.Hold<LayoutElement>(x => { x.flexibleWidth = isInfinite ? 10000 : 1; x.preferredWidth = includePreferred ? 1 : -1; });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node FlexibleWidth(this Node node, int width)
        {
#if UNITY_5_3_OR_NEWER
             return node.Hold<LayoutElement>(x => { x.flexibleWidth = width; });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node FlexibleHeight(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<LayoutElement>(x => { x.flexibleHeight = 10000; });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node SetSize(this Node go, float size = SmallSize)
        {
            if (size == -1)
                return go;
            return go.SetSize(new Vector2(1,1) * size);
        }

        public static Node SetSize(this Node go, float width, float height)
        {
            return go.SetSize(new Vector2(width, height));
        }

        public static Node SetSize(this Node node, Vector2 size)
        {
            if (size == -new Vector2(1, 1))
                return node;
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x => { x.sizeDelta = size; }).
                Hold<LayoutElement>(x => { x.minHeight = size.y; x.minWidth = size.x; x.preferredHeight = size.y; x.preferredWidth = size.x; });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node SetHeight(this Node node, float height)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x => { x.sizeDelta = new Vector2(x.sizeDelta.x, height); }).
                Hold<LayoutElement>(x => { x.preferredHeight = height; x.preferredWidth = -1; }).
                MinHeight(height);
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node SetWidth(this Node node, float width)
        {
            if (width == -1)
                return node;
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x => { x.sizeDelta = new Vector2(width, x.sizeDelta.y); }).
                Hold<LayoutElement>(x => { x.preferredWidth = width; x.preferredHeight = -1; }).
                MinWidth(width);
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node FitSize(this Node node, bool horizontalFit = true, bool verticalFit = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<ContentSizeFitter>(x =>
            {
                x.horizontalFit = horizontalFit ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
                x.verticalFit = verticalFit ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node MinSize(this Node go, Vector2 size)
        {
            return go.MinWidth(size.x).MinHeight(size.y);
        }

        public static Node MinHeight(this Node node, float height)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<LayoutElement>(x => x.minHeight = height);
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node MinWidth(this Node node, float width)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<LayoutElement>(x => x.minWidth = width);
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node StretchVerticalRight(this Node node, float minY = 0, float maxY = 0)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.SetAnchor(new Vector2(1, 0), new Vector2(1, 1)).Hold<RectTransform>(z =>
            {
                z.offsetMin = new Vector2(z.offsetMin.x, minY);
                z.offsetMax = new Vector2(z.offsetMax.x, maxY);
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node StretchVertical(this Node node, float minY = 0, float maxY = 0)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.SetAnchor(new Vector2(.5f, 0), new Vector2(.5f, 1)).Hold<RectTransform>(z =>
            {
                z.offsetMin = new Vector2(z.offsetMin.x, minY);
                z.offsetMax = new Vector2(z.offsetMax.x, maxY);
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node StretchHorizontal(this Node node, float minX = 0, float maxX = 0)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.SetAnchor(new Vector2(0, .5f), new Vector2(1, .5f)).Hold<RectTransform>(z =>
            {
                z.offsetMin = new Vector2(minX, z.offsetMin.y);
                z.offsetMax = new Vector2(maxX, z.offsetMax.y);
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node Stretch(this Node go, float margin = 0)
        {
            return go.SetAnchor(new Vector2(0,0), new Vector2(1, 1)).SetMargin(margin);
        }

        public static Node SetChildAlignment(this Node node, TextAnchor childAlignment)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Get<HorizontalOrVerticalLayoutGroup>(x => { x.childAlignment = childAlignment; });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node SetPadding(this Node go, int padding = SmallSpace)
        {
            return go.SetPadding(new Vector4(padding, padding, padding, padding));
        }

        public static Node SetPadding(this Node node, Vector4 margin)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<LayoutGroup>(x =>
            {
                x.padding = new RectOffset((int)margin.x, (int)margin.y, (int)margin.z, (int)margin.w);
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node SetSpacing(this Node node, float spacing)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<HorizontalOrVerticalLayoutGroup>(x =>
            {
                x.spacing = spacing;
            });
#elif GODOT
            */
            return node;
//#endif
        }

        // TODO: Add other anchors
        public static Node SetPivot(this Node node, Vector2 pivot)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                x.pivot = pivot;
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node SetPositionZ(this Node node, float positionZ)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                x.anchoredPosition3D = new Vector3(x.anchoredPosition.x, x.anchoredPosition.y, positionZ);
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node SetPositionX(this Node node, float positionX)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                x.anchoredPosition = new Vector2(positionX, x.anchoredPosition.y);
            });
#elif GODOT
            return node;
#endif
            */
            return node;
        }

        public static Node SetPositionY(this Node node, float positionY)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Transform>(x =>
            {
                x.localPosition = new Vector3(x.localPosition.x, positionY, x.localPosition.z);
                //UnityEngine.Canvas.ForceUpdateCanvases();
            });
#elif GODOT
            return node;
#endif
            */
            return node;
        }

        public static Node SetPosition(this Node node, Vector2 position)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                x.anchoredPosition = position;
            });
#elif GODOT
            return node;
#endif
            return node;
        }


        public static Node SetAnchor(this Node node, Vector2 anchorMin, Vector2 anchorMax)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                x.anchorMin = anchorMin;
                x.anchorMax = anchorMax;
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        // TODO: Add other anchors
        public static Node SetAnchor(this Node node, TextAnchor anchor, StretchType stretchType = StretchType.None)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                if (anchor == TextAnchor.UpperCenter && stretchType == StretchType.Horizontal)
                {
                    x.anchorMin = new Vector2(0, 1);
                    x.anchorMax = new Vector2(1, 1);
                } else if (anchor == TextAnchor.LowerCenter && stretchType == StretchType.Horizontal)
                {
                    x.anchorMin = new Vector2(0, 0);
                    x.anchorMax = new Vector2(1, 0);
                } else if (anchor == TextAnchor.MiddleLeft)
                {
                    x.anchorMin = new Vector2(0, .5f);
                    x.anchorMax = new Vector2(0, .5f);
                }
                else if (anchor == TextAnchor.LowerCenter)
                {
                    x.anchorMin = new Vector2(.5f, 0);
                    x.anchorMax = new Vector2(.5f, 0);
                }
                //if (stretchType.HasFlag(StretchType.Horizontal))
                //    x.anchorMin = Vector2.zero;
            });
#elif GODOT
            return node;
#endif
            */
            return node;
        }

        public static Node SetMargin(this Node node, float margin = SmallSpace)
        {
            return node.SetMargin(new Vector2(1,1) * margin, -new Vector2(1, 1) * margin);
        }

        public static Node SetMargin(this Node node, Vector2 offsetMin, Vector2 offsetMax)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(z =>
            {
                z.anchorMin = new Vector2(0, 0);
                z.anchorMax = new Vector2(1, 1);
                z.offsetMin = offsetMin;
                z.offsetMax = offsetMax;
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node SetMargin(this Node node, float margin, StretchType stretchType)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<RectTransform>(x =>
            {
                if (stretchType.HasFlag(StretchType.Horizontal))
                {
                    x.offsetMin = new Vector2(margin, x.offsetMin.y);
                    x.offsetMax = -new Vector2(margin, x.offsetMax.y);
                    x.anchorMax = new Vector2(1, 0);
                }
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node IgnoreLayout(this Node node, bool ignoreLayout = true)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<LayoutElement>(x => { x.ignoreLayout = ignoreLayout; });
#elif GODOT
            return node;
#endif
            return node;
        }
    }
}