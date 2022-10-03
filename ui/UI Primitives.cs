using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

#if UNITY_5_3_OR_NEWER
using SoftMasking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = UnityEngine.Material;
using Font = TMPro.TMP_FontAsset;
using Node = UnityEngine.GameObject;
using Random = UnityEngine.Random;
using Coffee.UIEffects;
using DG.DeAudio;
using LeTai.TrueShadow;
using ImageType = UnityEngine.UI.Image.Type;
using InputType = TMPro.TMP_InputField.InputType;
#elif GODOT
using Godot;
using Node = Godot.Node;
using Font = Godot.Font;
using Toggle = Godot.Button;
using Sprite = Godot.Texture;
#endif

namespace Altimit.UI
{
    public partial class AUI
    {
        public static Action<Node> OnToggleCreated = (x) => { };
        public static Action<Node> OnButtonCreated = (x) => { };
        public static Action<Node> OnTextCreated = (x) => { };
        public static Action<Node> OnCanvasCreated = (x) => { };
        public static Action<Node> OnInputCreated = (x) => { };
        public static Action<Node> OnDropdownCreated = (x) => { };

        public static Node ViewManager(this Node node, Node back = null)
        {
            return node.
                Hold<ViewManager>(x => { x.BackButtonGO = back; });
        }

        public static Node ToggleGroup<T>(this Node node) where T : Enum
        {
            node.ToggleGroup().RoundImage(AUI.Purple).HList(0, 0).FitWidth().Shadow();
            foreach (var name in Enum.GetNames(typeof(T)))
            {
                node.Hold(new ToggleButton() { Name = name, Material = AUI.Purple, UseShadow = false });
            }
            return node;    
        }

        public static Node ToggleGroup(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<BetterToggleGroup>();
#elif GODOT
            return null;
#endif
            return node;
        }
        
        public static Node OnToggle(this Node node, Action<ToggleButton> onToggle)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<BetterToggleGroup>(x=>x.onToggle += onToggle);
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Panel(this Node go)
        {
            return go.
                Hold<View>();
        }

        public static Node Panel(this Node node, Action<Node> onRender, params Node[] childPanels)
        {
#if UNITY_5_3_OR_NEWER
            return node.
                Hold<View>(x => { x.Init(onRender, childPanels); });
#elif GODOT
            return null;
#endif
            return node;
        }

        public static Node VList(this Node go, TextAnchor alignment)
        {
            return go.VList(SmallSpace, SmallSpace, alignment);
        }

        public static Node VList(this Node go, int padding = SmallSpace, int spacing = SmallSpace, TextAnchor alignment = TextAnchor.UpperLeft)
        {
#if UNITY_5_3_OR_NEWER
            return go.Hold<VerticalLayoutGroup>(x =>
            {
                x.childAlignment = alignment;
                x.childControlHeight = true;
                x.childControlWidth = true;
                x.childForceExpandHeight = false;
                x.childForceExpandWidth = true;
                x.padding = new RectOffset(padding, padding, padding, padding);
                x.spacing = spacing;
            });
#elif GODOT
            return go;
#endif
            return null;
        }

        public static Node GridList(this Node node, int padding = AUI.SmallSpace, int spacing = AUI.SmallSpace)
        {
#if UNITY_5_3_OR_NEWER
            return go.Hold<GridLayoutGroup>(x=>x.spacing = Vector2.one*spacing).SetPadding(padding).Hold<StretchingGrid>();
#elif GODOT
            return go;
#endif
            return node;
        }

        public static Node HList(this Node go, TextAnchor alignment)
        {
            return go.HList(SmallSpace, SmallSpace, alignment);
        }

        public static Node HList(this Node go, int padding = SmallSpace, int spacing = SmallSpace, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
#if UNITY_5_3_OR_NEWER
            return go.Hold<HorizontalLayoutGroup>(x =>
            {
                x.childAlignment = alignment;
                x.childControlHeight = true;
                x.childControlWidth = true;
                x.childForceExpandHeight = false;
                x.childForceExpandWidth = false;
                x.padding = new RectOffset(padding, padding, padding, padding);
                x.spacing = spacing;
            });
#elif GODOT
            return go;
#endif
            return go;
        }

