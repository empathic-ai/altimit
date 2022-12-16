using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Altimit.UI;

#if UNITY
using Altimit.UI.Unity;
using Altimit;
using TMPro;
using UnityEngine.UI;

namespace Altimit.UI
{
    [AType]
    public class Input : Control
    {
        [AProperty]
        public string Placeholder
        {
            get
            {
                return placeholderText.text;
            }
            set
            {
                placeholderText.text = value;
            }
        }

        [AProperty]
        public string Text
        {
            get
            {
                return tmpInputField.text;
            }
            set
            {
                tmpInputField.text = value;
            }
        }

        [AProperty]
        public InputType InputType
        {
            get
            {
                return (InputType)tmpInputField.inputType;
            }
            set
            {
                tmpInputField.inputType = (TMP_InputField.InputType)value;
            }
        }

        public Action<string> OnValueChanged;

        protected TMPro.TMP_InputField tmpInputField { get; private set; }
        protected TMPro.TextMeshProUGUI placeholderText { get; private set; }

        public Input() : base()
        {
            bool isSingleLine = true;
            string placeholder = "";

            GameObject placeholderGO, textGO, viewportGO;
            var inputType = InputType.Standard;

            var textAlignment = isSingleLine ? Anchor.MiddleLeft : Anchor.UpperLeft;

            GameObject.Hold<TMP_InputField>(x =>
            {
                tmpInputField = x;

                x.onFocusSelectAll = false;
                x.transition = Selectable.Transition.None;
                x.caretWidth = 3;
                x.customCaretColor = true;
                x.caretColor = AUI.MediumGrey.UMaterial.color;
                x.ForceLabelUpdate();
                x.inputType = (TMPro.TMP_InputField.InputType)inputType;
                x.lineType = (isSingleLine ? TMP_InputField.LineType.SingleLine : TMP_InputField.LineType.MultiLineNewline);
            });

            GameObject.Hold<InputFix>();

            this.SetHeight(AUI.TinySize + (AUI.SmallSpace * 2)).
                FlexibleWidth();

            GameObject.AddChildren(
                    //AUI.UI.RoundImage().IgnoreLayout().SetSprite(AUI.GetSprite("Circle Outline")).SetMaterial(useBorder ? AUI.Grey : AUI.None).OnHeld(x => x.Stretch()),
                    viewportGO = new GameObject().Hold<RectMask2D>().IgnoreLayout().OnHeld(x => x.SetMargin(0)).AddChildren(
                        placeholderGO = new GameObject().Text(placeholder, AUI.Colored.UMaterial, textAlignment, true).Hold<TextMeshProUGUI>(x => x.margin = new UnityEngine.Vector4(0, 0, 0, 0)).OnHeld(x => x.SetMargin(0)),
                        textGO = new GameObject().Text(null, AUI.DarkGrey.UMaterial, textAlignment).Hold<TextMeshProUGUI>(x => x.margin = new UnityEngine.Vector4(0, 0, 0, 0)).OnHeld(x => x.SetMargin(0))
                    )
                );

            placeholderGO.SetPivot(new Vector2(0, .5f));
            textGO.SetPivot(new Vector2(0, .5f));
   
            placeholderText = placeholderGO.GetComponent<TextMeshProUGUI>();

            //if (useImage)
            //    GameObject.RoundImage().Shadow();

            GameObject.Get<TMP_InputField>().textViewport = viewportGO.Get<RectTransform>();
            GameObject.Get<TMP_InputField>().textComponent = textGO.Get<TextMeshProUGUI>();
            GameObject.Get<TMP_InputField>().placeholder = placeholderGO.Get<TextMeshProUGUI>();
            GameObject.SetActive(false);
            GameObject.SetActive(true);
        }
    }
}
#endif