using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.UI;

namespace Altimit.UI
{
    [AType]
    public class MaskButton : Button
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

        Image image;

        public override void Start()
        {
            base.Start();
        }

        public MaskButton() : base()
        {
            this.AddChildren(
                new Image() { Sprite = AUI.GetSprite("Circle"), ImageType = ImageType.Sliced, IsMask = true, Size = Vector2.One * AUI.SmallSize, IsClickable = true }.AddChildren(
                    image = new Image() { Material = AUI.GetMaterial("Mask") }.Stretch()
                )
            );
        }
    }
}
