#if UNITY_64
//Uncomment
//using LeTai.TrueShadow;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
 
namespace Altimit.UI.Unity
{
    public static partial class UnityUIExtensions
    {
        /*
        //Shortcut for adding action when panel shows
        public static Node OnShow(this Node go, object value)
        {
            go.AddOrGet<Panel>().onShowPanel += new Action<Node>(x => x.Set(value));
            return go;
        }
        */

        /*
        public static Node Update(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            LayoutRebuilder.ForceRebuildLayoutImmediate(node.Get<RectTransform>());
            return node;
#elif GODOT
            return null;
#endif
        }
        */

        /*
        public static Node OnAwake(this Node go, Action<Node> onAwake)
        {
            return go.Hold<View>(x=> { x.onAwake += onAwake; });
        }

        //Shortcut for adding action when panel shows
        public static Node OnShow(this Node go, Action<Node> func)
        {
            go.AddOrGet<Panel>().onShow += func;
            return go;
        }*/
        public static Anchor AnchorFromTextAlignment(TextAlignmentOptions alignment)
        {
            Anchor anchor = Anchor.UpperLeft;
            switch (alignment)
            {
                case TextAlignmentOptions.TopLeft:
                    anchor = Anchor.UpperLeft;
                    break;
                case TextAlignmentOptions.Left | TextAlignmentOptions.Center:
                    anchor = Anchor.MiddleLeft;
                    break;
                case TextAlignmentOptions.BottomLeft:
                    anchor = Anchor.LowerLeft;
                    break;
                case TextAlignmentOptions.Top:
                    anchor = Anchor.UpperCenter;
                    break;
                case TextAlignmentOptions.Baseline:
                    anchor = Anchor.MiddleCenter;
                    break;
                case TextAlignmentOptions.Bottom:
                    anchor = Anchor.LowerCenter;
                    break;
                case TextAlignmentOptions.TopRight:
                    anchor = Anchor.UpperRight;
                    break;
                case TextAlignmentOptions.Right | TextAlignmentOptions.Center:
                    anchor = Anchor.MiddleRight;
                    break;
                case TextAlignmentOptions.BottomRight:
                    anchor = Anchor.LowerRight;
                    break;
                default:
                    break;
            }
            return anchor;
        }

        public static Anchor AnchorFromTextAlignment(HorizontalAlignmentOptions xAlignment, VerticalAlignmentOptions yAlignment)
        {
            Anchor anchor = Anchor.UpperLeft;
            if (xAlignment == HorizontalAlignmentOptions.Left && yAlignment == VerticalAlignmentOptions.Top) {
                anchor = Anchor.UpperLeft;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Left && yAlignment == VerticalAlignmentOptions.Middle)
            {
                anchor = Anchor.MiddleLeft;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Left && yAlignment == VerticalAlignmentOptions.Bottom)
            {
                anchor = Anchor.LowerLeft;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Center && yAlignment == VerticalAlignmentOptions.Top)
            {
                anchor = Anchor.UpperCenter;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Center && yAlignment == VerticalAlignmentOptions.Middle)
            {
                anchor = Anchor.MiddleCenter;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Center && yAlignment == VerticalAlignmentOptions.Bottom)
            {
                anchor = Anchor.LowerCenter;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Right && yAlignment == VerticalAlignmentOptions.Top)
            {
                anchor = Anchor.UpperRight;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Right && yAlignment == VerticalAlignmentOptions.Middle)
            {
                anchor = Anchor.MiddleRight;
            }
            else if (xAlignment == HorizontalAlignmentOptions.Right && yAlignment == VerticalAlignmentOptions.Bottom)
            {
                anchor = Anchor.LowerRight;
            }
            return anchor;
        }

