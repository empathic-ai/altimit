using System;
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
using Altimit.UI;

namespace Altimit.UI
{
    public static partial class AUI
    {
        public static Node Name(this Node go, string name)
        {
#if UNITY_5_3_OR_NEWER
            go.name = name;
#elif GODOT
            go.Name = name;
#endif
            return go;
        }

        public static Node SetMaterial(this Node node, Material material = null)
        {
#if UNITY_5_3_OR_NEWER
            if (node.AddOrGet<Image>().material.name == "Default UI Material")
            {
                node.AddOrGet<Image>().material = Default;
            }

            if (material != null)
                node.AddOrGet<Image>().material = material;
#elif GODOT
#endif

            return node;
        }

        public static Node OnValueChanged(this Node go, Action<bool> action)
        {
#if UNITY_5_3_OR_NEWER
            go.Toggle().Hold<Toggle>(x => x.onValueChanged.AddListener(new UnityAction<bool>(action)));
#elif GODOT

#endif
            return go;
        }

        public static Node OnValueChanged(this Node go, Action<float> action)
        {
#if UNITY_5_3_OR_NEWER
            go.Hold<Slider>(x => x.onValueChanged.AddListener(new UnityAction<float>(action)));
#elif GODOT

#endif
            return go;
        }

        public static Node OnClick(this Node node, Action action, bool resetListeners = false)
        {
#if UNITY_5_3_OR_NEWER
            node.Hold<Element>(x => x.onPointerClick += x=>action());
#elif GODOT

#endif

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

        public static Node SoftMask(this Node node, bool showMaskGraphic = true)
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

        public static Node ExpandWidth(this Node node, bool value = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Get<HorizontalLayoutGroup>(x => { x.childForceExpandWidth = value; }).Get<VerticalLayoutGroup>(x => { x.childForceExpandWidth = value; });
#elif GODOT
            */
            return node;
        }

        public static Node ExpandHeight(this Node node, bool value = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Get<HorizontalLayoutGroup>(x => { x.childForceExpandHeight = value; }).Get<VerticalLayoutGroup>(x => { x.childForceExpandHeight = value; });
#elif GODOT
            */
            return node;
//#endif
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

        public static Node FitSize(this Node node)
        {
            return node.FitWidth().FitHeight();
        }

        public static Node FitWidth(this Node node)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<ContentSizeFitter>(x => x.horizontalFit = ContentSizeFitter.FitMode.PreferredSize);
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node FitHeight(this Node node)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<ContentSizeFitter>(x => x.verticalFit = ContentSizeFitter.FitMode.PreferredSize);
#elif GODOT
            */
            return node;
//#endif
        }

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

        public static Node RoundImage(this Node node, Material material, Sprite sprite = null)
        {
            return node.Image(sprite == null ? GetSprite("Circle") : sprite, material, ImageType.Sliced);
        }

        public static Node RoundImage(this Node node, int size=-1, Material material = null, Sprite sprite = null)
        {
            return node.Image(sprite == null ? GetSprite("Circle") : sprite, material, ImageType.Sliced).SetSize(size);
        }

        public static Font GetFont(string name)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return Resources.Load<Font>("Fonts/" + name);
#elif GODOT
            */
            return null;
//#endif
        }

        public static Sprite GetSprite(string name)
        {
            if (name == null)
                return null;
#if UNITY_5_3_OR_NEWER
            return Resources.Load<Sprite>("Sprites/" + name);
#elif GODOT
            return null;
#endif
            return null;
        }

        public static Material GetMaterial(string name, bool isNew = false)
        {
#if UNITY_5_3_OR_NEWER
            Material material = Resources.Load<Material>("Materials/" + name);
            if (isNew && Application.isPlaying)
                return new Material(material);
            return material;
#elif GODOT
            return null;
#endif
            return null;
        }

        public static AudioStream GetSound(string name)
        {
#if UNITY_5_3_OR_NEWER
            return Resources.Load<AudioStream>("Sounds/" + name);
#elif GODOT
            return null;
#endif
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
#if UNITY_5_3_OR_NEWER
            return node.Hold<Image>(x => x.color = x.color.SetAlpha(alpha));
#elif GODOT
            return null;
#endif
            return null;
        }

        public static Color SetAlpha(this Color c, float alpha)
        {
            c.a = alpha;
            return c;
        }
    }
}