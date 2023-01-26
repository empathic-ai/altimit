//using UnityEngine;

#if UNITY
using Altimit.UI.Unity;
using DG.Tweening;
#else
using Microsoft.AspNetCore.Components.Rendering;
#endif

using Altimit;
using Altimit.UI;
using Altimit.Networking;
using System.Runtime.CompilerServices;
using System.Net;

namespace Altimit.UI {
    public class Window : Node
    {
#if GODOT
        public string Title
        {
            get
            {
                return GDWindow.Title;
            }
            set
            {
                GDWindow.Title = value;
            }
        }

        public Vector2 Position {
            get
            {
                return (Vector2)GDWindow.Position;
            }
            set {
                GDWindow.Position = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return (Vector2)GDWindow.Size;
            }
            set
            {
                GDWindow.Size = value;
            }
        }

        public bool AlwaysOnTop
        {
            get
            {
                return GDWindow.AlwaysOnTop;
            }
            set
            {
                GDWindow.AlwaysOnTop = value;
            }
        }

        public bool Borderless
        {
            get
            {
                return GDWindow.Borderless;
            }
            set
            {
                GDWindow.Borderless = value;
            }
        }

        public bool Transparent
        {
            get
            {
                return GDWindow.TransparentBg;
            }
            set
            {
                GDWindow.Transparent = true;
                GDWindow.TransparentBg = value;
            }
        }
        
        public bool Unfocusable
        {
            get
            {
                return GDWindow.Unfocusable;
            }
            set
            {
                GDWindow.Unfocusable = value;
                /*
                if (GDWindow.Unfocusable)
                {
                    GDWindow.GuiReleaseFocus();
                } else
                {
                    //GDWindow.SetMode(Godot.Window.ModeEnum.Maximized);
                    GDWindow.GrabFocus();
                }*/
                GDWindow.Visible = false;
                GDWindow.Visible = true;
            }
        }

        public bool Visible
        {
            get
            {
                return GDWindow.Visible;
            }
            set
            {
                GDWindow.Visible = value;
            }
        }
#else
        public Vector2 Position
        {
            get; set;
        }

        public Vector2 Size
        {
            get; set;
        }
        public bool Visible { get; set; }
        public bool Transparent { get; set; }
        public bool Unfocusable { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool Borderless { get; set; }
#endif

        Image highlightImage;

#if TEMP
        public Canvas Canvas
        {
            get
            {
                return canvasGO.GetComponent<Canvas>();
            }
        }

        public Element Element
        {
            get
            {
                return canvasGO.AddOrGet<Element>();
            }
        }
#endif

#if UNITY
        protected Sequence sequence;
#elif WEB

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // TODO: Possibly add region separation back in
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, nameof(this.Name), Microsoft.AspNetCore.Components.BindConverter.FormatValue(Name));
            builder.AddAttribute(2, "style", "width: 100%; height: 100%; margin: 0px; padding: 0px; position: fixed; left: 0; top: 0;"); //Microsoft.AspNetCore.Components.BindConverter.FormatValue(ChildrenDirty));
            //builder.OpenRegion(2);
            RenderChildren(builder);
            //builder.CloseRegion();
            builder.CloseElement();
        }

#endif

        //[SerializeField]
        protected Node3D renderGO;
        //[SerializeField]
        protected Node3D canvasGO;
        /*
        protected override void Render() {

        }
        */

        public void ToggleVisibility()
        {
            throw new System.NotImplementedException();
        }

        public void Close(bool isImmediate = false)
        {
            
            //SetVisibility(false, isImmediate);
        }

        protected void Render(RenderMode renderMode)
        {
#if TEMP
            if (userClientAM.IsDesktop)
            {
                gameObject.Canvas().Hold<GraphicRaycaster>().Hold<CanvasScaler>(canvasScaler =>
                {
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvasScaler.matchWidthOrHeight = .5f;
                    canvasScaler.referenceResolution = new Vector2(1080, 1080);
                });
            }
            else
            {
                gameObject.Canvas().FitSize().Hold<CurvedUI.CurvedUISettings>(y =>
                {
                    y.Angle = 0;
                });
            }

            canvasGO = gameObject.Name("Canvas").VList().Hold(
                renderGO = AUI.UI.Name("Render").VList(0, 0).ExpandHeight(true).RoundImage(AUI.GetMaterial("Orange"))
            );

            renderGO.Hold<HorizontalOrVerticalLayoutGroup>(x =>
            {
                x.childForceExpandHeight = true;
                x.childForceExpandWidth = true;
            });

            canvasGO.Hold<Canvas>(canvas =>
            {
                canvas.renderMode = renderMode;
                //canvas.worldCamera = renderMode.Equals(RenderMode.WorldSpace) ? null : ClientRoomApp.Controller.Head.GetComponentInChildren<Camera>();
                canvas.planeDistance = GetPlaneDistance();
            });

            canvasGO.Hold<HorizontalOrVerticalLayoutGroup>(x => x.enabled = renderMode.Equals(RenderMode.WorldSpace));
            if (!renderMode.Equals(RenderMode.WorldSpace))
                renderGO.Stretch();

            highlightImage = renderGO.AddOrGet<Image>();
            Highlight(false);
#endif
        }
        /*
        public virtual Sprite GetIcon()
        {
            return AUI.GetSprite(GetName());
        }
        */
        // Curves the window's canvas according to an angle
        public void Curve(int angle)
        {
#if TEMP
            canvasGO.AddOrGet<CurvedUI.CurvedUISettings>().Angle = angle;
#endif
        }

        public override void Update()
        {
            UpdateRotation();
            base.Update();
        }

        public virtual void UpdateRotation()
        {
#if TEMP
            transform.rotation = userClientAM.GetModule<UserClientAM>().IsDesktop ? Quaternion.identity :
                Quaternion.LookRotation(transform.position - userClientAM.GetModule<UserClientAM>().Controller.Head.transform.position);
#endif
        }

        public void Highlight(bool value)
        {
            var padding = value ? AUI.SmallSpace : 0;
#if TEMP
            highlightImage.gameObject.Hold<LayoutGroup>(x => x.padding = new RectOffset(padding, padding, padding, padding));
            highlightImage.enabled = value;
#endif
        }
        
        public void SetVisibility(bool isVisible, bool isImmediate = false)
        {
            //replace with temp code at some point
            //base.SetVisibility(isVisible, isImmediate);
#if TEMP
            if (isVisible)
                TryRender();

            if (userClientAM.GetModule<UserClientAM>().IsDesktop)
            {
                base.SetVisibility(isVisible, isImmediate);
                return;
            }

            sequence?.Kill();
            sequence = DOTween.Sequence();

            IsVisible = isVisible;

            float targetScale = isVisible ? defaultScale : .001f;

            if (!Application.isPlaying || isImmediate)
            {
                transform.localScale = targetScale * Vector3.one;
                gameObject.SetActive(IsVisible);
            }
            else
            {
                if (IsVisible)
                    gameObject.SetActive(true);
                sequence.Append(transform.DOScale(targetScale, .25f).SetEase(Ease.InQuad));
                if (!IsVisible)
                    sequence.OnComplete(() => gameObject.SetActive(false));
            }

            if (!isImmediate)
                OnTransition();

            if (isVisible)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
            sequence.Play();
#endif
        }

#if GODOT
        protected Godot.Window GDWindow => GDNode as Godot.Window;

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Window() { Title = GetType().Name.Replace("Window", null).SplitCamelCase(), Transparent = true };
        }
#endif
    }
}