using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Message;
    public Sprite sprite;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (shopTooltip) 
        {
            TooltipManager.instance.DisplayShopTooltip(track);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}
