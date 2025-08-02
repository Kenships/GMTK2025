using System.Collections.Generic;
using DefaultNamespace;
using TrackScripts;
using UnityEngine;

[CreateAssetMenu(fileName = "InventorySO", menuName = "Scriptable Objects/InventorySO")]
public class InventorySO : ScriptableObject
{
    public List<TrackSO> tracks;
    public List<ItemSO> items;
}
