using System;
/*
#if UNITY_5_3_OR_NEWER
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Material = UnityEngine.Material;
using Font = TMPro.TMP_FontAsset;
using Node = UnityEngine.Component;
using AudioStream = UnityEngine.AudioClip;
using ImageType = UnityEngine.UI.Image.Type;
#elif GODOT
using Godot;
using Node = Godot.Node;
using Sprite = Godot.Texture;
#endif
*/
using Altimit.UI;
using UnityEngine;

namespace Altimit.UI
{
    public static partial class AUI
    {
        public static Node Name(this Node go, string name)
        {
            
            return go;
        }

        public static Node SetMaterial(this Node node, Material material = null)
        {
            /*
#if UNITY_5_3_OR_NEWER
            if (node.AddOrGet<Image>().material.name == "Default UI Material")
            {
                node.AddOrGet<Image>().material = Default;
            }

            if (material != null)
                node.AddOrGet<Image>().material = material;
#elif GODOT
#endif
            */
            return node;
        }

        public static Node OnValueChanged(this Node go, Action<bool> action)
        {
            /*
#if UNITY_5_3_OR_NEWER
            go.Toggle().Hold<Toggle>(x => x.onValueChanged.AddListener(new UnityAction<bool>(action)));
#elif GODOT

#endif
            */
            return go;
        }

        public static Node OnValueChanged(this Node go, Action<float> action)
        {
            /*
#if UNITY_5_3_OR_NEWER
            go.Hold<Slider>(x => x.onValueChanged.AddListener(new UnityAction<float>(action)));
#elif GODOT

#endif
            */
            return go;
        }

        public static T OnClick<T>(this T node, Action action, bool resetListeners = false) where T : Button
        {
            node.OnPointerClick += action;
            return node;
        }

        public static Node Mask(this Node node, bool showMaskGraphic = true)
        {
            //go.Hold<Image>(x => x.enabled = showMaskGraphic);
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Mask>(x=> x.showMaskGraphic = showMaskGraphic);
#elif GODOT
            */
            return node;
        }

        public static T SoftMask<T>(this T node, bool showMaskGraphic = true) where T : Node
        {
            /*
#if UNITY_5_3_OR_NEWER
            // Redirecting to regular mask due to shading error when VR is in use--TODO: fix shader
            //return go.Mask(showMaskGraphic);
            node.Hold<Image>(x => x.enabled = showMaskGraphic);
            return node.Hold<SoftMask>();
#elif GODOT
            */
            return node;
        }

        public static Node ChildControl(this Node node, bool childControlWidth = true, bool childControlHeight = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Get<HorizontalOrVerticalLayoutGroup>(x =>
            {
                x.childControlWidth = childControlWidth;
                x.childControlHeight = childControlHeight;
            });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node SetSortingOrder(this Node node, int sortingOrder)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Canvas().Hold<Canvas>(x => { x.overrideSorting = true; x.sortingOrder = sortingOrder; }).Hold<GraphicRaycaster>().Hold<OverrideCanvasHelper>();
#elif GODOT
            */
            return node;
//#endif
        }
                    /*
        public static Node FitSize(this Node node)
        {
            return node.FitWidth().FitHeight();
        }
                    
                    
        public static Node FitWidth(this Node node)
        {

#if UNITY_5_3_OR_NEWER
            return node.Hold<ContentSizeFitter>(x => x.horizontalFit = ContentSizeFitter.FitMode.PreferredSize);
#elif GODOT

            return node;
#endif
        }
    */

        public static Node SetSprite(this Node node, Sprite sprite = null)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Image>(x => x.sprite = sprite);
#elif GODOT
            */
            return node;
//#endif
        }
        /*
        public static Node RoundImage(this Node node, Material material, Sprite sprite = null)
        {
            return node.Image(sprite == null ? GetSprite("Circle") : sprite, material, ImageType.Sliced);
        }

        public static T RoundImage<T>(this T node, int size=-1, Material material = null, Sprite sprite = null) where T : Node
        {
            return node.Image(sprite == null ? GetSprite("Circle") : sprite, material, ImageType.Sliced).SetSize(size);
        }
        */
        public static Font GetFont(string name)
        {
#if UNITY_5_3_OR_NEWER
            return new Font(Resources.Load<TMPro.TMP_FontAsset>("Fonts/" + name));
#elif GODOT
            return null;
#else
            return null;
#endif
        }

        public static Sprite GetSprite(string name)
        {
            if (name == null)
                return null;

#if UNITY_5_3_OR_NEWER
            return new Sprite(Resources.Load<UnityEngine.Sprite>($"Sprites/{name}"));
#elif GODOT
            return null;
#else
            return null;
#endif
        }

        public static Material GetMaterial(string name, bool isNew = false)
        {
#if UNITY
            var uMaterial = Resources.Load<UnityEngine.Material>("Materials/" + name);
            if (isNew && Application.isPlaying)
                uMaterial = new UnityEngine.Material(uMaterial);
            return new Material(uMaterial);
#elif GODOT
            return null;
#endif
        }

        public static AudioStream GetSound(string name)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return Resources.Load<AudioStream>("Sounds/" + name);
#elif GODOT
            return null;
#endif
            */
            return null;
        }

        /*
        public static Node UI
        {
            get
            {
#if UNITY_5_3_OR_NEWER
                return A.New().Hold<RectTransform>(x=>x.gameObject.layer = LayerMask.NameToLayer("UI"));
#elif GODOT
                return A.New();
#endif
            }
        }
        */

        public static Node SetAlpha(this Node node, float alpha)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Image>(x => x.color = x.color.SetAlpha(alpha));
#elif GODOT
            return null;
#endif
            */
            return null;
        }

        public static Color SetAlpha(this Color c, float alpha)
        {
            c.a = alpha;
            return c;
        }
    }
}