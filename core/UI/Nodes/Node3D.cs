using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_64
using UnityEngine;
using Altimit.UI.Unity;
#endif

namespace Altimit.UI
{
    [AType]
    public class Node3D : Node
    {
#if UNITY_64
        [AProperty]
        public bool IsVisible
        {
            get
            {
                return GameObject.activeSelf;
            }
            set
            {
                GameObject.SetActive(value);
            }
        }

        public Vector3 Position
        {
            get
            {
                return (Vector3)GameObject.transform.position;
            }
            set
            {
                GameObject.transform.position = value;
            }
        }

        [AProperty]
        public Vector3 LocalPosition
        {
            get
            {
                return (Vector3)GameObject.transform.localPosition;
            }
            set
            {
                GameObject.transform.localPosition = value;
            }
        }

        [AProperty]
        public Vector3 LocalScale
        {
            get
            {
                return (Vector3)GameObject.transform.localScale;
            }
            set
            {
                GameObject.transform.localScale = value;
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                return (Vector3)GameObject.transform.eulerAngles;
            }
            set
            {
                GameObject.transform.eulerAngles = value;
            }
        }

        [AProperty]
        public Vector3 LocalEulerAngles
        {
            get
            {
                return (Vector3)GameObject.transform.localEulerAngles;
            }
            set
            {
                GameObject.transform.localEulerAngles = value;
            }
        }

        public Vector3 Forward
        {
            get
            {
                return (Vector3)GameObject.transform.forward;
            }
        }

        public Vector3 Up
        {
            get
            {
                return (Vector3)GameObject.transform.up;
            }
        }

        public Vector3 Right
        {
            get
            {
                return (Vector3)GameObject.transform.right;
            }
        }

        public Node3D() : base()
        {
        }

        bool isParentNotRendered = false;

        public override void OnParentChanged()
        {
            base.OnParentChanged();
        }

        public override void OnAnyParentChanged()
        {
            base.OnAnyParentChanged();

            UpdateRendering();
        }

        void UpdateRendering()
        {
            var parent = Parent;
            string name = Name;
            while (parent != null && parent is not Node3D)
            {
                name = parent.Name + " -> " + name;
                parent = parent.Parent;
            }

            GameObject.name = name;

            //var parent = this.FindNodeInParent<Node3D>();
            if (parent != null)
            {
                GameObject.transform.SetParent(((Node3D)parent).GameObject.transform);
            }
            else
            {
                GameObject.transform.SetParent(null);
            }
        }

#elif GODOT
        public Vector3 Position
        {
            get
            {
                return (Vector3)GDNode3D.GlobalPosition;
            }
            set
            {
                // TODO: Possibly find better work around than using SetDeferred
                // Using because in Godot you usually wait for On_Ready to be called to change this property
                GDNode3D.SetDeferred("global_position", (Godot.Vector3)value);
            }
        }

        [AProperty]
        public Vector3 LocalPosition
        {
            get
            {
                return (Vector3)GDNode3D.Position;
            }
            set
            {
                GDNode3D.Position = (Godot.Vector3)value;
            }
        }

        [AProperty]
        public Vector3 LocalScale
        {
            get
            {
                return (Vector3)GDNode3D.Scale;
            }
            set
            {
                GDNode3D.Scale = (Godot.Vector3)value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return (Vector3)GDNode3D.GlobalRotation;
            }
            set
            {
                GDNode3D.GlobalRotation = (Godot.Vector3)value;
            }
        }

        [AProperty]
        public Vector3 LocalRotation
        {
            get
            {
                return (Vector3)GDNode3D.Rotation;
            }
            set
            {
                GDNode3D.Rotation = (Godot.Vector3)value;
            }
        }

        public Godot.Node3D GDNode3D => GDNode as Godot.Node3D;
        
        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Node3D();
        }
#else

        public Node3D() : base()
        {
            
        }
#endif
    }
}