        public static Node CanvasGroup(this Node node, float alpha = 1)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<CanvasGroup>(x => { x.alpha = alpha; });
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node Canvas(this Node node)
        {
            /*
#if UNITY_5_3_OR_NEWER
            node.Hold<Canvas>(x => {
                x.planeDistance = 1;
                x.additionalShaderChannels = AdditionalCanvasShaderChannels.Normal;
            });
            OnCanvasCreated?.Invoke(node);
            return node;
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node SmallImage(this Node go, Sprite sprite = null, Material material = null, ImageType type = ImageType.Simple)
        {
            return go.Image(sprite, material, type).SetSize(AUI.SmallSize);
        }

        public static Node Image(this Node node, Sprite sprite = null, Material material = null, ImageType type = ImageType.Simple)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<Image>(x =>
            {
                x.sprite = sprite;
                x.type = type;
            }).SetMaterial(material);
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Slider(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            RectTransform fillRect, handleRect;
            return node.Hold(
                AUI.UI.Image(null, AUI.Clear).OnHeld(x => x.Stretch(-AUI.SmallSpace)).Hold(
                    AUI.UI.RoundImage(AUI.DarkGrey).OnHeld(x => x.Stretch(AUI.SmallSpace))
                ),
                AUI.UI.OnHeld(x => x.Stretch()).Hold(
                    AUI.UI.RoundImage(AUI.LightGrey).OnHeld(x => x.Stretch())
                ).Hold(out fillRect),
                AUI.UI.OnHeld(x => x.Stretch()).Hold(
                    AUI.UI.Image(AUI.GetSprite("Circle")).SetSize(30).Shadow()
                ).Hold(out handleRect)
            ).Hold<Slider>(x =>
            {
                x.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
                x.handleRect = handleRect;
                x.fillRect = fillRect;
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Bar(this Node node, Material frontMaterial = null, Material backMaterial = null)
        {
#if UNITY_5_3_OR_NEWER
            return node.RoundImage(AUI.SmallSize, backMaterial).SetHeight(AUI.MiniSize).Hold(
                    AUI.UI.RoundImage(AUI.SmallSize, frontMaterial).Stretch().Hold<RectTransform>(x=>x.pivot = new Vector2(0,.5f))
                );
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Search(this Node node, Func<string, CancellationToken, Task> onSearchRequest = null)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Search>(x=>x.Init(onSearchRequest)).RoundImage(AUI.Default).Shadow().MinWidth(200).HList().SetHeight(AUI.SmallSize).Hold(
                    UI.Input("Search", InputType.Standard, 0, true, false).SetSprite(null),
                    UI.Image(GetSprite("Search"), AUI.MediumGrey).SetSize(TinySize)
                );
#elif GODOT
            return null;
#endif
            */
            return null;
        }

        public static Node Input(this Node go, string placeholder, bool isSingleLine)
        {
            return Input(go, placeholder, InputType.Standard, AUI.SmallSpace, isSingleLine);
        }

        public static Node ScrollView(this Node go, params Node[] children)
        {
            /*
            return go.ScrollView(
                AUI.UI.VList().Hold(children)
            );
            */
            return go;
        }
        
