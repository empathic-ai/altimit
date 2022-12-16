using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.UI;

namespace Altimit.UI
{
    [AType]
    public class ImageLabelButton : Button
    {
        public Sprite Sprite
        {
            get
            {
                return image.Sprite;
            }
            set
            {
                image.Sprite = value;
            }
        }

        [AProperty]
        public string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        Image image;
        Label label;

        public override void Start()
        {
            base.Start();
        }

        public ImageLabelButton() : base()
        {
            this.AddChildren(
                new HList() { AllSpace = AUI.SmallSpace }.Stretch().AddChildren(
                    new Image() { Sprite = AUI.GetSprite("Circle"), ImageType = ImageType.Sliced, IgnoreLayout = true, Material = AUI.Purple }.Stretch(),
                    image = new Image() { Size = Vector2.One * AUI.TinySize },
                    label = new Label() { }
                )
            );
        }
    }
}