        public static void TextAlignmentFromAnchor(Anchor anchor, out HorizontalAlignmentOptions xAlignment, out VerticalAlignmentOptions yAlignment)
        {
            xAlignment = HorizontalAlignmentOptions.Left;
            yAlignment = VerticalAlignmentOptions.Top;

            switch (anchor)
            {
                case Anchor.UpperLeft:
                    xAlignment = HorizontalAlignmentOptions.Left;
                    yAlignment = VerticalAlignmentOptions.Top;
                    break;
                case Anchor.MiddleLeft:
                    xAlignment = HorizontalAlignmentOptions.Left;
                    yAlignment = VerticalAlignmentOptions.Middle;
                    break;
                case Anchor.LowerLeft:
                    xAlignment = HorizontalAlignmentOptions.Left;
                    yAlignment = VerticalAlignmentOptions.Bottom;
                    break;
                case Anchor.UpperCenter:
                    xAlignment = HorizontalAlignmentOptions.Center;
                    yAlignment = VerticalAlignmentOptions.Top;
                    break;
                case Anchor.MiddleCenter:
                    xAlignment = HorizontalAlignmentOptions.Center;
                    yAlignment = VerticalAlignmentOptions.Middle;
                    break;
                case Anchor.LowerCenter:
                    xAlignment = HorizontalAlignmentOptions.Center;
                    yAlignment = VerticalAlignmentOptions.Bottom;
                    break;
                case Anchor.UpperRight:
                    xAlignment = HorizontalAlignmentOptions.Right;
                    yAlignment = VerticalAlignmentOptions.Top;
                    break;
                case Anchor.MiddleRight:
                    xAlignment = HorizontalAlignmentOptions.Right;
                    yAlignment = VerticalAlignmentOptions.Middle;
                    break;
                case Anchor.LowerRight:
                    xAlignment = HorizontalAlignmentOptions.Right;
                    yAlignment = VerticalAlignmentOptions.Bottom;
                    break;
                default:
                    break;
            }
        }

        public static TextAlignmentOptions TextAlignmentFromAnchor(Anchor anchor)
        {
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;
            switch (anchor)
            {
                case Anchor.UpperLeft:
                    alignment = TextAlignmentOptions.TopLeft;
                    break;
                case Anchor.MiddleLeft:
                    alignment = TextAlignmentOptions.Left | TextAlignmentOptions.Center;
                    break;
                case Anchor.LowerLeft:
                    alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case Anchor.UpperCenter:
                    alignment = TextAlignmentOptions.Top;
                    break;
                case Anchor.MiddleCenter:
                    alignment = TextAlignmentOptions.Baseline | TextAlignmentOptions.Center;
                    break;
                case Anchor.LowerCenter:
                    alignment = TextAlignmentOptions.Bottom;
                    break;
                case Anchor.UpperRight:
                    alignment = TextAlignmentOptions.TopRight;
                    break;
                case Anchor.MiddleRight:
                    alignment = TextAlignmentOptions.Right | TextAlignmentOptions.Center;
                    break;
                case Anchor.LowerRight:
                    alignment = TextAlignmentOptions.BottomRight;
                    break;
                default:
                    break;
            }
            return alignment;
        }

        public static GameObject Text(this GameObject go, string text, UnityEngine.Material material = null, Anchor anchor = Anchor.UpperLeft, bool isClear = false)
        {
            if (material == null)
                material = AUI.Default.UMaterial;

            TextAlignmentOptions alignment = TextAlignmentFromAnchor(anchor);

            return go.Hold<TextMeshProUGUI>(x =>
            {
                x.text = text;
                x.color = material.color;
                x.alignment = alignment;
                x.color = material.color.SetAlpha(isClear ? .3f : 1);
                x.font = AUI.Font.UFont;
            });
        }

        public static UnityEngine.Color SetAlpha(this UnityEngine.Color c, float alpha)
        {
            c.a = alpha;
            return c;
        }

        public static GameObject SetPivot(this GameObject node, Vector2 pivot)
        {
            return node.Hold<RectTransform>(x =>
            {
                x.pivot = pivot;
            });
        }

        public static GameObject SetMargin(this GameObject go, UnityEngine.Vector2 offsetMin, UnityEngine.Vector2 offsetMax)
        {
            return go.Hold<RectTransform>(x =>
            {
                x.offsetMin = offsetMin;
                x.offsetMax = -offsetMax;
            });
        }

