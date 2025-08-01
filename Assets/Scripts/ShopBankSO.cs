using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;


[CreateAssetMenu(fileName = "ShopBankSO", menuName = "Scriptable Objects/ShopBankSO")]
public class ShopBankSO : ScriptableObject
{
    public List<TrackSO> availableTracks;
    public List<ItemSO> availableItems;
}
