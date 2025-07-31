using Obvious.Soap;
using UnityEngine;

public class PlaylistUtil : MonoBehaviour
{
    public GameObject GetNextInQueue()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount == 1)
            {
                return child.GetChild(0).gameObject;
            }
        }
        
        return null;
    }

    public GameObject DeQueue()
    {

        return null;
    }

    public GameObject EnQueue()
    {
        
        return null;
    }
    
    public GameObject Get(int index)
    {
        
        return null;
    }

    public GameObject Remove(int index)
    {
        
        return null;
    }
    
    public GameObject Remove(GameObject removeObject){
        
        return null;
    }
    
    public Transform GetNextEmptySlot()
    {
        foreach (Transform child in transform)
        {
            if (child.childCount == 0)
            {
                return child;
            }
        }
        return null;
    }
}
