using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TrackScripts;
using DefaultNamespace;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Message;
    public TrackSO track;
    public ItemSO item;
    public bool shopTooltip;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (track)
        {
            TooltipManager.instance.DisplayTrackTooltip(track, shopTooltip);
        }
        else if (Message != null)
        {
            TooltipManager.instance.DisplayModifierTooltip(Message);
        }
        else if(item)
        {
            TooltipManager.instance.DisplayItemTooltip(item);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.instance.HideTooltip();
    }
}