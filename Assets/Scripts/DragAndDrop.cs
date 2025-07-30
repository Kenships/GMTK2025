using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] Canvas canvas;
    private RectTransform m_RectTransform;
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        m_RectTransform.anchoredPosition += eventData.delta/ canvas.scaleFactor;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
