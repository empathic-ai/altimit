//using UnityEngine;

#if UNITY
using Altimit.UI.Unity;
using DG.Tweening;
#endif

using Altimit;
using Altimit.UI;
using Altimit.Networking;

namespace Altimit.UI {
    public class Window : Canvas
    {
        public App App => FindNodeInParent<WindowManager>().App;

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
#endif

        //[SerializeField]
        protected Node3D renderGO;
        //[SerializeField]
        protected Node3D canvasGO;
        protected override void Render() {

        }
        public void Close(bool isImmediate = false)
        {
            SetVisibility(false, isImmediate);
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

        public virtual Sprite GetIcon()
        {
            return AUI.GetSprite(GetName());
        }

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

        public override void SetVisibility(bool isVisible, bool isImmediate = false)
        {
            //replace with temp code at some point
            base.SetVisibility(isVisible, isImmediate);
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
    }
}