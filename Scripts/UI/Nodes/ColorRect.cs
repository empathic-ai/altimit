using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
using SoftMasking;
#endif

namespace Altimit.UI
{
    [AType(true)]
    public class ColorRect : Control
    {
#if UNITY
        public Color Color { get; set; }

        //Todo: add back in once Material is consistent rather than always new instance
        //[AProperty]
        public Material Material
        {
            get
            {
                return new Material(image.material);
            }
            set
            {
                image.material = value.UMaterial;
            }
        }

        [AProperty]
        public bool UseShadow = true;
        
        [AProperty]
        public bool IsMask
        {
            get
            {
                return mask.enabled;
            }
            set
            {
                mask.enabled = value;
            }
        }

        [AProperty]
        public override bool IsClickable
        {
            get
            {
                return image.raycastTarget;
            }
            set
            {
                image.raycastTarget = value;
            }
        }

        protected UnityEngine.UI.Image image { get; private set; }
        //protected SoftMask mask { get; private set; }
        protected Mask mask { get; private set; }

        public Image() : base()
        {
            image = GameObject.AddComponent<UnityEngine.UI.Image>();
            image.raycastTarget = false;
            //mask = GameObject.AddComponent<SoftMask>();
            mask = GameObject.AddComponent<Mask>();
            mask.enabled = false;
        }
#endif
        public Color Color
        {
            get
            {
                return (Color)GDColorRect.Color;
            }
            set
            {
                GDColorRect.Color = value;
            }
        }

        public bool IgnoreLayout { get; internal set; }
        public Material Material { get; internal set; }
        public Vector2 Size { get; internal set; }
        public bool UseShadow { get; internal set; }
        public bool IsClickable { get; internal set; }
        public bool IsMask { get; internal set; }

        protected Godot.ColorRect GDColorRect => GDNode as Godot.ColorRect;

        public ColorRect() : base()
        {
        }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.ColorRect() { Color = Color.white };
        }
    }
}