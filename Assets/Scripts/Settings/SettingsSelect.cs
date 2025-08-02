using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject Active;
    
    [SerializeField]
    private List<GameObject> nonActive;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Active.SetActive(true);

        foreach (GameObject go in nonActive)
        {
            go.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
