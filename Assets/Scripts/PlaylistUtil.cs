using UnityEngine;

public class PlaylistUtil : MonoBehaviour
{
    public Transform GetNextAvailableSlot()
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
