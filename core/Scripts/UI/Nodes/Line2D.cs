#if GODOT
using Godot;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public class Line2D : Node
    {
#if GODOT
        [AProperty]
        // TODO: investigate ways of easily binding property as well as subproperties/elements?
        public AList<Vector2> Points
        {
            get
            {
                var list = new AList<Vector2>();
                list.AddRange(GDLine2D.Points.Select(x => (Vector2)x));
                return list;
            }
            set
            {
                GDLine2D.Points = value.Select(x => new Godot.Vector2(x.x, x.y)).ToArray();
            }
        }

        [AProperty]
        public Color Color {
            get {
                return (Color)GDLine2D.DefaultColor;
            }
            set {
                GDLine2D.DefaultColor = (Godot.Color)value;
            }
        }

        [AProperty]
        public float Width
        {
            get
            {
                return GDLine2D.Width;
            }
            set
            {
                GDLine2D.Width = value;
            }
        }

        public Godot.Line2D GDLine2D => GDNode as Godot.Line2D;

        protected override Godot.Node GenerateGDNode()
        {
            return new Godot.Line2D();
        }
#else
        public int Width { get; set; }
        public Color Color { get; set; }
        public AList<Vector2> Points { get; set; }
#endif

        public Line2D() : base()
        {
            // TODO: Add remove in BindList
            //Points.BindList(x => GDLine2D.Points = Points.Select(x => new Godot.Vector2(x.x, x.y)).ToArray(),
            //    x => GDLine2D.Points = Points.Select(x => new Godot.Vector2(x.x, x.y)).ToArray());
        }

    }
}
