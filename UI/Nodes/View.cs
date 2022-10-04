using System;
/*
#if UNITY_5_3_OR_NEWER
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = UnityEngine.Material;
using Font = TMPro.TMP_FontAsset;
using Node = UnityEngine.GameObject;
using Component = UnityEngine.MonoBehaviour;
#elif GODOT
using Godot;
using Node = Godot.Node;
using Component = Godot.Node;
#endif
*/

namespace Altimit.UI
{
    public enum HistoryType
    {
        ClearHistory,
        AddHistory,
        IgnoreHistory
    }

    public interface IView
    {

    }

    public partial class WindowView<T> : ManagedView where T : Window
    {
        public T Window;
    }

    public partial class ManagedView : View
    {
        public override bool IsAutoManaged()
        {
            return true;
        }
    }

    [AType]
    public partial class View : Node2D, IView
    {
        //Panels by default hide other panels
        public virtual bool IsAutoManaged()
        {
            return false;
        }
 //       [ReadOnly]
        public View ParentPanel;
        public bool Snap = true;
        public bool SizeFit = false;

        protected ViewManager viewManager
        {
            get
            {
                /*
#if UNITY_5_3_OR_NEWER
                if (_panelManager == null)
                {
                    Transform parent = transform;
                    while (parent.GetComponent<ViewManager>()==null && parent.parent != null)
                    {
                        parent = parent.parent;
                    }
                    _panelManager = parent.GetComponent<ViewManager>();
                }
                if (_panelManager == null)
                {
                    OS.LogError("Failed to find panel manager for "+ GetType().Name+".");
                }

                return _panelManager;
#elif GODOT
                return null;
#endif
                */
                return null;
            }
            set
            {
                _panelManager = value;
            }
        }

        ViewManager _panelManager;

        public Action OnSubmit;

        public bool IsVisible = false;
        bool isRendered = false;

        public virtual void Awake()
        {
        }

        bool hasBeenEnabled = false;

        protected virtual void Render()
        {
            //renderGO = gameObject;
        }

        public virtual void SetVisibility(bool isVisible)
        {
            /*
            if (IsVisible == isVisible)
                return;

            IsVisible = isVisible;
            gameObject.SetActive(IsVisible);
            if (IsVisible)
            {
                if (!isRendered)
                {
                    InternalRender();
                    Render();
                    PostRender();
                    isRendered = true;
                }
                OnShow();
            }
            */
        }

        public virtual void InternalRender()
        {

        }

        public virtual void OnShow()
        {

        }

        public virtual void PostRender()
        {

        }

        /*
#if UNITY_5_3_OR_NEWER
        public void OnEnable()
        {

            if (!hasBeenEnabled)
            {
                if (IsAutoManaged())
                {
                    gameObject.SetActive(false);
                } else
                {
                    SetVisibility(true);
                }
                hasBeenEnabled = true;
                return;
            }
    }

    private void Start()
        {
            gameObject.Name(this.GetType().Name.Replace("View", null));
            //if (Element == null)
            //    Element = GetComponent<Binder>();
            //if (SubmitButton)
            //    SubmitButton.onClick.AddListener(Submit);
        }

        protected Action<GameObject> onRender;

        //TODO: seperate loader from default view and create loadview that inherits from this
        public void Init (Action<GameObject> onRender, GameObject[] childPanels)
        {
            this.onRender = onRender;
        }

        protected GameObject renderGO;

        public void OnDisable()
        {

        }

        public void Hide()
        {
            SetVisibility(false);
        }

        public virtual void Show(HistoryType historyType = HistoryType.ClearHistory)
        {
            //if (UseLoader && stopLoading)
            //    loaderGO.SetActive(false);
            SetVisibility(true);
        }

        public bool IsShowing ()
        {
            Transform parent = transform;
            do
            {
                if (!parent.gameObject.activeSelf)
                    return false;

                parent = parent.parent;
            }
            while (parent != null);

            return true;
        }
#elif GODOT
#endif
        */
    }
}