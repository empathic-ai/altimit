using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_64
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
#endif
namespace Altimit.UI
{
    [AType(true)]
    public class GridContainer : Container
    {
#if UNITY_64
        public override Anchor ContainerAnchor { get; set; }
        public override bool ExpandChildWidth { get; set; }
        public override bool ExpandChildHeight { get; set; }
        public override bool FitWidth { get; set; }
        public override bool FitHeight { get; set; }
        public override float Padding { get; set; }
        public override float Spacing
        {
            get; set;
        }

#elif GODOT
        public int Columns
        {
            get
            {
                return GDGridContainer.Columns;
            }
            set
            {
                GDGridContainer.Columns = value;
            }
        }
        public override Anchor ContainerAnchor { get; set; }
        public override bool ExpandChildWidth { get; set; }
        public override bool ExpandChildHeight { get; set; }
        public override bool FitWidth { get; set; }
        public override bool FitHeight { get; set; }
        public override float Padding { get; set; }
        public override float Spacing
        {
            get
            {
                return (float)GDGridContainer.GetThemeConstant("h_separation");
            }
            set
            {
                GDGridContainer.AddThemeConstantOverride("h_separation", (int)value);
                GDGridContainer.AddThemeConstantOverride("v_separation", (int)value);
            }
        }

        public Godot.GridContainer GDGridContainer => GDNode as Godot.GridContainer;

        public GridContainer() : base()
        {
            Spacing = 0;
        }

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.GridContainer();
        }
#else
        public int Columns
        {
            get; set;
        }
        public override Anchor ContainerAnchor { get; set; }
        public override bool ExpandChildWidth { get; set; }
        public override bool ExpandChildHeight { get; set; }
        public override bool FitWidth { get; set; }
        public override bool FitHeight { get; set; }
        public override float Padding { get; set; }
        public override float Spacing
        {
            get; set;
        }

        public GridContainer() : base()
        {
        }
#endif
    }
}