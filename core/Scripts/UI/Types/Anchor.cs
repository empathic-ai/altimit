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
    public enum Anchor
    {
        //
        // Summary:
        //     Text is anchored in upper left corner.
        UpperLeft = 0,
        //
        // Summary:
        //     Text is anchored in upper side, centered horizontally.
        UpperCenter = 1,
        //
        // Summary:
        //     Text is anchored in upper right corner.
        UpperRight = 2,
        //
        // Summary:
        //     Text is anchored in left side, centered vertically.
        MiddleLeft = 3,
        //
        // Summary:
        //     Text is centered both horizontally and vertically.
        MiddleCenter = 4,
        //
        // Summary:
        //     Text is anchored in right side, centered vertically.
        MiddleRight = 5,
        //
        // Summary:
        //     Text is anchored in lower left corner.
        LowerLeft = 6,
        //
        // Summary:
        //     Text is anchored in lower side, centered horizontally.
        LowerCenter = 7,
        //
        // Summary:
        //     Text is anchored in lower right corner.
        LowerRight = 8
    }
#if GODOT
    public static class AnchorExtensions
    {
        public static Tuple<HorizontalAlignment, VerticalAlignment> GDAlignmentFromAnchor(Anchor anchor)
        {
            switch (anchor)
            {
                case Anchor.UpperLeft:
                    return Tuple.Create(HorizontalAlignment.Left, VerticalAlignment.Top);
                case Anchor.UpperCenter:
                    return Tuple.Create(HorizontalAlignment.Center, VerticalAlignment.Top);
                case Anchor.UpperRight:
                    return Tuple.Create(HorizontalAlignment.Right, VerticalAlignment.Top);
                case Anchor.MiddleLeft:
                    return Tuple.Create(HorizontalAlignment.Left, VerticalAlignment.Center);
                case Anchor.MiddleCenter:
                    return Tuple.Create(HorizontalAlignment.Center, VerticalAlignment.Center);
                case Anchor.MiddleRight:
                    return Tuple.Create(HorizontalAlignment.Right, VerticalAlignment.Center);
                case Anchor.LowerLeft:
                    return Tuple.Create(HorizontalAlignment.Left, VerticalAlignment.Bottom);
                case Anchor.LowerCenter:
                    return Tuple.Create(HorizontalAlignment.Center, VerticalAlignment.Bottom);
                case Anchor.LowerRight:
                    return Tuple.Create(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            }
            return Tuple.Create(HorizontalAlignment.Left, VerticalAlignment.Top);
        }

        public static Anchor AnchorFromGDLayoutPreset(Godot.Control.LayoutPreset layoutPreset)
        {
            switch (layoutPreset)
            {
                case Godot.Control.LayoutPreset.TopLeft:
                    return Anchor.UpperLeft;
                case Godot.Control.LayoutPreset.CenterLeft:
                    return Anchor.MiddleLeft;
                case Godot.Control.LayoutPreset.BottomLeft:
                    return Anchor.LowerLeft;
                case Godot.Control.LayoutPreset.CenterTop:
                    return Anchor.UpperCenter;
                case Godot.Control.LayoutPreset.Center:
                    return Anchor.MiddleCenter;
                case Godot.Control.LayoutPreset.CenterBottom:
                    return Anchor.LowerCenter;
                case Godot.Control.LayoutPreset.TopRight:
                    return Anchor.UpperRight;
                case Godot.Control.LayoutPreset.CenterRight:
                    return Anchor.MiddleRight;
                case Godot.Control.LayoutPreset.BottomRight:
                    return Anchor.LowerRight;
            }
            return Anchor.UpperLeft;
        }

        public static Godot.Control.LayoutPreset GDLayoutPresetFromAnchor(Anchor anchor)
        {
            switch (anchor)
            {
                case Anchor.UpperLeft:
                    return Godot.Control.LayoutPreset.TopLeft;
                case Anchor.MiddleLeft:
                    return Godot.Control.LayoutPreset.CenterLeft;
                case Anchor.LowerLeft:
                    return Godot.Control.LayoutPreset.BottomLeft;
                case Anchor.UpperCenter:
                    return Godot.Control.LayoutPreset.CenterTop;
                case Anchor.MiddleCenter:
                    return Godot.Control.LayoutPreset.Center;
                case Anchor.LowerCenter:
                    return Godot.Control.LayoutPreset.CenterBottom;
                case Anchor.UpperRight:
                    return Godot.Control.LayoutPreset.TopRight;
                case Anchor.MiddleRight:
                    return Godot.Control.LayoutPreset.CenterRight;
                case Anchor.LowerRight:
                    return Godot.Control.LayoutPreset.BottomRight;
            }
            return Godot.Control.LayoutPreset.TopLeft;
        }

        public static Godot.BoxContainer.AlignmentMode GDAlignmentModeFromAnchor(Anchor anchor) {
            switch (anchor)
            {
                case Anchor.UpperLeft:
                case Anchor.MiddleLeft:
                case Anchor.LowerLeft:
                    return Godot.BoxContainer.AlignmentMode.Begin;
                case Anchor.UpperCenter:
                case Anchor.MiddleCenter:
                case Anchor.LowerCenter:
                    return Godot.BoxContainer.AlignmentMode.Center;
                case Anchor.UpperRight:
                case Anchor.MiddleRight:
                case Anchor.LowerRight:
                    return Godot.BoxContainer.AlignmentMode.End;
            }
            return Godot.BoxContainer.AlignmentMode.Begin;
        }
        public static Anchor AnchorFromGDAlignmentMode(Godot.BoxContainer.AlignmentMode alignmentMode)
        {
            switch (alignmentMode)
            {
                case Godot.BoxContainer.AlignmentMode.Begin:
                    return Anchor.UpperLeft;
                case Godot.BoxContainer.AlignmentMode.Center:
                    return Anchor.MiddleCenter;
                case Godot.BoxContainer.AlignmentMode.End:
                    return Anchor.LowerRight;
            }
            return Anchor.UpperLeft;
        }
        public static Anchor AnchorFromGDAlignment(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            if (verticalAlignment.Equals(VerticalAlignment.Top))
            {
                if (horizontalAlignment.Equals(HorizontalAlignment.Left))
                {
                    return Anchor.UpperLeft;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Center))
                {
                    return Anchor.UpperCenter;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Right))
                {
                    return Anchor.UpperRight;
                }
            }
            else if (verticalAlignment.Equals(VerticalAlignment.Center))
            {
                if (horizontalAlignment.Equals(HorizontalAlignment.Left))
                {
                    return Anchor.MiddleLeft;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Center))
                {
                    return Anchor.MiddleCenter;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Right))
                {
                    return Anchor.MiddleRight;
                }
            }
            else if (verticalAlignment.Equals(VerticalAlignment.Bottom))
            {
                if (horizontalAlignment.Equals(HorizontalAlignment.Left))
                {
                    return Anchor.LowerLeft;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Center))
                {
                    return Anchor.LowerCenter;
                }
                else if (horizontalAlignment.Equals(HorizontalAlignment.Right))
                {
                    return Anchor.LowerRight;
                }
            }
            return Anchor.UpperLeft;
        }
    }
#endif
}
