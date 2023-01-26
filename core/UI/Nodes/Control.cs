using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
using UnityEngine.EventSystems;
#elif GODOT
using Godot;
#elif WEB
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endif

namespace Altimit.UI
{
    [AType(true)]
    public class Control : Node
    {
#if UNITY
        [AProperty]
        public Vector2 Size
        {
            get
            {
                return new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            }
            set
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value.y);
                layoutElement.preferredWidth = value.x;
                layoutElement.preferredHeight = value.y;
                rectTransform.ForceUpdateRectTransforms();
            }
        }

        [AProperty]
        public Vector2 AnchorMin
        {
            get
            {
                return (Vector2)rectTransform.anchorMin;
            }
            set
            {
                rectTransform.anchorMin = value;
            }
        }

        [AProperty]
        public Vector2 AnchorMax
        {
            get
            {
                return (Vector2)rectTransform.anchorMax;
            }
            set
            {
                rectTransform.anchorMax = value;
            }
        }

        public bool FlexibleSize
        {
            set
            {
                FlexibleWidth = value;
                FlexibleHeight = value;
            }
        }

        [AProperty]
        public bool FlexibleWidth
        {
            get
            {
                return layoutElement.flexibleWidth > 0;
            }
            set
            {
                layoutElement.flexibleWidth = value ? 1 : -1;
            }
        }

        [AProperty]
        public virtual bool IsClickable
        {
            get; set;
        } = false;

        [AProperty]
        public bool FlexibleHeight
        {
            get
            {
                return layoutElement.flexibleHeight > 0;
            }
            set
            {
                layoutElement.flexibleHeight = value ? 1 : -1;
            }
        }

        //[AProperty]
        //public Vector2 OffsetMin { get; set; }

        //[AProperty]
        //public Vector2 OffsetMax { get; set; }

        //[AProperty]
        //public Vector2 Margin { get; set; }

        [AProperty]
        public bool IgnoreLayout
        {
            get
            {
                return layoutElement.ignoreLayout;
            }
            set
            {
                layoutElement.ignoreLayout = value;
            }
        }
        [AProperty]
        public bool IsDraggable
        {
            get; set;
        }

        public event Action OnPointerClick
        {
            add
            {
                element.onPointerClick += value;
            }
            remove
            {
                element.onPointerClick -= value;
            }
        }

        public event Action<PointerEventData> OnDrag
        {
            add
            {
                element.onDrag += value;
            }
            remove
            {
                element.onDrag -= value;
            }
        }
        
        public event Action<PointerEventData> OnEndDrag
        {
            add
            {
                element.onEndDrag += value;
            }
            remove
            {
                element.onEndDrag -= value;
            }
        }

        protected RectTransform rectTransform { get; private set; }
        protected Element element { get; private set; }
        protected LayoutElement layoutElement { get; private set; }

        public Control() : base()
        {
            rectTransform = GameObject.AddComponent<RectTransform>();
            layoutElement = GameObject.AddComponent<LayoutElement>();
            element = GameObject.AddComponent<Element>();

            OnDrag += OnNodeDrag;
            OnEndDrag += OnNodeEndDrag;
        }

        public virtual void OnNodeEndDrag(PointerEventData obj)
        {
        }

        public virtual void OnNodeDrag(PointerEventData eventData)
        {
            if (IsDraggable)
            {
                Position += new Vector3(eventData.delta.x, eventData.delta.y, 0);
            }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();


        }
