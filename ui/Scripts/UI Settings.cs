#if UNITY_5_3_OR_NEWER
using TMPro;
using UnityEngine;
using Material = UnityEngine.Material;
using Font = TMPro.TMP_FontAsset;
using Texture = UnityEngine.Sprite;
#elif GODOT
using Godot;
#endif

namespace Altimit.Unity.UI
{
    public partial class AUI
    {
        public static Material Green
        {
            get
            {
                return GetMaterial("Green");
            }
        }

        public static Material Gold
        {
            get
            {
                return GetMaterial("Gold");
            }
        }

        public static Material Purple
        {
            get
            {
                return GetMaterial("Purple");
            }
        }
        public static Material Yellow
        {
            get
            {
                return GetMaterial("Yellow");
            }
        }
        public static Material Orange
        {
            get
            {
                return GetMaterial("Orange");
            }
        }

        public static Material Colored
        {
            get
            {
                return GetMaterial("Colored");
            }
        }
        public static Material LightGrey
        {
            get
            {
                return GetMaterial("Light Grey");
            }
        }
        public static Material MediumGrey
        {
            get
            {
                return GetMaterial("Medium Grey");
            }
        }
        public static Material DarkGrey
        {
            get
            {
                return GetMaterial("Dark Grey");
            }
        }
        public static Material Dark
        {
            get
            {
                return GetMaterial("Dark");
            }
        }
        public static Material None
        {
            get
            {
                return GetMaterial("None");
            }
        }
        public static Material Red
        {
            get
            {
                return GetMaterial("Red");
            }
        }
        public static Material Blue
        {
            get
            {
                return GetMaterial("Blue");
            }
        }
        public static Material Default
        {
            get
            {
                return GetMaterial("Default");
            }
        }
        public static Material TwoSided
        {
            get
            {
                return GetMaterial("TwoSided");
            }
        }
        public static Material Masked
        {
            get
            {
                //return Default;
                //Removed due to issues rendeirng SoftMask
                return GetMaterial("Mask");
            }
        }
        public static float Light
        {
            get
            {
                return .66f;
            }
        }
        
        public static Material SemiTransparent
        {
            get
            {
                return GetMaterial("SemiTransparent");
            }
        }
        public static Material Clear
        {
            get {
                return GetMaterial("Clear");
            }
        }
        public static Material DimLight
        {
            get
            {
                return GetMaterial("Dim Light");
            }
        }

        public static Texture InputFieldBackground
        {
            get
            {
                return GetSprite("InputFieldBackground");
            }
        }

        public static Font Font = GetFont("Roboto/Roboto-Medium SDF");

        public const int MiniSize = 10;
        public const int TinySize = 40;
        public const int SmallSize = 70;
        public const int MediumSize = 100;
        public const int LargeSize = 130;
        public const int SmallSpace = 15;
        public const int TinySpace = 5;
    }
}