using Altimit.UI;


namespace Altimit.UI
{
    public partial class AUI
    {
        public static T FlexibleSize<T>(this T node, bool flexibleSize = true) where T : Control
        {
            node.FlexibleSize = flexibleSize;
#if UNITY_LEGACY
             return node.Hold<LayoutElement>(x => { x.flexibleWidth = isInfinite ? 10000 : 1; x.preferredWidth = includePreferred ? 1 : -1; });
#endif
            return node;
        }

        public static T FlexibleWidth<T>(this T node, bool flexibleWidth = true) where T : Control
        {
            node.FlexibleWidth = flexibleWidth;
#if UNITY_LEGACY
             return node.Hold<LayoutElement>(x => { x.flexibleWidth = isInfinite ? 10000 : 1; x.preferredWidth = includePreferred ? 1 : -1; });
#endif
            return node;
        }

        public static T FlexibleHeight<T>(this T node, bool flexibleHeight = true) where T : Control
        {
            node.FlexibleHeight = flexibleHeight;
            return node;

#if UNITY_LEGACY
            return node.Hold<LayoutElement>(x => { x.flexibleHeight = 10000; });
#endif
        }

        public static T SetSize<T>(this T node, float size = SmallSize) where T : Control
        {
            return node.SetSize(new Vector2(1,1) * size);
        }

        public static Control SetSize(this Control go, float width, float height)
        {
            return go.SetSize(new Vector2(width, height));
        }

        public static T SetSize<T>(this T node, Vector2 size) where T : Control
        {
            node.Size = size;
#if UNITY_LEGACY
            return node.Hold<RectTransform>(x => { x.sizeDelta = size; }).
                Hold<LayoutElement>(x => { x.minHeight = size.y; x.minWidth = size.x; x.preferredHeight = size.y; x.preferredWidth = size.x; });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static T SetHeight<T>(this T node, float height) where T : Control
        {
            node.Height = height;
            return node;
#if UNITY_LEGACY
            return node.Hold<RectTransform>(x => { x.sizeDelta = new Vector2(x.sizeDelta.x, height); }).
                Hold<LayoutElement>(x => { x.preferredHeight = height; x.preferredWidth = -1; }).
                MinHeight(height);
#endif
        }

        public static T FitSize<T>(this T node, bool fitWidth = true, bool fitHeight = true) where T : List
        {
            node.FitWidth = fitWidth;
            node.FitHeight = fitHeight;
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

        public static Node SetChildAlignment(this Node node, Anchor childAlignment)
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

        public static Node SetPositionZ(this Node node, float positionZ)
        {
#if UNITY_LEGACY
            return node.Hold<RectTransform>(x =>
            {
                x.anchoredPosition3D = new Vector3(x.anchoredPosition.x, x.anchoredPosition.y, positionZ);
            });
#elif GODOT
            return node;
#endif
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
#if UNITY_LEGACY
            return node.Hold<RectTransform>(x =>
            {
                x.anchoredPosition = position;
            });
#elif GODOT
            return node;
#endif
            return node;
        }


        public static Control SetAnchor(this Control node, Vector2 anchorMin, Vector2 anchorMax)
        {
            node.AnchorMin = anchorMin;
            node.AnchorMax = anchorMax;
            return node;
        }


        public static T Stretch<T>(this T node, float margin = 0) where T : Control
        {
            node.OnParentChanged(x => x.SetMargin(margin));
            return node;
        }

        public static Control SetMargin(this Control node, float margin = 0)
        {
            return node.SetMargin(new Vector2(1,1) * margin, -new Vector2(1, 1) * margin);
        }

        public static Control SetMargin(this Control node, Vector2 offsetMin, Vector2 offsetMax)
        {
            node.AnchorMin = new Vector2(0, 0);
            node.AnchorMax = new Vector2(1, 1);

            var parent = node.Parent as Control;
            if (parent != null)
            {
                // TODO: currently only works for 0 margin, change math for position and size to support other margins
                node.LocalPosition = Vector3.Zero;
                node.Size = parent.Size;
            }
            return node;
        }

        public static Node SetMargin(this Node node, float margin, StretchType stretchType)
        {
            return node;
        }

        public static Node IgnoreLayout(this Node node, bool ignoreLayout = true)
        {
#if UNITY_LEGACY
            return node.Hold<LayoutElement>(x => { x.ignoreLayout = ignoreLayout; });
#elif GODOT
            return node;
#endif
            return node;
        }
    }
}