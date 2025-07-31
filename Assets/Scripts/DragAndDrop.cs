using Obvious.Soap;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        
        transform.SetParent(playlistUtil.GetNextAvailableSlot() ?? canvas.transform);
        
        Tween.LocalPositionY(
            target: m_RectTransform,
            startValue: m_RectTransform.anchoredPosition.y,
            endValue: 0,
            duration: 0.5f,
            ease: Ease.InOutExpo,
            cycles:1
        );
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (heldObject.Value)
        {
            transform.SetParent(playlistUtil.GetNextAvailableSlot() ?? canvas.transform);
        
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

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
