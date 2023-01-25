#if UNITY
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

namespace Altimit.UI.Unity
{
    public class ScrollRectHelper : MonoBehaviour
    {
        const int scrollBarWidth = 20;
        HorizontalLayoutGroup layoutGroup;
        Scrollbar scrollBar;
        CanvasGroup canvasGroup;
        ScrollRect scrollRect;
        RectTransform rectTransform;
        // Start is called before the first frame update
        void Start()
        {

        }

        private void Awake()
        {
            scrollRect = gameObject.GetComponent<ScrollRect>();
            rectTransform = gameObject.GetComponent<RectTransform>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollBar = scrollRect.verticalScrollbar;
            if (scrollBar != null)
            {
                layoutGroup = gameObject.GetComponent<HorizontalLayoutGroup>();
                canvasGroup = scrollBar.gameObject.AddOrGet<CanvasGroup>();
                //GetComponent<ScrollRect>().verticalScrollbar.onValueChanged.AddListener(x => );
                SnapOff();
            }
        }

        void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        void SnapOff()
        {
            sequence?.Kill();
            layoutGroup.padding.right = -(scrollBarWidth+10);
            canvasGroup.alpha = 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            wasContentExtending = IsContentExtending();
        }

        bool IsContentExtending()
        {
            return rectTransform.rect.height < scrollRect.content.rect.height;
        }

        Sequence sequence;
        bool wasContentExtending = false;

        // Update is called once per frame
        void Update()
        {
            if (scrollBar == null)
                return;
            
            var isContextExtenidng = IsContentExtending();
            if (isContextExtenidng != wasContentExtending)
            {
                sequence?.Kill();
                sequence = DOTween.Sequence();
                wasContentExtending = isContextExtenidng;
                Tweener layoutGroupTweener = DOTween.To(() => layoutGroup.padding.right, x => layoutGroup.padding.right = x, isContextExtenidng ? 0 : -(scrollBarWidth+10), .25f).SetSpeedBased();
                layoutGroupTweener.onUpdate += () => LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
                //canvasGroup.alpha = isContextExtenidng ? 1 : 0;
                Tweener canvasGroupTweener = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, isContextExtenidng ? 1 : 0, .01f).SetSpeedBased();
                sequence.SetDelay(.1f);
                if (isContextExtenidng)
                {
                    sequence.Append(canvasGroupTweener);
                    sequence.Append(layoutGroupTweener);
                }
                else
                {
                    sequence.Append(layoutGroupTweener);
                    sequence.Append(canvasGroupTweener);
                }
                sequence.Play();
            }
            
        }
    }
}
#endif