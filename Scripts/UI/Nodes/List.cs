using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType(true)]
    public abstract class List : Control
    {
        [AProperty]
        public abstract Anchor Anchor { get; set; }
        [AProperty]
        public abstract bool ExpandChildWidth { get; set; }
        [AProperty]
        public abstract bool ExpandChildHeight { get; set; }
        public bool FitSize
        {
            set
            {
                FitWidth = value;
                FitHeight = value;
            }
        }
        [AProperty]
        public abstract bool FitWidth { get; set; }
        [AProperty]
        public abstract bool FitHeight { get; set; }

        public float AllSpace
        {
            set
            {
                Padding = value;
                Spacing = value;
            }
        }

        [AProperty]
        public abstract float Padding { get; set; }
        [AProperty]
        public abstract float Spacing { get; set; }

        public List() : base()
        {

        }
    }
}
