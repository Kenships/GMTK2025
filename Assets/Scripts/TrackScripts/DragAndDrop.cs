using System;
using Obvious.Soap;
using PrimeTween;
using TrackScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] GameObjectVariable heldObject;
    [SerializeField] Canvas canvas;
    [SerializeField] SnapToParentObject snapToParentObject;

    [SerializeField] private BoolVariable lockDragAndDrop;
    private RectTransform m_RectTransform;
    private Image m_Image;
    public Canvas Canvas { get => canvas; set => canvas = value; }
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
        snapToParentObject = GetComponent<SnapToParentObject>();
    }

    private void Update()
    {
        if (lockDragAndDrop.Value) return;
        
        if (heldObject.Value && heldObject.Value != gameObject)
        {
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)m_RectTransform.parent, Mouse.current.position.value, Camera.current, out var localPoint))
            {
                Rect r = m_RectTransform.rect;

                if (localPoint.y >= r.yMin && localPoint.y <= r.yMax)
                {
                    snapToParentObject.SnapToParent();
                }
                
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (lockDragAndDrop.Value) return;
        
        Tween.StopAll(m_RectTransform);
        
        heldObject.Value = gameObject;
        Debug.Log(heldObject.Value.name);
        transform.SetParent(Canvas.transform);
        transform.SetAsLastSibling();
        m_Image.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (lockDragAndDrop.Value) return;
        
        heldObject.Value = gameObject;
        
        m_RectTransform.anchoredPosition += new Vector2(0, eventData.delta.y/ Canvas.scaleFactor);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (lockDragAndDrop.Value) return;
        
        heldObject.Value = null;
        m_Image.raycastTarget = true;

        snapToParentObject.SnapToParent();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
