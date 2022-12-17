using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit.UI;

namespace Altimit.UI
{
    [AType]
    public class ImageButton : Button
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

        public Material Material
        {
            get
            {
                return image.Material;
            }
            set
            {
                image.Material = value;
            }
        }

        Image image;

        public override void Start()
        {
            this.AddChildren(
                image = new Image() { Size = Vector2.One * AUI.SmallSize, UseShadow = true, IsClickable = true }
            );
        }

        public ImageButton() : base()
        {
        }

#if GODOT
        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.TextureButton();
        }
#endif
    }
}
