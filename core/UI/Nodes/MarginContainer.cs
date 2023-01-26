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
    public class MarginContainer : Control
    {
#if UNITY_64
        [AProperty]
        public int Margin
        {
            get; set;
        }
#elif GODOT
        [AProperty]
        public int Margin
        {
            get
            {
                return GDMarginContainer.GetThemeConstant("margin_top");
            }
            set
            {
                GDMarginContainer.AddThemeConstantOverride("margin_top", value);
                GDMarginContainer.AddThemeConstantOverride("margin_bottom", value);
                GDMarginContainer.AddThemeConstantOverride("margin_left", value);
                GDMarginContainer.AddThemeConstantOverride("margin_right", value);
            }
        }

        protected Godot.MarginContainer GDMarginContainer => GDNode as Godot.MarginContainer;

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.MarginContainer();
        }
#else
        [AProperty]
        public int Margin
        {
            get; set;
        }
#endif
    }
}