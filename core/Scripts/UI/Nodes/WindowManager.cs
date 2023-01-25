using System;
using System.Collections.Generic;
using System.Linq;
using Altimit.Networking;
using Altimit.UI;

namespace Altimit.UI
{
    public partial class WindowManager : Node
    {
        public App App;
        public AList<Window> Windows = new AList<Window>();
        public const bool DebugMessages = false;

        public override void Start()
        {
            Windows.OnAdded += ItemAdded;
            Windows.OnRemoved += ItemRemoved;
        }

        public void ItemAdded(Node window)
        {
        }

        public void ItemRemoved(Node window)
        {
        }

        ~WindowManager()
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
            var window = GetWindow(type);
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

        public T AddOrGetWindow<T>(bool setVisible = true) where T : Window
        {
            return (T)AddOrGetWindow(typeof(T), setVisible);
        }

        protected virtual Window AddWindow(Type type, bool isVisible = true)
        {
            Window window = (Window)Activator.CreateInstance(type);

            AddWindow(window);
            window.SetVisibility(isVisible);
            //TODO: ADd back in
            //window.UpdatePosition();

            return window;
        }

        public virtual void AddWindow(Window window)
        {
            window.Parent = this;
            Windows.Add(window);          
        }

        public T CreateWindow<T>() where T : Window
        {
            return (T)AddWindow(typeof(T));
        }

        public Node[] GetAllWindows()
        {
            return Windows.ToArray();
        }


        public void CloseWindow(Canvas window, bool isImmediate = false)
        {
                                    /* todo: uncomment after UI changes
            if (DebugMessages)
                Debug.Log("Closing window named " + window.GetName());
            window.Close(isImmediate);
                                    */
        }

        public void CloseWindows(IEnumerable<Canvas> windows, bool isImmediate = false)
        {
            foreach (var window in windows)
                CloseWindow(window, isImmediate);
        }

        public void CloseAllWindows(bool isImmediate = false)
        {
            /* todo: uncomment after UI changes
            if (DebugMessages)
            Debug.Log("Closing all windows");
            CloseWindows(GetAllWindows(), isImmediate);
            */
        }
    }
}