        public static Node ScrollView(this Node node, Node contentNode, int spacing = 0, int padding = 0, bool useBar = true)
        {
#if UNITY_5_3_OR_NEWER
            ScrollRect scrollRect;
            Scrollbar scrollbar;
            Node handleNode;
            Node marginNode;
            Node viewportNode;

            int scrollBarWidth = 20;
            int halfScrollBarWidth = scrollBarWidth / 2;

            //previous scroll sensitivity: 140?
            node.Hold<ScrollRect>(x => { x.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide; x.horizontal = false; x.movementType = ScrollRect.MovementType.Clamped; x.scrollSensitivity = 20; }, out scrollRect);
            node.RoundImage().Mask(false).HList(0,0).ExpandWidth(false).ExpandHeight(true).Hold(
                viewportNode = AUI.UI.FlexibleWidth(true, false).Image(AUI.GetSprite("Solid")).Hold<Mask>(x=>x.showMaskGraphic = false).Hold(
                    marginNode = AUI.UI.VList(spacing, padding).FitHeight().SetPivot(new Vector2(.5f, 1)).OnHeld(x => x.SetAnchor(TextAnchor.UpperCenter, StretchType.Horizontal).SetMargin(0)).Hold(
                        contentNode.SetPivot(new Vector2(.5f, 1))
                    )
                )
            );
            if (useBar)
            {
                node.Hold(
                    AUI.UI.Hold<Scrollbar>(x => { x.direction = Scrollbar.Direction.BottomToTop; x.transition = Selectable.Transition.None; }, out scrollbar).Hold(
                        AUI.UI.RoundImage(AUI.Default).Shadow().Hold(
                            handleNode = AUI.UI.OnHeld(x => x.Stretch()).RoundImage(AUI.Purple).Shadow()
                        )
                    )
                );
                scrollbar.gameObject.StretchVerticalRight().SetPivot(new Vector2(1, 0)).SetPositionX(0).SetWidth(scrollBarWidth+5);
                scrollbar.targetGraphic = handleNode.Get<Image>();
                scrollbar.handleRect = handleNode.Get<RectTransform>();
                scrollbar.transform.GetChild(0).gameObject.SetAnchor(Vector3.zero, Vector3.one).SetMargin(new Vector2(0, halfScrollBarWidth), new Vector2(-halfScrollBarWidth, -halfScrollBarWidth));
                scrollbar.gameObject.SetActive(true);
                scrollRect.content = marginNode.Get<RectTransform>();
                scrollRect.viewport = viewportNode.Get<RectTransform>();
                scrollRect.verticalScrollbar = scrollbar;
            }
            
            viewportNode.SetMargin(Vector2.zero, new Vector2(-scrollBarWidth, 0));

            node.Hold<ScrollRectHelper>();
#elif GODOT
#endif
            return node;
        }

        public static float TextWidthApproximation(string text, Font fontAsset, int fontSize, FontStyles style)
        {
            float width = 0;
#if UNITY_5_3_OR_NEWER
            // Compute scale of the target point size relative to the sampling point size of the font asset.
            float pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
            pointSizeScale *= .9f;
            float emScale = 1 * 0.01f;

            float styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
            float normalSpacingAdjustment = fontAsset.normalSpacingOffset;

            for (int i = 0; i < text.Length; i++)
            {
                char unicode = text[i];
                TMP_Character character;
                // Make sure the given unicode exists in the font asset.
                if (fontAsset.characterLookupTable.TryGetValue(unicode, out character))
                    width += character.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
            }
#elif GODOT
#endif
            return width;
        }

        public static Node Input(this Node node, string placeholder = null, InputType inputType = InputType.Standard, int padding = SmallSpace, bool isSingleLine = true, bool useImage = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
           Node placeholderGO, textGO, viewportGO;

            var textAlignment = isSingleLine ? TextAnchor.MiddleLeft : TextAnchor.UpperLeft;
            node.Hold<TMP_InputField>(x => {
                x.onFocusSelectAll = false;
                x.transition = Selectable.Transition.None;
                x.caretWidth = 3;
                x.customCaretColor = true;
                x.caretColor = AUI.MediumGrey.color;
                x.ForceLabelUpdate();
                x.inputType = inputType;
                x.lineType = (isSingleLine ? TMP_InputField.LineType.SingleLine : TMP_InputField.LineType.MultiLineNewline);
            }).
                Hold<InputFix>().//Image(AUI.circl,AUI.Default,UnityEngine.UI.Image.Type.Sliced).
                SetHeight(AUI.TinySize + (padding * 2)).
                FlexibleWidth().
                Hold(
                    //AUI.UI.RoundImage().IgnoreLayout().SetSprite(AUI.GetSprite("Circle Outline")).SetMaterial(useBorder ? AUI.Grey : AUI.None).OnHeld(x => x.Stretch()),
                    viewportGO = UI.Hold<RectMask2D>().IgnoreLayout().OnHeld(x=>x.SetMargin(padding)).Hold(
                        placeholderGO = UI.Text(placeholder, Colored, textAlignment, true).Hold<TextMeshProUGUI>(x => x.margin = new Vector4(0, 0, 0, 0)).OnHeld(x => x.Stretch()),
                        textGO = UI.Text(null, AUI.DarkGrey, textAlignment).Hold<TextMeshProUGUI>(x=>x.margin = new Vector4(0,0,0,0)).OnHeld(x => x.Stretch())
                    )
                );
            if (useImage)
                node.RoundImage().Shadow();
            node.Get<TMP_InputField>().textViewport = viewportGO.Get<RectTransform>();
            node.Get<TMP_InputField>().textComponent = textGO.Get<TextMeshProUGUI>();
            node.Get<TMP_InputField>().placeholder = placeholderGO.Get<TextMeshProUGUI>();
            node.SetActive(false);
            node.SetActive(true);
            OnInputCreated(node);
#elif GODOT
#endif
            */
            return node;
        }

