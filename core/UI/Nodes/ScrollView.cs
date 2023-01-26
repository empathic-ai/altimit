using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Altimit.UI;

#if UNITY_64
using Altimit.UI.Unity;
using Altimit;
using TMPro;
using UnityEngine.UI;

namespace Altimit.UI
{
    [AType]
    public class ScrollView : Control
    {
        public Control Content { get; set; }

        public ScrollView() : base()
        {
            int spacing = 0;
            int padding = 0;
            bool useBar = true;

            ScrollRect scrollRect;
            Scrollbar scrollbar;
            GameObject handleNode;
            GameObject marginNode;
            GameObject viewportNode;

            int scrollBarWidth = 20;
            int halfScrollBarWidth = scrollBarWidth / 2;

            Content = new Control();

            //previous scroll sensitivity: 140?
            GameObject.Hold<ScrollRect>(x => { x.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide; x.horizontal = false; x.movementType = ScrollRect.MovementType.Clamped; x.scrollSensitivity = 20; }, out scrollRect);
            GameObject.RoundImage().Mask(false).HList(0, 0).ExpandWidth(false).ExpandHeight(true);
            GameObject.AddChildren(
                viewportNode = new GameObject().FlexibleWidth(true, false).Image(AUI.GetSprite("Solid").USprite).Hold<Mask>(x => x.showMaskGraphic = false).AddChildren(
                    marginNode = new GameObject().VList(spacing, padding).FitHeight().SetPivot(new Vector2(.5f, 1)).OnHeld(x => x.SetAnchor(TextAnchor.UpperCenter, StretchType.Horizontal).SetMargin(0)).AddChildren(
                        Content.GameObject.SetPivot(new Vector2(.5f, 1))
                    )
                )
            );
            
            if (useBar)
            {
                GameObject.AddChildren(
                    new GameObject().Hold<Scrollbar>(x => { x.direction = Scrollbar.Direction.BottomToTop; x.transition = Selectable.Transition.None; }, out scrollbar).AddChildren(
                        new GameObject().RoundImage(AUI.Default.UMaterial).Shadow().AddChildren(
                            handleNode = new GameObject().OnHeld(x => x.SetMargin(0)).RoundImage(AUI.Purple.UMaterial).Shadow()
                        )
                    )
                );
                scrollbar.gameObject.StretchVerticalRight().SetPivot(new Vector2(1, 0)).SetPositionX(0).SetWidth(scrollBarWidth + 5);
                scrollbar.targetGraphic = handleNode.Get<UnityEngine.UI.Image>();
                scrollbar.handleRect = handleNode.Get<RectTransform>();
                scrollbar.transform.GetChild(0).gameObject.SetAnchor(UnityEngine.Vector3.zero, UnityEngine.Vector3.one).SetMargin(new UnityEngine.Vector2(0, halfScrollBarWidth), new UnityEngine.Vector2(-halfScrollBarWidth, -halfScrollBarWidth));
                scrollbar.gameObject.SetActive(true);
                scrollRect.content = marginNode.Get<RectTransform>();
                scrollRect.viewport = viewportNode.Get<RectTransform>();
                scrollRect.verticalScrollbar = scrollbar;
            }

            viewportNode.SetMargin(UnityEngine.Vector2.zero, new Vector2(-scrollBarWidth, 0));

            GameObject.Hold<ScrollRectHelper>();
        }
    }
}
#endif