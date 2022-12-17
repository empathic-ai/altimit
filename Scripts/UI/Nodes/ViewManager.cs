using System.Collections.Generic;
/*
#if UNITY_5_3_OR_NEWER
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEditor;

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
using System.Linq;
using System;

namespace Altimit.UI
{
    [AType(true)]
    public partial class ViewManager : Control
    {
        public Canvas Window;
        public List<View> ViewHistory = new List<View>();
        public Control BackButtonGO;
        //public UnityEngine.UI.Button BackButton;
        /*
        public bool AllowSnapping
        {
            get
            {
                return allowSnapping;
            }
            set
            {
                allowSnapping = value;
                if (!allowSnapping)
                    SizeFitter.enabled = false;
            }
        }*/

        bool allowSnapping = true;
        /*
        ContentSizeFitter SizeFitter
        {
            get
            {
                if (sizeFitter == null)
                {
                    sizeFitter = gameObject.AddOrGet<ContentSizeFitter>();
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
                return sizeFitter;
            }
        }
        ContentSizeFitter sizeFitter;
        */
        public View CurrentView;
        Node lastSelectedGO = null;

        // Use this for initialization
        void Awake()
        {
            /*
#if UNITY_5_3_OR_NEWER
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                Selection.selectionChanged += UpdateSelectedPanel;
#endif
                return;
            }

            Show(gameObject.AddOrGet<View>());
#endif
            */
        }

        public override void Start()
        {
            base.Start();
            /*
#if UNITY_5_3_OR_NEWER
            if (!Application.isPlaying) return;
            if (BackButtonGO)
            {
                BackButtonGO.Get<Button>(true).onClick.AddListener(UI_ShowLastPanel);
                BackButtonGO.SetActive(false);
            }
#endif
            */
        }

        public void OnEnable()
        {

        }

        void OnDisable ()
        {
            /*
#if UNITY_EDITOR
            Selection.selectionChanged -= UpdateSelectedPanel;
#endif
            */
        }

        public void Show<T>(HistoryType historyType = HistoryType.ClearHistory) where T : View
        {
            var view = GetView<T>();
            Show(view, historyType);
        }

        public T GetView<T>() where T : View
        {
            return this.FindNodeInChildren<T>(true);
        }

        public void Show<T>(Action<T> onShow, HistoryType historyType = HistoryType.ClearHistory) where T : View
        {
            var view = GetView<T>();//(T)GetChildPanels(gameObject).Single(x => x.GetType() == typeof(T));
            if (view == null)
                OS.LogError($"Failed to find view of type {typeof(T)}!");
            Show(view, historyType);
            onShow?.Invoke(view);
        }

        public void Show(View panel, HistoryType historyType = HistoryType.ClearHistory)
        {
            if (panel == null)
                return;

            //if (historyType.Equals(HistoryType.AddHistory) && currentPanel != null)
            //{
            //    if (PanelHistory.Count == 0 || PanelHistory[PanelHistory.Count-1] != currentPanel)
            //    PanelHistory.Add(currentPanel);
            //}

            //Debug.Log(gameObject.name + " is showing panel " + panel.gameObject.name);

            View lastPanel = panel;

            //Iterates through parent panels.
            View firstPanel = panel;
            View parentPanel = firstPanel.Parent.FindNodeInParent<View>();
            while (parentPanel != null)
            {
                parentPanel.SetVisibility(true);
                GetChildPanels(parentPanel).ForEach(x => { if (x.IsAutoManaged()) x.SetVisibility(x == firstPanel); });
                firstPanel = parentPanel;
                parentPanel = parentPanel.Parent.FindNodeInParent<View>();
            }
            //Debug.Log(lastPanel.gameObject.name + ", " + (contentSizeFitter == null).ToString() + ", " + lastPanel.SizeFit);

            //SizeFitter.enabled = AllowSnapping && lastPanel.SizeFit;
            CurrentView = panel;

            if (OS.Settings.IsPlaying)
            {
                if (historyType.Equals(HistoryType.ClearHistory))
                    ClearHistory();

                if (!historyType.Equals(HistoryType.IgnoreHistory))
                {
                    if (ViewHistory.Count == 0 || ViewHistory[ViewHistory.Count - 1] != panel)
                        ViewHistory.Add(panel);
                }

                panel.Show(historyType);
            }

            //Iterates through child panels. The first child panel found is set active by default, while all others are deactivated
            List<View> childPanels = GetChildPanels(lastPanel);
            while (childPanels.Count > 0)
            {
                childPanels.ForEach(x => {
                    if (x.IsAutoManaged())
                        x.SetVisibility(childPanels.IndexOf(x) == 0);
                });
                lastPanel = childPanels[0];
                childPanels = GetChildPanels(lastPanel);
            }
            OnShowPanel(panel);
        }

        /*
        possibly uncomment

        // Update is called once per frame
        void UpdateSelectedPanel()
        {
            Node selectedGO = null;

            selectedGO = Selection.activeGameObject;

            if (selectedGO != null && selectedGO != lastSelectedGO)
            {
                View panel = selectedGO.FindNodeInParent<View>();
                //Debug.Log(panel.IsShowing());
                if (panel != null && (CurrentView == null || !(panel.GetComponentsInChildren<View>().Contains(CurrentView) || CurrentView.GetComponentsInChildren<View>().Contains(panel))))
                    Show(panel);
            }
            lastSelectedGO = selectedGO;
        }
        */

        List<View> GetChildPanels (Node node)
        {
            List<View> subPanels = new List<View>();
            foreach (var child in node.Children)
            {
                View subView = child as View;
                if (subView != null)
                {
                    subPanels.Add(subView);
                } else
                {
                    subPanels.AddRange(GetChildPanels(child));
                }
            }
            return subPanels;
        }

        Action<View> onShowView;

        public void OnShowPanel (View viewHistory)
        {
            //LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.rectTransform());
            if (BackButtonGO != null)
                BackButtonGO.SetVisibility(ViewHistory.Count > 1);
#if TEMP
            Canvas.ForceUpdateCanvases();
#endif
            onShowView?.Invoke(viewHistory);
        }

        public void UI_ShowLastPanel ()
        {
            ShowLastPanel();
        }

        public void ClearHistory ()
        {
            ViewHistory.Clear();
        }
        public void RemoveLastHistory()
        {
            if (ViewHistory.Count > 0)
                ViewHistory.RemoveAt(ViewHistory.Count-1);
        }
        public void ShowLastPanel ()
        {
            if (ViewHistory.Count <= 1)
                return;

           // if (PanelHistory[PanelHistory.Count-1].HistoryType.Equals(HistoryType.IgnoreHistory))
            ViewHistory.RemoveAt(ViewHistory.Count - 1);
            Show(ViewHistory[ViewHistory.Count - 1], HistoryType.IgnoreHistory);//.OnStart(()=>PanelHistory.RemoveAt(PanelHistory.Count-1));
        }
    }
}
