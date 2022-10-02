using Altimit.Unity;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Altimit.UI
{
    public static partial class UnityUIExtensions
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
#if UNITY_5_3_OR_NEWER
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
        public static Node Hold(this Node node)
        {
            return node;
        }

        public static Node Hold(this Node go, Action<Node> func)
        {
            func(go);
            return go;
        }

        //Adds or gets a component
        public static GameObject Hold<T>(this GameObject go) where T : Component
        {
            T t;
            return go.Hold<T>(out t);
        }

        //Adds or gets a component
        public static GameObject Hold<T>(this GameObject go, out T t) where T : Component
        {
            if (go == null)
                throw new Exception("Attempted to hold a component using a Node that doesn't exist!");
            t = go.AddOrGet<T>();
            return go;
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static GameObject Hold<T>(this GameObject go, Action<T> func) where T : Component
        {
            T t;
            return go.Hold<T>(func, out t);
        }

        //Adds or gets a component and sets values for it
        //Example: New<HorizontalLayoutGroup>(x => { x.padding = new RectOffset(0,0,0,0); });
        public static GameObject Hold<T>(this GameObject go, Action<T> func, out T t) where T : Component
        {
            return go.Hold(false, func, out t);
        }


        public static GameObject Hold<T>(this GameObject go, bool includeChildren, Action<T> func) where T : Component
        {
            T t;
            return go.Hold<T>(includeChildren, func, out t);
        }

        public static GameObject Hold<T>(this GameObject go, bool includeChildren, out T t) where T : Component
        {
            t = go.AddOrGet<T>(includeChildren);
            return go;
        }

        public static GameObject Hold<T>(this GameObject go, bool includeChildren, Action<T> func, out T t) where T : Component
        {
            t = go.AddOrGet<T>(includeChildren);
            func(t);
            return go;
        }

        //Sets a Node's children
        public static Node Hold(this Node go, params Action<Node>[] children)
        {
            //children.ToList().ForEach(x => { x.transform.SetParent(go.transform); });
            return go;
        }

        public static GameObject HoldFirst(this GameObject go, params GameObject[] children)
        {
            children.ToList().ForEach(x => { x.transform.parent = go.transform; x.transform.SetAsFirstSibling(); });
            return go;
        }

        //Sets a Node's children
        public static GameObject Hold(this GameObject go, params Component[] children)
        {
            //bool resetChildren = (node.GetComponent<RectTransform>() != null);
            children.ToList().ForEach(x => { x.transform.parent = go.transform; }); //.SetParent(node, resetChildren, resetChildren, resetChildren); });
            return go;
        }

        /*
        //Sets a Node's children
        public static Node Switch(this Node go, Node target)
        {
            go.Release();
            go.Hold(target);
            return go;
        }*/

        //Sets a Node's children
        /*
        public static Node Release(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            node.transform.DetachChildren();
            return node;
#elif GODOT
            return null;
#endif
        }
        */

        // TODO: Call generic version instead, or have generic refer to this
        public static Component AddOrGet(this GameObject node, Type type, bool includeChildren = false)
        {
            var comp = node.Get(type, includeChildren);
            if (comp == null)
                return node.gameObject.AddComponent(type);
            return comp;
        }

        //Adds or gets a component
        public static T AddOrGet<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return (T)go.AddOrGet(typeof(T), includeChildren);
        }

        //Returns if a Node has a component or not
        public static bool Has<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return go.Get<T>(includeChildren) != null;
        }

        /*
        //Gets a component
        public static T GetInParent<T>(this Node go) where T : Node
        {
            return (go == null ? null : go.GetComponentInParent<T>());
        }

        //Gets a component
        public static T[] GetInParents<T>(this Node node) where T : Node
        {
            return (node == null ? new T[0] : node.GetComponentsInParent<T>());
        }
        */

        //Gets a component
        public static T Get<T>(this GameObject go, bool includeChildren = false) where T : Component
        {
            return (T)go.Get(typeof(T), includeChildren);
        }

        public static Component Get(this GameObject go, Type type, bool includeChildren = false)
        {
            if (go == null)
                throw new Exception("Tried setting component on a null Node!");

            if (includeChildren)
                return go.GetComponentInChildren(type);

            return go.GetComponent(type);
        }

        //Gets a component
        public static Node Get(this Node go)
        {
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

        public static GameObject OnHeld(this GameObject go, Action<GameObject> func)
        {
            go.Hold<ParentObserver>(x =>
            {
                x.onUpdateSingle += func;
            });
            return go;
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
#if UNITY_5_3_OR_NEWER
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