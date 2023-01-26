using System;
/*
#if UNITY_64
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

    public partial class WindowView<T> : ManagedView where T : Canvas
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
    public partial class View : Node
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

        public bool IsVisible { get; set; } = false;

        protected ViewManager viewManager => FindNodeInParent<ViewManager>();

        public Action OnSubmit;

        bool isRendered = false;

        public override void Start()
        {
        }

        bool hasBeenEnabled = false;

        protected virtual void Render()
        {
            //renderGO = gameObject;
        }

        public virtual void SetVisibility(bool isVisible)
        {
            if (IsVisible == isVisible)
                return;

            IsVisible = isVisible;
            //base.SetVisibility(isVisible);

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
        
        public virtual void Show(HistoryType historyType = HistoryType.ClearHistory)
        {
            //if (UseLoader && stopLoading)
            //    loaderGO.SetActive(false);
            SetVisibility(true);
        }

        /*
#if UNITY_64
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