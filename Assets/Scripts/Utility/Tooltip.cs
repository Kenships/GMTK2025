using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TrackScripts;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Message;
    public TrackSO track;
    public bool shopTooltip;
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