        public static Node Button(this Node go, string text, bool useShadow)
        {
            return go.Button(text, null, null, null, useShadow);
        }

        public static Node Button(this Node node, string text = null, Material material = null, Material textMaterial = null, Sprite buttonSprite = null, bool useShadow = true, bool useChildSize = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            if (material == null)
                material = SemiTransparent;
            if (material == Default && textMaterial == null)
                textMaterial = AUI.DarkGrey;

            Image image;
            if (useChildSize)
                node.HList(0, 0, TextAnchor.MiddleCenter).ExpandWidth(true);
            Node textGO;
            node.Button().HList().SetPadding(new Vector4(AUI.SmallSpace*2, AUI.SmallSpace*2, AUI.SmallSpace, AUI.SmallSpace)).SetHeight(SmallSize).MinWidth(SmallSize).RoundImage(-1, material, buttonSprite).MinWidth(SmallSize).OnHeld(x=>x.Stretch()).SetHeight(SmallSize).Hold(out image).Hold(
                textGO = UI.Text(text, textMaterial, TextAnchor.MiddleCenter)
            );
            if (useShadow)
                node.Shadow();

            node.Hold<Button>(x => x.image = image);
            return node;
#elif GODOT

            return node;
#endif
            */
            return node;
        }

        public static Node ImageButton(this Node go, Sprite sprite = null, Material material = null)
        {
            return go.ImageButton(null, sprite, material);
        }

        public static Node ImageButton(this Node node, string text = null, Sprite sprite = null, Material material = null)
        {
            /*
#if UNITY_5_3_OR_NEWER
            if (material == null)
                material = Purple;

            Image image;
            node.RoundImage(-1, material, AUI.GetSprite("Circle")).Hold(out image).Shadow().Scale().HList(0, 0, TextAnchor.MiddleCenter);
            // If this icon button includes a text label, render it. Otherwise, just render the icon as part of the contents
            if (text != null)
            {
                node.SetPadding(new Vector4(AUI.SmallSpace * 2, AUI.SmallSpace * 2, AUI.SmallSpace, AUI.SmallSpace));
                node.SetSpacing(AUI.SmallSpace);
                node.Hold(
                    UI.Image(sprite).SetSize(TinySize),
                    UI.Text(text, AUI.Default, TextAnchor.MiddleCenter)
                );
            } else
            {
                node.ExpandWidth(false);
                node.SetSize(SmallSize).ExpandWidth(false).Hold(
                     UI.Image(sprite).SetSize(TinySize)
                );
            }
            if (!node.GetComponent<Selectable>())
                node.Button().Hold<Button>(x => x.image = image);
            return node;
#elif GODOT

            return node;
#endif
            */
            return node;
        }

        public static Node ToggleButton(this Node go, string text, Material material, bool useShadow)
        {
            return go.ToggleButton(text, material, null, useShadow);
        }

        public static Node ToggleButton(this Node node, string text = null, Material material = null, Material textMaterial = null, bool useShadow = true)
        {
            /*
#if UNITY_5_3_OR_NEWER
            if (material == null)
                material = Default;
            if (material == Default && textMaterial == null)
                textMaterial = AUI.DarkGrey;

            Image image;

            node.Toggle().SetHeight(SmallSize).RoundImage(-1, material, AUI.GetSprite("Circle")).OnHeld(x => x.Stretch()).HList(TextAnchor.MiddleCenter).SetHeight(SmallSize).Hold(out image).Hold(
                UI.Text(text, textMaterial, TextAnchor.MiddleCenter)
            );
            if (useShadow)
                node.Shadow();
            node.OnHeld(x => {
                var toggleGroup = node.transform.parent.GetComponent<ToggleGroup>();
                x.Get<Toggle>().group = toggleGroup;
            });
            return node.Hold<Toggle>(x => { x.image = image; }).Hold<SidebarToggleEffect>();
#elif GODOT
            return node;
#endif
            */
            return node;
        }

