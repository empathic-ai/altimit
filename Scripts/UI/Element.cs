using System;
using System.Linq;
#if UNITY
using UnityEngine;
using UnityEngine.EventSystems;

namespace Altimit.UI.Unity {

public class Element : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler {

    public delegate void OnPointerEvent(PointerEventData eventData);
    // public delegate void ControllerInteractionEvent(ControllerInteractionEventArgs e);
    public Action<PointerEventData> onPointerEnter;
    public Action<PointerEventData> onEndDrag;
    public Action<PointerEventData> onDrag;
    public OnPointerEvent onPointerDown;
    public OnPointerEvent onPointerUp;
    public Action<PointerEventData> onPointerExit;
    public Action onPointerClick;

    public OnPointerEvent onGripStart;
    public OnPointerEvent onGrip;
    public OnPointerEvent onGripEnd;

    public OnPointerEvent onBeginGrab;
    public OnPointerEvent onGrab;
    public OnPointerEvent onEndGrab;

    public bool IncludeChildren = true;
    public bool IsPointerOver = false;
    [NonSerialized]
    public bool IsGripping = false;
    Rect oldRect;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    RectTransform _rectTransform;

    public bool IsGrabbing = false;

    public new void OnEnable ()
    {
        base.OnEnable();
    }

    protected override void Awake()
    {
        if (!Application.isPlaying)
            return;

        // oldRect = rectTransform.rect;
        base.Awake();
    }

    public new void Start ()
    {
        base.Start();
    }
    //eventually fill out with all necessary events inherited
    public void RegisterElement (Element element)
    {
        element.onGrab += OnGrab;
        element.onDrag += OnDrag;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        IsGrabbing = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
            onDrag(eventData);

        var parent = transform.parent;
        if (parent != null)
        {
            var element = parent.GetComponent<Element>();
            if (element != null)
            {
                element.OnDrag(eventData);
            }
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
            onEndDrag(eventData);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onPointerClick?.Invoke();
    }

    public virtual void OnBeginGrab (PointerEventData eventData)
    {
        IsGrabbing = true;
        if (onBeginGrab != null)
            onBeginGrab(eventData);
    }

    public virtual void OnGrab(PointerEventData eventData)
    {
        if (onGrab != null)
            onGrab(eventData);
    }

    public virtual void OnEndGrab(PointerEventData eventData)
    {
        IsGrabbing = false;
        if (onEndGrab != null)
            onEndGrab(eventData);
    }

    public virtual void OnPointerEnter (PointerEventData eventData)
    {
        IsPointerOver = true;
        if (onPointerEnter != null)
            onPointerEnter(eventData);
    }

    public virtual void OnPointerExit (PointerEventData eventData)
    {
        IsPointerOver = false;
        if (onPointerExit != null)
            onPointerExit(eventData);
    }

    public void OnGripStart(PointerEventData e)
    {
        IsGripping = true;
        if (onGripStart != null)
            onGripStart(e);
    }
    public void OnGripEnd(PointerEventData e)
    {
        IsGripping = false;
        if (onGripEnd != null)
            onGripEnd(e);
    }

    public void OnGrip(PointerEventData e)
    {
        if (onGrip != null)
            onGrip(e);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke(eventData);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null)
            onPointerDown(eventData);
    }

    private void Update()
    {
        /*
        if (rectTransform.rect.size != oldRect.size && OnRectTransformDimensionsChanged != null)
        {
         //   Debug.Log(t.rect.size + ", " + oldRect.size);
            OnRectTransformDimensionsChanged(this, new EventArgs());
        }
        oldRect = rectTransform.rect;
        */
    }

    public Action onDimensionsChange;

    protected override void OnRectTransformDimensionsChange()
    {
        if (onDimensionsChange != null)
        {
            onDimensionsChange?.Invoke();
        }
    }
}

}
#endif