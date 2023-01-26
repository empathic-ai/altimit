using System;
using System.Linq;
using System.Threading.Tasks;
using Random = System.Random;

namespace Altimit.UI
{
    public static partial class A
    {
        /*
        //Shortcut for adding action when panel shows
        public static Node OnShow(this Node go, object value)
        {
            go.AddOrGet<Panel>().onShowPanel += new Action<Node>(x => x.Set(value));
            return go;
        }
        */

        /*
        public static Node Update(this Node node)
        {
#if UNITY_64
            LayoutRebuilder.ForceRebuildLayoutImmediate(node.Get<RectTransform>());
            return node;
#elif GODOT
            return null;
#endif
        }
        */

        /*
        public static Node OnAwake(this Node go, Action<Node> onAwake)
        {
            return go.Hold<View>(x=> { x.onAwake += onAwake; });
        }

        //Shortcut for adding action when panel shows
        public static Node OnShow(this Node go, Action<Node> func)
        {
            go.AddOrGet<Panel>().onShow += func;
            return go;
        }*/

        //Dummy function
        public static Node AddChildren(this Node node)
        {
            return node;
        }

        public static Node Hold(this Node node, Action<Node> func)
        {
            func(node);
            return node;
        }

        //Adds or gets a component
        public static T Hold<T>(this T node) where T : Node
        {
            T t;
            return node.Hold<T>(out t);
        }

        //Adds or gets a component
        public static T Hold<T>(this T node, out T t) where T : Node
        {
            if (node == null)
                throw new Exception("Attempted to hold a component using a Node that doesn't exist!");
            t = node;
            return node;
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static Node Hold<T>(this Node node, Action<T> func) where T : Node
        {
            T t;
            return node.Hold<T>(func, out t);
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static Node Hold<T>(this Node go, Action<T> func, out T t) where T : Node
        {
            return go.Hold(false, func, out t);
        }


        public static Node Hold<T>(this Node node, bool includeChildren, Action<T> func) where T : Node
        {
            T t;
            return node.Hold<T>(includeChildren, func, out t);
        }

        public static Node Hold<T>(this Node node, bool includeChildren, out T t) where T : Node
        {
            t = node.AddOrGet<T>(includeChildren);
            return node;
        }

        public static Node Hold<T>(this Node node, bool includeChildren, Action<T> func, out T t) where T : Node
        {
            t = node.AddOrGet<T>(includeChildren);
            //Holder holder = go.AddOrGet<Holder>();
            //holder.OnHold += ()=> { func(t); };
            func(t);
            return node;
        }

        /*
        //Sets a Node's children
        public static Node Hold(this Node go, params Action<Node>[] children)
        {
            //children.ToList().ForEach(x => { x.transform.SetParent(go.transform); });
            return go;
        }

        public static Node HoldFirst(this Node node, params Node[] children)
        {
            
#if UNITY_64
            children.ToList().ForEach(x => { x.SetParent(node, true, true, true); x.transform.SetAsFirstSibling(); });
            return node;
#elif GODOT
            
            return null;
//#endif
        }
        */

        //Sets a Node's children
        public static T AddChildren<T>(this T node) where T : Node
        {
            return node;
        }

        public static T AddChildren<T>(this T node, params Node[] children) where T : Node
        {
            foreach (var child in children)
            {
                node.AddChild(child);
            }
            return node;
        }

        //Sets a Node's children
        public static Node Switch(this Node go, Node target)
        {
            go.Release();
            go.AddChildren(target);
            return go;
        }

        //Sets a Node's children
        public static Node Release(this Node node)
        {
            /*
#if UNITY_64
            node.transform.DetachChildren();
            return node;
#elif GODOT
            return null;
#endif
            */
            return node;
        }

        //Returns an empty Node
        public static Node New(string name = "")
        {
            Node node = new Node() { Name = (name == "" ? OS.Random.Next(0, 1000).ToString() : name) };
            return node;
        }

        // TODO: Call generic version instead, or have generic refer to this
        public static Node AddOrGet(this Node node, Type type, bool includeChildren = false)
        {
            /*
#if UNITY_64
            var comp = node.Get(type, includeChildren);
            if (comp == null)
                return node.gameObject.AddComponent(type);
            return comp;
#elif GODOT
            */
            return null;
//#endif
        }

        //Adds or gets a component
        public static T AddOrGet<T>(this Node go, bool includeChildren = false) where T : Node
        {
            return (T)go.AddOrGet(typeof(T), includeChildren);
        }

        //Returns if a Node has a component or not
        public static bool Has<T>(this Node go, bool includeChildren = false) where T : Node
        {
            return go.Get<T>(includeChildren) != null;
        }

        //Gets a component
        public static T GetInParent<T>(this Node node) where T : Node
        {
            /*
#if UNITY_64
            return (node == null ? null : node.GetComponentInParent<T>());
#elif GODOT
            return null;
#endif
            */
            return null;
        }

        //Gets a component
        public static T[] GetInParents<T>(this Node node) where T : Node
        {
            /*
#if UNITY_64
            return (node == null ? new T[0] : node.GetComponentsInParent<T>());
#elif GODOT
            return null;
#endif
            */
            return null;
        }

        //Gets a component
        public static T Get<T>(this Node node, bool includeChildren = false) where T : Node
        {
            return (T)node.Get(typeof(T), includeChildren);
        }

        public static Node Get(this Node node, Type type, bool includeChildren = false)
        {
            /*
#if UNITY_64
            if (node == null)
                throw new Exception("Tried setting component on a null Node!");
            if (includeChildren)
                return node.GetComponentInChildren(type);

            return node.GetComponent(type);
#elif GODOT
            return null;
#endif
            */
            return node;
        }

        //Gets a component
        public static Node Get(this Node go)
        {
            if (go == null)
                go = New();
            return go;
        }

        //Sets a Node's children
        public static Node Get<T>(this Node node, Action<T> func) where T : Node
        {
            if (node.Has<T>())
            {
                func(node.Get<T>());
            }
            return node;
        }

        public static T OnParentChanged<T>(this T node, Action<T> func) where T : Node
        {
            node.BindProperty(x => x.Parent, parent => {
                func(node);
            });
            /*
#if UNITY_64
            node.Hold<ParentObserver>(x =>
            {
                x.onUpdateSingle += func;
            });
#elif GODOT
#endif
            */
            return node;
        }

        public static Node OnNextFrame(this Node node, Action<Node> func)
        {
            ExecuteFunc(node, func);
            return node;
        }

        static async void ExecuteFunc(Node node, Action<Node> func)
        {
            await Task.Delay(2000);
            func(node);
        }

        /*
        public static Node SetParent(this Node node, Node parent, bool setPosition = false, bool setRotation = false, bool setScale = false)
        {
#if UNITY_64
            node.transform.SetParent(parent.transform);
            if (setPosition)
                node.transform.localPosition = Vector3.zero;
            if (setRotation)
                node.transform.localEulerAngles = Vector3.zero;
            if (setScale)
                node.transform.localScale = Vector3.one;
#elif GODOT
#endif
            return node;
        }
        */

        /*
        public static RectTransform rectTransform(this Node go)
        {
            return go.GetComponent<RectTransform>();
        }
        */

        public static Node Call(this Node go, Action<Node> action)
        {
            if (action != null)
                action(go);
            return go;
        }
    }
}