        public static Node ScaleButton(this Node node, string text = null, Material material = null, bool useChildSize = true)
        {
#if UNITY_5_3_OR_NEWER
            return node.Button(text, material, null, null, true, useChildSize).Hold<ScaleButton>();
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Scale(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            return node.Hold<ScaleButton>();
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Dropdown(this Node node, string[] options)
        {
            /*
#if UNITY_5_3_OR_NEWER
            Node scrollViewGO;
            Node arrowGO;
            RectTransform template;
            MDropdown dropdown;
            TMP_Text captionText;
            TMP_Text itemText;
            var longestOption = options.OrderByDescending(s => s.Length).First();

            node.HList(0, 0, TextAnchor.UpperCenter).ChildControl(true,false).Hold(
                AUI.UI.VList(0, 0).Hold(
                    AUI.UI.HList().RoundImage().Shadow().Hold(
                        AUI.UI.Text(AUI.DarkGrey).Hold(true, out captionText).MinWidth(TextWidthApproximation(longestOption, Font, 36, FontStyles.Normal)),
                        arrowGO = AUI.UI.Image(AUI.GetSprite("Down"), AUI.DarkGrey).SetSize(AUI.TinySize)
                    ),
                    AUI.UI.SetHeight(10),
                    scrollViewGO = AUI.UI.ScrollView(
                        AUI.UI.Toggle().Image(material: null).HList(alignment: TextAnchor.MiddleLeft).Hold(
                            AUI.UI.Text(AUI.DarkGrey, TextAnchor.MiddleLeft).Hold<TMPro.TextMeshProUGUI>(x => x.enableWordWrapping = false).Hold(true, out itemText)
                        )
                    , 0, 0).Mask(true).Shadow().Hold(out template).OnHeld(x => x.Stretch().SetMargin(Vector2.zero, new Vector2(30, 0))).HoldFirst(
                        AUI.UI.RoundImage().IgnoreLayout().OnHeld(x => x.Stretch())
                    )
                ).Hold<MDropdown>(x => { x.alphaFadeSpeed = 0; x.template = template; x.itemText = itemText; x.captionText = captionText; x.AddOptions(options.ToList()); }, out dropdown)
                .FitSize(false, true).OnHeld(x => x.SetPivot(new Vector2(.5f, 1)).SetPositionY(0))
            ).SetHeight(AUI.SmallSize);
            var shownOptionsCount = Mathf.Clamp(options.Length, 1, 3);
            scrollViewGO.SetPosition(new Vector2(0, 0)).SetMargin(0, StretchType.Horizontal).SetHeight(shownOptionsCount * 72).SetActive(false);
            
            // Only show scrollbar if needed
            scrollViewGO.Get<ScrollRect>().verticalScrollbar.gameObject.SetActive(options.Length > shownOptionsCount);
            OnDropdownCreated?.Invoke(dropdown.gameObject);

            dropdown.gameObject.Hold<MDropdown>(x => x.ArrowGO = arrowGO);
            return node;
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node BackButton(this Node node)
        {
            /*
#if UNITY_5_3_OR_NEWER
            Node arrowGO;
            BackButton backButton;
            node.Button().Hold<Button>(x=>x.transition = Selectable.Transition.None).Image(AUI.GetSprite("Solid")).SetHeight(SmallSize).ExpandWidth(true).Shadow().Hold(
                arrowGO = AUI.UI.Image(GetSprite("Back"), AUI.DarkGrey).SetSize(TinySize)
            ).Hold<BackButton>(out backButton).SetSortingOrder(100);
            backButton.ArrowGO = arrowGO;
            return node;
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node Shadow(this Node node)
        {
#if UNITY_5_3_OR_NEWER
            /*
            if (!go.GetComponent<UIShadow>())
            {
                for (int i = 1; i < 6; i += 1)
                {
                    var shadow = go.AddComponent<UIShadow>();
                    try {
                        shadow.style = ShadowStyle.Outline8;
                        shadow.effectDistance = Vector2.one * i;
                        shadow.effectColor = new Color(0, 0, 0, 3 / 255.0f);
                    } catch (Exception e)
                    {
                        throw new Exception("Failed to add a shadow component! Add an image to this Node first.", e);
                    }
                }
            }
            */
            return node.Hold<TrueShadow>(x=> {
                x.Size = 15;
                x.Spread = 0;
                x.OffsetDistance = 0;
                x.OffsetAngle = 0;
                x.Color = new Color(0, 0, 0, .33f);
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node MaskButton(this Node node, Sprite sprite = null)
        {
            return node.Scale().Button().RoundImage(AUI.SmallSize).Shadow().SoftMask(false).Hold(
                        new TextureRect() { Sprite = sprite, Material = AUI.Masked }.OnHeld(x=>x.Stretch())
                    );
        }

        public static Node Toggle(this Node node)
        {
            if (node.Has<ToggleButton>())
                return node;
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Toggle>(x=> { x.transition = Selectable.Transition.None; x.toggleTransition = UnityEngine.UI.Toggle.ToggleTransition.None; }).
                OnClick(PlayClick).Call(OnToggleCreated);
#elif GODOT
            */
            return node;
        }

        public static void PlayClick()
        {
#if UNITY_5_3_OR_NEWER
            var audioSource = DeAudioManager.Play(GetSound("Click"), .25f, Random.Range(.9f, 1.2f));
            audioSource.audioSource.bypassEffects = true;
#elif GODOT
#endif
        }

        public static Node Button(this Node node)
        {
            /*
#if UNITY_5_3_OR_NEWER
            return node.Hold<Button>(x=>x.transition = Selectable.Transition.None).OnClick(PlayClick).Call(OnButtonCreated);
#elif GODOT
            */
            return node;
//#endif
        }

        public static Node Text(this Node go, Material material, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            return go.Text(null, material, alignment, false);
        }

        public static Node Text(this Node go, TextAnchor alignment, Material material = null)
        {
            return go.Text(null, material, alignment, false);
        }

        public static Node Text(this Node node
            , string text, bool includeChildren, Material material = null)
        {
            if (material == null)
                material = Default;

#if UNITY_5_3_OR_NEWER
            return node.Hold<TextMeshProUGUI>(includeChildren, x =>
            {
                x.text = text;
                x.color = material.color;
            });
#elif GODOT
            return node;
#endif
            return node;
        }

        public static Node Text(this Node node, string text = null, Material material = null, TextAnchor textAnchor = TextAnchor.UpperLeft, bool isClear = false, bool includeChildren = false)
        {
            if (material == null)
                material = Default;

#if UNITY_5_3_OR_NEWER
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;
            switch(textAnchor)
            {
                case TextAnchor.UpperLeft:
                    alignment = TextAlignmentOptions.TopLeft;
                    break;
                case TextAnchor.MiddleLeft:
                    alignment = TextAlignmentOptions.MidlineLeft;
                    break;
                case TextAnchor.LowerLeft:
                    alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case TextAnchor.UpperCenter:
                    alignment = TextAlignmentOptions.Top;
                    break;
                case TextAnchor.MiddleCenter:
                    alignment = TextAlignmentOptions.Midline;
                    break;
                case TextAnchor.LowerCenter:
                    alignment = TextAlignmentOptions.BottomLeft;
                    break;
                case TextAnchor.UpperRight:
                    alignment = TextAlignmentOptions.TopRight;
                    break;
                case TextAnchor.MiddleRight:
                    alignment = TextAlignmentOptions.MidlineRight;
                    break;
                case TextAnchor.LowerRight:
                    alignment = TextAlignmentOptions.BottomRight;
                    break;
                default:
                    break;
            }

            return node.Hold<TextMeshProUGUI>(includeChildren, x =>
            {
                x.extraPadding = true;
                x.alignment = alignment;
                x.margin = new Vector4(0, -5, 0, -5);
                x.text = text;
                x.color = material.color.SetAlpha(isClear ? .3f : 1);
                x.font = Font;
            }).Call(OnTextCreated);
#elif GODOT
            return node;
#endif
            return node;
        }
    }
}