#elif GODOT

        [AProperty]
        public Vector2 AnchorMin
        {
            get
            {
                return new Vector2(gdControl.AnchorLeft, gdControl.AnchorBottom);
            }
            set
            {
                gdControl.LayoutMode = 1;
                gdControl.AnchorTop = value.x;
                gdControl.AnchorLeft = value.y;
            }
        }

        [AProperty]
        public Vector2 AnchorMax
        {
            get
            {
                return new Vector2(gdControl.AnchorRight, gdControl.AnchorTop);
            }
            set
            {
                gdControl.LayoutMode = 1;
                gdControl.AnchorRight = value.x;
                gdControl.AnchorBottom = value.y;
            }
        }

        // TODO: Fill out or possibly remove
        public virtual event Action OnPointerClick
        {
            add
            {
                
            }
            remove
            {
            }
        }

        [AProperty]
        public Vector2 MinSize
        {
            get
            {
                return (Vector2)gdControl.CustomMinimumSize;
            }
            set
            {
                gdControl.CustomMinimumSize = value;
            }
        }

        [AProperty]
        public float Height
        {
            get
            {
                return gdControl.Size.y;
            }
            set
            {
                gdControl.Size = new Vector2(gdControl.Size.x, value);
            }
        }

        [AProperty]
        public float Width
        {
            get
            {
                return gdControl.Size.x;
            }
            set
            {
                ExpandWidth = false;
                gdControl.Size = new Vector2(value, gdControl.Size.y);
                MinWidth = value;
                //Size = new Vector2(value, Size.y);
            }
        }

        [AProperty]
        public bool ExpandWidth
        {
            get
            {
                return (gdControl.SizeFlagsHorizontal & (int)Godot.Control.SizeFlags.Expand) != 0;
            }
            set
            {
                if (value)
                {
                    gdControl.SizeFlagsHorizontal += (int)Godot.Control.SizeFlags.Expand;
                    gdControl.SizeFlagsHorizontal += (int)Godot.Control.SizeFlags.Fill;
                }
                else
                {
                    gdControl.SizeFlagsHorizontal -= (int)Godot.Control.SizeFlags.Expand;
                    gdControl.SizeFlagsHorizontal -= (int)Godot.Control.SizeFlags.Fill;
                }
            }
        }

        [AProperty]
        public bool ExpandHeight
        {
            get
            {
                return (gdControl.SizeFlagsVertical & (int)Godot.Control.SizeFlags.Expand) != 0;
            }
            set
            {
                if (value)
                {
                    gdControl.SizeFlagsVertical += (int)Godot.Control.SizeFlags.Expand;
                    gdControl.SizeFlagsVertical += (int)Godot.Control.SizeFlags.Fill;
                }
                else
                {
                    gdControl.SizeFlagsVertical -= (int)Godot.Control.SizeFlags.Expand;
                    gdControl.SizeFlagsVertical -= (int)Godot.Control.SizeFlags.Fill;
                }
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
            set
            {
                Width = value.x;
                Height = value.y;
            }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return new Vector3(gdControl.Position.x, gdControl.Position.y, 0);
            }
            set
            {
                gdControl.Position = new Godot.Vector2(value.x, value.y);
            }
        }

        [AProperty]
        public bool IsVisible
        {
            get
            {
                return gdControl.Visible;
            }
            set
            {
                gdControl.Visible = value;
            }
        }


        [AProperty]
        public float MinWidth
        {
            get
            {
                return MinSize.x;
            }
            set
            {
                MinSize = new Vector2(value, MinSize.y);
            }
        }

        [AProperty]
        public float MinHeight
        {
            get
            {
                return MinSize.y;
            }
            set
            {
                MinSize = new Vector2(MinSize.x, value);
            }
        }

        [AProperty]
        public Anchor Anchor
        {
            get
            {
                return AnchorExtensions.AnchorFromGDLayoutPreset((Godot.Control.LayoutPreset)gdControl.AnchorsPreset);
            }
            set
            {
                gdControl.AnchorsPreset = (int)AnchorExtensions.GDLayoutPresetFromAnchor(value);
                //MinSize = new Vector2(MinSize.x, value);
            }
        }


        protected Godot.Control gdControl => GDNode as Godot.Control;

        public Control() : base()
        {
            gdControl.SizeFlagsHorizontal -= (int)Godot.Control.SizeFlags.ShrinkEnd;
        }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Control();
        }
