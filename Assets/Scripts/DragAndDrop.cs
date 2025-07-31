using System;
using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] GameObjectVariable heldObject;
    [SerializeField] Canvas canvas;
    [SerializeField] PlaylistUtil playlistUtil;
    private RectTransform m_RectTransform;
    private Image m_Image;
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
    }

    private void Update()
    {
        if (heldObject.Value && heldObject.Value != gameObject)
        {
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)m_RectTransform.parent, Mouse.current.position.value, Camera.current, out var localPoint))
            {
                Rect r = m_RectTransform.rect;

                if (localPoint.y >= r.yMin && localPoint.y <= r.yMax)
                {
                    SnapToParent();
                }
                
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        heldObject.Value = gameObject;
        transform.SetParent(canvas.transform);
        m_Image.raycastTarget = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        m_RectTransform.anchoredPosition += new Vector2(0, eventData.delta.y/ canvas.scaleFactor);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        heldObject.Value = null;
        m_Image.raycastTarget = true;

        SnapToParent();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    private void SnapToParent()
    {
        Tween.StopAll(m_RectTransform);
        transform.SetParent(playlistUtil.GetNextEmptySlot() ?? canvas.transform);
        
        Tween.LocalPositionY(
            target: m_RectTransform,
            startValue: m_RectTransform.anchoredPosition.y,
            endValue: 0,
            duration: 0.5f,
            ease: Ease.InOutExpo,
            cycles:1
        );
    }
}
