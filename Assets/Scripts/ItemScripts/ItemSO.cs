using UnityEngine;

namespace DefaultNamespace
{
    
    [CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
    public class ItemSO : ScriptableObject
    {
        public Sprite itemCover;
        public string description;
        public int price;
        public string name;
    }
}
