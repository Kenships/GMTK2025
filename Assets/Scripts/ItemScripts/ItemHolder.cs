using UnityEngine;

namespace DefaultNamespace
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] private ItemSO item;
        
        public ItemSO Item {get => item; set => item = value;}

        private void Start()
        {
            GetComponent<Tooltip>().item = item;
        }

    }
    
}
