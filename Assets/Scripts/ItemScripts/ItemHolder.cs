using UnityEngine;

namespace DefaultNamespace
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] private ItemSO item;
        
        public ItemSO Item {get => item; set => item = value;}
        
    }
}