        public static GameObject SetMargin(this GameObject go, float margin, StretchType stretchType = StretchType.All)
        {
            return go.Hold<RectTransform>(x =>
            {
                x.anchorMin = new Vector2(0, 0);
                x.anchorMax = new Vector2(1, 1);

                x.offsetMin = new Vector2(margin, margin);
                x.offsetMax = -new Vector2(margin, margin);

                /*
                if (stretchType.HasFlag(StretchType.Vertical))
                {
                    x.anchorMax = new Vector2(0, 1);
                }
                if (stretchType.HasFlag(StretchType.Horizontal))
                {
                    x.anchorMax = new Vector2(1, 0);
                }
                */
            });
        }

        public static GameObject HList(this GameObject go, TextAnchor alignment)
        {
            return go.HList(AUI.SmallSpace, AUI.SmallSpace, alignment);
        }

        public static GameObject ExpandHeight(this GameObject go, bool value = true)
        {
            return go.Get<HorizontalLayoutGroup>(x => { x.childForceExpandHeight = value; }).Get<VerticalLayoutGroup>(x => { x.childForceExpandHeight = value; });
        }
        
        public static GameObject ExpandWidth(this GameObject go, bool value = true)
        {
            return go.Get<HorizontalLayoutGroup>(x => { x.childForceExpandWidth = value; }).Get<VerticalLayoutGroup>(x => { x.childForceExpandWidth = value; });
        }
        
        public static GameObject HList(this GameObject go, int padding = AUI.SmallSpace, int spacing = AUI.SmallSpace, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            return go.Hold<HorizontalLayoutGroup>(x =>
            {
                x.childAlignment = alignment;
                x.childControlHeight = true;
                x.childControlWidth = true;
                x.childForceExpandHeight = false;
                x.childForceExpandWidth = false;
                x.padding = new RectOffset(padding, padding, padding, padding);
                x.spacing = spacing;
            });
        }

        public static GameObject SetWidth(this GameObject go, float width)
        {
            if (width == -1)
                return go;

            return go.Hold<RectTransform>(x => { x.sizeDelta = new Vector2(width, x.sizeDelta.y); }).
                Hold<LayoutElement>(x => { x.preferredWidth = width; x.preferredHeight = -1; x.minWidth = width; });
        }

        public static GameObject SetPositionX(this GameObject go, float positionX)
        {
            return go.Hold<RectTransform>(x =>
            {
                x.anchoredPosition = new Vector2(positionX, x.anchoredPosition.y);
            });
        }

        public static GameObject Shadow(this GameObject go)
        {
            /*
            if (!go.GetComponent<UIShadow>())
            {
                for (int i = 1; i < 6; i += 1)
                {
                    var shadow = go.AddComponent<UIShadow>();
                    try {
                        shadow.style = ShadowStyle.Outline8;
                        shadow.effectDistance = Vector2.one * i;
                        shadow.effectColor = new Color(0, 0, 0, 3 / 255.0f);
                    } catch (Exception e)
                    {
                        throw new Exception("Failed to add a shadow component! Add an image to this Node first.", e);
                    }
                }
            }
            */
            return go.Hold<TrueShadow>(x=> {
                x.Size = 15;
                x.Spread = 0;
                x.OffsetDistance = 0;
                x.OffsetAngle = 0;
                x.Color = new Color(0, 0, 0, .33f);
            });
        }

        public static GameObject SetAnchor(this GameObject go, UnityEngine.Vector2 anchorMin, UnityEngine.Vector2 anchorMax)
        {
            return go.Hold<RectTransform>(x =>
            {
                x.anchorMin = anchorMin;
                x.anchorMax = anchorMax;
            });
        }

        public static GameObject SetAnchor(this GameObject go, TextAnchor anchor, StretchType stretchType = StretchType.None)
        {
            return go.Hold<RectTransform>(x =>
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
            });
        }

        public static GameObject StretchVerticalRight(this GameObject go, float minY = 0, float maxY = 0)
        {
            return go.SetAnchor(new Vector2(1, 0), new Vector2(1, 1)).Hold<RectTransform>(z =>
            {
                z.offsetMin = new Vector2(z.offsetMin.x, minY);
                z.offsetMax = new Vector2(z.offsetMax.x, maxY);
            });
        }

        public static GameObject FitHeight(this GameObject go)
        {
            return go.Hold<ContentSizeFitter>(x => x.verticalFit = ContentSizeFitter.FitMode.PreferredSize);
        }