#elif WEB
        [AProperty]
        public Anchor Anchor
        {
            get; set;
        }

        public Vector3 LocalPosition
        {
            get; set;
        }

        [Parameter]
        [AProperty]
        public Vector2 AnchorMin
        {
            get
            {
                return anchorMin;
            }
            set
            {
                anchorMin = value;
                UpdateState();
            }
        }

        private Vector2 anchorMin;

        [Parameter]
        [AProperty]
        public Vector2 AnchorMax
        {
            get
            {
                return anchorMax;
            }
            set
            {
                anchorMax = value;
                UpdateState();
            }
        }

        private Vector2 anchorMax;

        [AProperty]
        public float Height
        {
            get
            {
                return Size.y;
            }
            set
            {
                Size = new Vector2(Size.x, value);
            }
        }

        [AProperty]
        public float Width
        {
            get
            {
                return Size.x;
            }
            set
            {
                Size = new Vector2(value, Size.y);
            }
        }

        public Vector2 Size
        {
            get; set;
        }

        public event Action OnPointerClick;

        public event Action<PointerEventData> OnDrag;

        public event Action<PointerEventData> OnEndDrag;

        [AProperty]
        public bool ExpandWidth
        {
            get; set;
        }

        [AProperty]
        public bool ExpandHeight
        {
            get; set;
        }

        [AProperty]
        public Vector2 MinSize
        {
            get; set;
        }

        [AProperty]
        public float MinWidth
        {
            get
            {
                return MinSize.x;
            }
            set
            {
                MinSize = new Vector2(value, MinSize.y);
            }
        }

        [AProperty]
        public float MinHeight
        {
            get
            {
                return MinSize.y;
            }
            set
            {
                MinSize = new Vector2(MinSize.x, value);
            }
        }

        [AProperty]
        public bool IsVisible
        {
            get; set;
        }

        Dictionary<string, string> styleDictionary = new Dictionary<string, string>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // TODO: Possibly add region separation back in
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "Name", Microsoft.AspNetCore.Components.BindConverter.FormatValue(Name));

            if (Width > 0)
                styleDictionary["width"] = $"{Width}";
            if (Height > 0)
                styleDictionary["height"] = $"{Height}";

            styleDictionary["position"] = "inherit";
            styleDictionary["left"] = $"{ AnchorMin.x * 100}%";
            styleDictionary["top"] = $"{AnchorMin.y * 100}%";
            styleDictionary["right"] = $"{(1 - AnchorMax.x) * 100}%";
            styleDictionary["bottom"] = $"{(1-AnchorMax.y) * 100}%";

            string style = "";
            foreach (var keyvaluePair in styleDictionary)
            {
                style += $"{keyvaluePair.Key}: {keyvaluePair.Value};";
            }

            builder.AddAttribute(2, "style", style);
            //builder.OpenRegion(2);
            RenderChildren(builder);
            //builder.CloseRegion();
            builder.CloseElement();

            styleDictionary.Clear();
        }
#else
        [AProperty]
        public Anchor Anchor
        {
            get; set;
        }

        public Vector3 LocalPosition
        {
            get; set;
        }

        [AProperty]
        public Vector2 AnchorMin
        {
            get; set;
        }

        [AProperty]
        public Vector2 AnchorMax
        {
            get; set;
        }

        [AProperty]
        public float Height
        {
            get
            {
                return Size.y;
            }
            set
            {
                Size = new Vector2(Size.x, value);
            }
        }

        [AProperty]
        public float Width
        {
            get
            {
                return Size.x;
            }
            set
            {
                Size = new Vector2(value, Size.y);
            }
        }

        public Vector2 Size
        {
            get; set;
        }

        public event Action OnPointerClick;

        public event Action<PointerEventData> OnDrag;

        public event Action<PointerEventData> OnEndDrag;
        
        [AProperty]
        public bool ExpandWidth
        {
            get; set;
        }

        [AProperty]
        public bool ExpandHeight
        {
            get; set;
        }
        
        [AProperty]
        public Vector2 MinSize
        {
            get; set;
        }

        [AProperty]
        public float MinWidth
        {
            get
            {
                return MinSize.x;
            }
            set
            {
                MinSize = new Vector2(value, MinSize.y);
            }
        }

        [AProperty]
        public float MinHeight
        {
            get
            {
                return MinSize.y;
            }
            set
            {
                MinSize = new Vector2(MinSize.x, value);
            }
        }

        [AProperty]
        public bool IsVisible
        {
            get; set;
        }
#endif

        public bool ExpandSize
        {
            set
            {
                ExpandWidth = value;
                ExpandHeight = value;
            }
        }


        public virtual void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}
