using System;
using System.Collections.Generic;
using System.Linq;

namespace Altimit.UI
{
    public partial class SystemManager : Node
    {
        public AList<Window> Windows = new AList<Window>();
        //public BoundList<Window> ExposedWindows = new BoundList<Window>();
        public const bool DebugMessages = false;
#if UNITY_5_3_OR_NEWER
        public void Awake()
        {
            Init();
        }

        void Init()
        {
            gameObject.name = "[OS]";
            foreach (var window in gameObject.GetComponentsInChildren<Window>())
            {
                AddWindow(window);
            }

            Windows.OnAdded += ItemAdded;
            Windows.OnRemoved += ItemRemoved;
        }

        public void ItemAdded(Window window)
        {
            //var window = (Window)args.item;
            //if (window.IsExposed())
            //    ExposedWindows.Add(window);
        }

        public void ItemRemoved(Window window)
        {
            //ExposedWindows.Remove((Window)args.item);
            //Debug.Log(Windows.Contains((Window)args.item));
        }

        ~SystemManager()
        {
            Windows.OnAdded -= ItemAdded;
            Windows.OnRemoved -= ItemRemoved;
        }


        public void CloseWindow<T>(bool isImmediate = false) where T : Window
        {
            T window = GetWindow<T>();
            if (window != null)
                window.Close(isImmediate);
        }

        public T GetWindow<T>() where T : Window
        {
            return (T)GetWindow(typeof(T));
        }

        public Window GetWindow(Type type)
        {
            return Windows.SingleOrDefault(x => x.GetType() == type);
        }

        public T ToggleWindow<T>() where T : Window
        {
            T window = GetWindow<T>();
            if (window == null)
            {
                return CreateWindow<T>();
            }

            window.ToggleVisibility();
            return window;
        }

        public Window AddOrGetWindow(Type type, bool setVisible = true)
        {
            Window window = GetWindow(type);
            if (window == null)
            {
                window = AddWindow(type, setVisible);
            }
            else
            {
                if (setVisible)
                    window.SetVisibility(true);
                //Debug.Log(window.gameObject.name);
                //Debug.Log(window.gameObject.activeSelf);
            }
            return window;
        }

        public T AddOrGetWindow<T>(bool isVisible = true) where T : Window
        {
            return (T)AddOrGetWindow(typeof(T), isVisible);
        }

        protected virtual Window AddWindow(Type type, bool isVisible = true)
        {
            GameObject windowGO = new GameObject();
            windowGO.AddComponent(type);
            var window = (Window)windowGO.GetComponent(type);

            //if (window.IsCompatible(this))
            //{
            AddWindow(window);
            window.SetVisibility(isVisible);
            window.UpdatePosition();
                //window.Focus(true);
                //window.SetDisplay(isOpen);
            //} else
            //{
              //  Destroy(windowGO);
              //  window = null;
            //}
            //windowGO.transform.localScale = Vector3.one;
            return window;
        }

        public virtual void AddWindow(Window window)
        {
            window.gameObject.SetParent(gameObject, false);
            window.SystemManager = this;
            Windows.Add(window);
            //window.OnSystemStarted(this);
        }

        public T CreateWindow<T>() where T : Window
        {
            return (T)AddWindow(typeof(T));
        }

        public Window[] GetAllWindows()
        {
            return Windows.ToArray();
        }


        public void CloseWindow(Window window, bool isImmediate = false)
        {
            if (DebugMessages)
                Debug.Log("Closing window named " + window.GetName());
            //Windows.Remove(window);
            window.Close(isImmediate);
        }

        public void CloseWindows(IEnumerable<Window> windows, bool isImmediate = false)
        {
            foreach (var window in windows)
                CloseWindow(window, isImmediate);
        }

        public void CloseAllWindows(bool isImmediate = false)
        {
            if (DebugMessages)
                Debug.Log("Closing all windows");
            CloseWindows(GetAllWindows(), isImmediate);
        }
#endif


    }
}
