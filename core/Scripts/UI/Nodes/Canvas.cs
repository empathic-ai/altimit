using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
#endif

namespace Altimit.UI
{
    [AType]
    public class Canvas : Control
    {
        public bool isPreviewed = false;
        protected bool isDestroyed = false;

        //public bool IsSubWindow = false;
        protected float defaultScale = .00175f;

        protected bool isRendered = false;

        protected virtual void Render()
        {

        }

        public virtual string GetName()
        {
            return this.GetType().Name.Replace("Window", null);
        }

        public virtual void SetVisibility(bool isVisible, bool isImmediate = false)
        {
            base.SetVisibility(isVisible);

            /*
#if UNITY_5_3_OR_NEWER
            gameObject.SetActive(isVisible);
#endif
            */
            if (isVisible)
            {
                TryRender();
            }
        }

        public void TryRender()
        {
            if (!isRendered)
            {
                Render();
                isRendered = true;
            }
        }


        // Positions a window in front of the player
        public virtual Canvas UpdatePosition(bool isImmediate = true)
        {
            return this;
        }
#if UNITY
        [AProperty]
        public RenderMode RenderMode
        {
            get
            {
                return (RenderMode)GameObject.AddOrGet<UnityEngine.Canvas>().renderMode;
            }
            set
            {
                GameObject.AddOrGet<UnityEngine.Canvas>().renderMode = (UnityEngine.RenderMode)value;
            }
        }

        /*
#if UNITY_5_3_OR_NEWER
                [HideInInspector]
#endif
        */


        public void OnEnable()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected UnityEngine.Canvas canvas { get; private set; }

        public Canvas() : base()
        {
            canvas = GameObject.AddComponent<UnityEngine.Canvas>();
            canvas.planeDistance = 1;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal;

            GameObject.AddComponent<GraphicRaycaster>();
            GameObject.AddComponent<CanvasScaler>();
        }

        public override void Start()
        {
            base.Start();

            /*
#if UNITY_5_3_OR_NEWER
            gameObject.layer = LayerMask.NameToLayer("UI");
#endif
            */
            /*
            if (OS.Settings.IsPlaying)
            {
                //Clear();
                TryRender();
            }*/
        }


        public virtual void Init()
        {
        }

        public virtual bool IsExposed()
        {
            return true;
        }

        public virtual int GetPlaneDistance()
        {
            return 100;
        }

        public virtual void Clear()
        {
            /*
#if UNITY_5_3_OR_NEWER
            isRendered = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                if (!Application.isPlaying)
                {
                    DestroyImmediate(go);
                }
                else
                {
                    Destroy(go);
                }
            }
#elif GODOT
#endif
            */
        }

        /*
        public void AddSubWindow(Window window)
        {
            window.transform.parent = transform;
            window.SetSubWindow(true);
        }
        
        public void SetSubWindow(bool value)
        {
            IsSubWindow = value;
            gameObject.Hold<ContentSizeFitter>(x => x.enabled = IsSubWindow);
            gameObject.HList(TextAnchor.MiddleCenter).Hold<HorizontalLayoutGroup>(x => x.enabled = IsSubWindow);
            canvasGO.transform.localScale = Vector3.one * (IsSubWindow ? 1 : defaultScale);
            renderGO.transform.localScale = Vector3.one;
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            if (value)
            {
                UpdateSubWindowVisiblity();
                Destroy(canvasGO.GetComponent<CurvedUI.CurvedUISettings>());
                Destroy(canvasGO.GetComponent<CurvedUI.CurvedUIRaycaster>());
                Destroy(canvasGO.GetComponent<CanvasScaler>());
                Destroy(canvasGO.GetComponent<GraphicRaycaster>());
                Destroy(canvasGO.GetComponent<Canvas>());
            }
        }
        */

        public override void Update()
        {
        }
        


        public void Open(bool isImmediate = false)
        {
            SetVisibility(true, isImmediate);
        }


        void UpdateSubWindowVisiblity()
        {
            //gameObject.IgnoreLayout(!isVisible);
        }


        protected virtual void OnTransition()
        {
            //SoundManager.PlaySFX("Swipe");
        }

        protected virtual void OnShow()
        {
        }


        protected virtual void OnHide()
        {

        }



        /*
        public async void DelayHide(int duration, CancellationToken cancelationToken)
        {
            try
            {
                await Task.Delay(duration, LinkToken(cancelationToken));
                Hide();
            } catch (System.OperationCanceledException e)
            {
            }
        }*/

        public static int GetWaitTime(string text)
        {
            //''' Calculate the amount of time needed to read the notification '''
            var wpm = 180;  // readable words per minute
            var word_length = 5;  // standardized number of chars in calculable word
            var words = text.Length / word_length;
            var words_time = ((words / wpm) * 60) * 1000;

            var delay = 1500;  // milliseconds before user starts reading the notification
            var bonus = 2000;  // extra time

            return (delay + words_time + bonus);
        }
#endif
    }
}