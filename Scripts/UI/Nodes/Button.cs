using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public class Button : Node
    {
        public event Action OnPointerClick
        {
            add
            {
                FindNodeInChildren<Control>().OnPointerClick += value;
            }
            remove
            {
                FindNodeInChildren<Control>().OnPointerClick -= value;
            }
        }

        public Button() : base()
        {
        }
    }
}
