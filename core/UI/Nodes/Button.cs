using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public class Button : Control
    {
#if GODOT
        public event Action OnPointerClick
        {
            add
            {
                GDButton.Pressed += value;
            }
            remove
            {
                GDButton.Pressed -= value;
            }
        }

        public Button() : base()
        {
        }

        public Godot.Button GDButton => GDNode as Godot.Button;

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Button();
        }
#endif
    }
}
