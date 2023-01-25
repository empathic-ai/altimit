using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Altimit;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;
using Altimit.UI.Unity;
#elif WEB
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endif

namespace Altimit.UI
{
    [AType(true)]
    public class Node :
#if WEB
        ComponentBase,
        // TODO: Add in to directly embed nodes as components in razor
        //, IComponent
#endif
        IUpdateable, IEnumerable<Node>
    {
#if GODOT
        static Dictionary<Godot.Node, Node> nodesByGDNodes = new Dictionary<Godot.Node, Node>();
#endif
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
                        parent.OnChildrenChanged();
                        parent = null;
                    }
                    if (value != null)
                    {
                        value.Children.Add(this);
                        parent = value;
                        parent.OnChildrenChanged();
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
#elif WEB
            get; set;
#else
            get; set;
#endif
        }

        [AProperty]
        public AList<Node> Children { get; set; } = new AList<Node>();

#if GODOT
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

        public Godot.Node GDNode;
#else
#if WEB

        [Parameter]
#endif
        [AProperty]
        public string Name
        {
            get; set;
        }
#endif

        public Node() : base()
        {
#if GODOT
            GDNode = GenerateGDNode();
            nodesByGDNodes[GDNode] = this;

            //GDNode.Name = this.GetType().Name;
#endif
            Name = this.GetType().Name;
            Updater.Instance.AddUpdateable(this);
            Updater.Instance.OnNextUpdate(Start);
        }

#if GODOT
        protected virtual Godot.Node GenerateGDNode()
        {
            return new Godot.Node();
        }
#elif WEB

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        [AProperty]
        public bool ChildrenDirty { get; set; }

        public virtual RenderFragment? Content { get; }

        // To center: style: align-items: center; justify-content: center;
        public RenderFragment RenderFragment => BuildRenderTree;

        // How to add events using RenderTreeBuilder
        // https://stackoverflow.com/questions/60077877/events-in-blazor-rendertreebuilder
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            // TODO: Possibly add region separation back in
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, nameof(this.Name), Microsoft.AspNetCore.Components.BindConverter.FormatValue(Name));
            //builder.AddAttribute(2, nameof(this.ChildrenDirty), Microsoft.AspNetCore.Components.BindConverter.FormatValue(ChildrenDirty)); //Microsoft.AspNetCore.Components.BindConverter.FormatValue(ChildrenDirty));
            //builder.OpenRegion(2);
            RenderChildren(builder);
            //builder.CloseRegion();
            builder.CloseElement();
        }

        protected void RenderChildren(RenderTreeBuilder builder)
        {
            ChildContent?.Invoke(builder);
        }
#endif
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

        public virtual void OnChildrenChanged()
        {
#if WEB
            //OS.Log($"Child count changed on {Name}: {Children.Count} children.");

            ChildContent = builder =>
            {
                foreach (var child in Children)
                {
                    child.RenderFragment(builder);
                }
            };

            UpdateState();
#endif
        }

        protected void UpdateState()
        {
            if (isInitialized)
            {
                StateHasChanged();
            }
        }

        bool isInitialized = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            isInitialized = true;
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
