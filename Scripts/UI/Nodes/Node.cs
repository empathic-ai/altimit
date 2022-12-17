using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Altimit;
using Google.Protobuf.WellKnownTypes;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
using Altimit.UI.Unity;
#endif

namespace Altimit.UI
{
    [AType(true)]
    public class Node : IUpdateable, IEnumerable<Node>
    {

        static Dictionary<Godot.Node, Node> nodesByGDNodes = new Dictionary<Godot.Node, Node>();

        [AProperty]
        public Node Parent {
            get {
                return parent;
            }
            set {
                if (parent != value)
                {
                    var oldParent = parent;
                    if (parent != null)
                    {
                        parent.Children.Remove(this);
                        parent = null;
                    }
                    if (value != null)
                    {
                        value.Children.Add(this);
                        parent = value;
                    }
                    OnParentChanged();
                    OnAnyParentChanged();
                }
            }
        }

        public Node Root
        {
            get
            {
                Node _parent = this;
                while (_parent != null)
                {
                    _parent = _parent.Parent;
                }
                return _parent;
            }
        }

        private Node parent
        {
#if UNITY_2017_1_OR_NEWER
            get
            {
                if (GameObject.transform.parent == null)
                    return null;

                return GameObject.transform.parent.GetComponent<NodeMonoBehaviour>().Node;
            }
            set
            {
                if (value == null)
                {
                    GameObject.transform.parent = null;
                }
                else
                {
                    GameObject.transform.SetParent(value.GameObject.transform, true);
                }
            }
#elif GODOT
            get
            {
                if (GDNode.GetParent() == null)
                    return null;

                Node node;
                if (nodesByGDNodes.TryGetValue(GDNode.GetParent(), out node)) {
                    return node;
                }
                return null;
            }
            set
            {
                if (GDNode.GetParent() != null)
                    GDNode.GetParent().RemoveChild(GDNode);

                if (value != null)
                {
                    value.GDNode.AddChild(GDNode);
                }
            }
#else
            get; set;
#endif
        }

        [AProperty]
        public string Name {
            get {
                return GDNode.Name;
            }
            set
            {
                GDNode.Name = value;
            }
        }

        [AProperty]
        public AList<Node> Children { get; set; } = new AList<Node>();


        public Godot.Node GDNode;


        public Node()
        {
            GDNode = GenerateGDNode();
            nodesByGDNodes[GDNode] = this;

            GDNode.Name = this.GetType().Name;
            GodotSettings.SceneRoot.AddChild(GDNode);

            Updater.Instance.AddUpdateable(this);
            Updater.Instance.OnNextUpdate(Start);
        }

        protected virtual Godot.Node GenerateGDNode()
        {
            return new Godot.Node();
        }

        public void AddChild(Node child)
        {
            child.Parent = this;
        }

        public virtual void Destroy()
        {
       
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {
        }

        public virtual void OnParentChanged()
        {
        }

        public virtual void OnAnyParentChanged()
        {
            foreach (var child in Children)
            {
                child.OnAnyParentChanged();
            }
        }

        public T FindNodeInParent<T>(bool includeSelf = false) where T : Node
        {
            Node node = includeSelf ? this : Parent;
            while (node != null && !(node is T))
            {
                node = node.Parent;
            }

            if (!(node is T))
                return null;

            return node as T;
        }

        public T FindNodeInChildren<T>(bool includeSelf = false) where T : Node
        {
            if (includeSelf && this is T)
            {
                return this as T;
            }

            foreach (var child in Children)
            {
                var node = child.FindNodeInChildren<T>(true);
                if (node != null)
                {
                    return node;
                }
            }
            return null;
        }

        //Legacy
        public static string AddSpacesToCamelcase(string text)
        {
            return Regex.Replace(text, "(\\B[A-Z])", " $1");
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
