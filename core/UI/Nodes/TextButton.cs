using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public class TextButton : Button
    {
        [AProperty]
        public string Text {
            get
            {
                return label.Text;
            }
            set {
                label.Text = value;
            }
        }

        [AProperty]
        public Material Material { get; set; }

        protected Label label;

        public TextButton() : base()
        { 
            this.AddChildren(
                new HList() { ContainerAnchor = Anchor.MiddleCenter }.Stretch().AddChildren(
                    new Image() { Sprite = AUI.GetSprite("Circle"), ImageType = ImageType.Sliced, IgnoreLayout = true, Material = AUI.Purple, IsClickable = true }.Stretch(),
                    label = new Label() { Anchor = Anchor.MiddleCenter }
                )
            );
        }
    }
}
