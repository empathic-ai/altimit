using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
#if UNITY_5_3_OR_NEWER
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = UnityEngine.Material;
using Font = TMPro.TMP_FontAsset;
using Node = UnityEngine.GameObject;
using DG.Tweening;
using Component = UnityEngine.MonoBehaviour;
#elif GODOT
using Godot;
using Node = Godot.Node;
using Component = Godot.Node;
using Sprite = Godot.Texture;
#endif
//using UnityEngine.InputSystem;

namespace Altimit.UI
{
#if UNITY_5_3_OR_NEWER
    [ExecuteInEditMode]
#endif
    public partial class Window : Node
    {
        public bool IsVisible = false;
        public SystemManager SystemManager;

#if UNITY_5_3_OR_NEWER
        [HideInInspector]
#endif

        public bool isPreviewed = false;
        protected bool isDestroyed = false;

        //public bool IsSubWindow = false;
        protected float defaultScale = .00175f;
#if UNITY_5_3_OR_NEWER
        [SerializeField]
#endif
        protected bool isRendered = false;

        public void OnEnable()
        {
        }

        protected virtual void Awake()
        {
#if UNITY_5_3_OR_NEWER
            gameObject.layer = LayerMask.NameToLayer("UI");
#endif
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void Start()
        {
#if UNITY_5_3_OR_NEWER
            if (Application.isPlaying)
            {
                //Clear();
                TryRender();
            }
#elif GODOT
#endif

        }

        public virtual string GetName()
        {
            return this.GetType().Name.Replace("Window", null);
        }

        public virtual Sprite GetIcon()
        {
#if UNITY_5_3_OR_NEWER
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + GetName());
            //Debug.Log(GetName() + ", " + (sprite == null).ToString());
            return sprite;// Resources.Load<Sprite>("Windows/Sprites/" + GetName());
#elif GODOT
            return null;
#endif
            return null;
        }

        protected virtual void Render()
        {

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

        public virtual void Update()
        {
        }
        
        public void Close(bool isImmediate = false)
        {
            SetVisibility(false, isImmediate);
        }

        public void Open(bool isImmediate = false)
        {
            SetVisibility(true, isImmediate);
        }

        public void ToggleVisibility()
        {
            SetVisibility(!IsVisible);
        }

        void UpdateSubWindowVisiblity()
        {
            //gameObject.IgnoreLayout(!isVisible);
        }

        public virtual void SetVisibility(bool isVisible, bool isImmediate = false)
        {
#if UNITY_5_3_OR_NEWER
            gameObject.SetActive(isVisible);
#endif
        }

        protected virtual void OnTransition()
        {
            //SoundManager.PlaySFX("Swipe");
        }

        protected virtual void OnShow()
        {
        }

        public void TryRender()
        {
            if (!isRendered)
            {
                Render();
                isRendered = true;
            }
        }

        protected virtual void OnHide()
        {

        }

        // Positions a window in front of the player
        public virtual Window UpdatePosition(bool isImmediate = true)
        {
            return this;
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

        public static string AddSpacesToCamelcase(string text)
        {
            return Regex.Replace(text, "(\\B[A-Z])", " $1");
        }

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
    }
}