        public static GameObject VList(this GameObject go, TextAnchor alignment)
        {
            return go.VList(AUI.SmallSpace, AUI.SmallSpace, alignment);
        }

        public static GameObject VList(this GameObject go, int padding = AUI.SmallSpace, int spacing = AUI.SmallSpace, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            return go.Hold<VerticalLayoutGroup>(x =>
            {
                x.childAlignment = alignment;
                x.childControlHeight = true;
                x.childControlWidth = true;
                x.childForceExpandHeight = false;
                x.childForceExpandWidth = true;
                x.padding = new RectOffset(padding, padding, padding, padding);
                x.spacing = spacing;
            });
        }

        public static GameObject Mask(this GameObject node, bool showMaskGraphic = true)
        {
            return node.Hold<Mask>(x=> x.showMaskGraphic = showMaskGraphic);
        }

        public static GameObject SetSize(this GameObject node, float size = AUI.SmallSize)
        {
            return node.SetSize(new Vector2(1, 1) * size);
        }

        public static GameObject SetSize(this GameObject node, UnityEngine.Vector2 size)
        {
            return node.Hold<RectTransform>(x => { x.sizeDelta = size; }).
                Hold<LayoutElement>(x => { x.minHeight = size.y; x.minWidth = size.x; x.preferredHeight = size.y; x.preferredWidth = size.x; });
        }

        public static GameObject RoundImage(this GameObject go, UnityEngine.Material material, UnityEngine.Sprite sprite = null)
        {
            return go.Image(sprite == null ? AUI.GetSprite("Circle").USprite : sprite, material, UnityEngine.UI.Image.Type.Sliced);
        }

        public static GameObject RoundImage(this GameObject go, int size = -1, UnityEngine.Material material = null, UnityEngine.Sprite sprite = null)
        {
            return go.Image(sprite == null ? AUI.GetSprite("Circle").USprite : sprite, material, UnityEngine.UI.Image.Type.Sliced).SetSize(size);
        }

        public static GameObject Image(this GameObject go,
            UnityEngine.Sprite sprite = null,
            UnityEngine.Material material = null,
            UnityEngine.UI.Image.Type type = UnityEngine.UI.Image.Type.Simple)
        {
            return go.Hold<UnityEngine.UI.Image>(x =>
            {
                x.sprite = sprite;
                x.type = type;
                x.material = material;
            });
        }

        public static GameObject FlexibleWidth(this GameObject go, bool includePreferred = true, bool isInfinite = true)
        {
             return go.Hold<LayoutElement>(x => { x.flexibleWidth = isInfinite ? 10000 : 1; x.preferredWidth = includePreferred ? 1 : -1; });
        }

        public static GameObject IgnoreLayout(this GameObject go, bool ignoreLayout = true)
        {
            return go.Hold<LayoutElement>(x => { x.ignoreLayout = ignoreLayout; });
        }

        //Dummy function
        public static GameObject Hold(this GameObject node)
        {
            return node;
        }

        public static GameObject Hold(this GameObject go, Action<GameObject> func)
        {
            func(go);
            return go;
        }

        //Adds or gets a component
        public static GameObject Hold<T>(this GameObject go) where T : Component
        {
            T t;
            return go.Hold<T>(out t);
        }

        //Adds or gets a component
        public static GameObject Hold<T>(this GameObject go, out T t) where T : Component
        {
            if (go == null)
                throw new Exception("Attempted to hold a component using a Node that doesn't exist!");
            t = go.AddOrGet<T>();
            return go;
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static GameObject Hold<T>(this GameObject go, Action<T> func) where T : Component
        {
            return go.Hold<T>(func, out _);
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static GameObject Hold<T>(this GameObject go, Action<T> func, out T t) where T : Component
        {
            return go.Hold(false, func, out t);
        }


        public static GameObject Hold<T>(this GameObject go, bool includeChildren, Action<T> func) where T : Component
        {
            return go.Hold<T>(includeChildren, func, out _);
        }

        public static GameObject Hold<T>(this GameObject go, bool includeChildren, out T t) where T : Component
        {
            t = go.AddOrGet<T>(includeChildren);
            return go;
        }

        public static GameObject Hold<T>(this GameObject go, bool includeChildren, Action<T> func, out T t) where T : Component
        {
            t = go.AddOrGet<T>(includeChildren);
            func(t);
            return go;
        }

        //Sets a Node's children
        public static GameObject Hold(this GameObject go, params Action<GameObject>[] children)
        {
            //children.ToList().ForEach(x => { x.transform.SetParent(go.transform); });
            return go;
        }

        public static GameObject AddChildren(this GameObject go, params GameObject[] children)
        {
            children.ToList().ForEach(x => { x.transform.parent = go.transform; });
            return go;
        }

        public static GameObject HoldFirst(this GameObject go, params GameObject[] children)
        {
            children.ToList().ForEach(x => { x.transform.parent = go.transform; x.transform.SetAsFirstSibling(); });
            return go;
        }

        //Sets a Node's children
        public static GameObject Hold(this GameObject go, params Component[] children)
        {
            //bool resetChildren = (node.GetComponent<RectTransform>() != null);
            children.ToList().ForEach(x => { x.transform.parent = go.transform; }); //.SetParent(node, resetChildren, resetChildren, resetChildren); });
            return go;
        }

        /*
        //Sets a Node's children
        public static Node Switch(this Node go, Node target)
        {
            go.Release();
            go.Hold(target);
            return go;
        }*/

        //Sets a Node's children
        /*
        public static Node Release(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            node.transform.DetachChildren();
            return node;
#elif GODOT
            return null;
#endif
        }
        */

        // TODO: Call generic version instead, or have generic refer to this
        public static Component AddOrGet(this GameObject node, Type type, bool includeChildren = false)
        {
            var comp = node.Get(type, includeChildren);
            if (comp == null)
                return node.gameObject.AddComponent(type);
            return comp;
        }

        //Adds or gets a component
        public static T AddOrGet<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return (T)go.AddOrGet(typeof(T), includeChildren);
        }

        //Returns if a Node has a component or not
        public static bool Has<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return go.Get<T>(includeChildren) != null;
        }

        /*
        //Gets a component
        public static T GetInParent<T>(this Node go) where T : Node
        {
            return (go == null ? null : go.GetComponentInParent<T>());
        }

        //Gets a component
        public static T[] GetInParents<T>(this Node node) where T : Node
        {
            return (node == null ? new T[0] : node.GetComponentsInParent<T>());
        }
        */

        //Gets a component
        public static T Get<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return (T)go.Get(typeof(T), includeChildren);
        }

        public static Component Get(this GameObject go, Type type, bool includeChildren = false)
        {
            if (go == null)
                throw new Exception("Tried setting component on a null Node!");

            if (includeChildren)
                return go.GetComponentInChildren(type);

            return go.GetComponent(type);
        }

        //Gets a component
        public static GameObject Get(this GameObject go)
        {
            return go;
        }

        //Sets a Node's children
        public static GameObject Get<T>(this GameObject node, Action<T> func) where T : Component
        {
            if (node.Has<T>())
            {
                func(node.Get<T>());
            }
            return node;
        }

        public static GameObject OnHeld(this GameObject go, Action<GameObject> func)
        {
            go.Hold<ParentObserver>(x =>
            {
                x.onUpdateSingle += func;
            });
            return go;
        }

        public static GameObject OnNextFrame(this GameObject node, Action<GameObject> func)
        {
            ExecuteFunc(node, func);
            return node;
        }

        static async void ExecuteFunc(GameObject node, Action<GameObject> func)
        {
            await Task.Delay(2000);
            func(node);
        }
        /*
        public static Node SetParent(this Node node, Node parent, bool setPosition = false, bool setRotation = false, bool setScale = false)
        {
#if UNITY_5_3_OR_NEWER
            node.transform.SetParent(parent.transform);
            if (setPosition)
                node.transform.localPosition = Vector3.zero;
            if (setRotation)
                node.transform.localEulerAngles = Vector3.zero;
            if (setScale)
                node.transform.localScale = Vector3.one;
#elif GODOT
#endif
            return node;
        }
        */

        /*
        public static RectTransform rectTransform(this Node go)
        {
            return go.GetComponent<RectTransform>();
        }
        */

        public static GameObject Call(this GameObject go, Action<GameObject> action)
        {
            if (action != null)
                action(go);
            return go;
        }
    }
}
#endif