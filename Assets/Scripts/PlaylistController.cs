using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Obvious.Soap;
using TrackScripts;
using UnityEngine;

public class PlaylistController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    
    [SerializeField]
    private GameObject trackHolderPrefab;

    [SerializeField]
    private GameObject slotPrefab;

    [SerializeField]
    private int capacityMax;

    [SerializeField]
    private GameObjectVariable HeldTrack;
    
    public int Capacity => capacityMax;
    
    public int TrackCount
    {
        get
        {
            if (!HeldTrack)
                return GetAllTracks().Count;
            
            return HeldTrack.Value ? GetAllTracks().Count + 1 : GetAllTracks().Count;
        }
    }

    private RectTransform m_rectTransform;
    private List<Transform> children;
    
    private void Awake()
    {
        children = new List<Transform>();
        m_rectTransform = GetComponent<RectTransform>();
        
        int currentCapacity = transform.childCount; 
        
        int capacityToFill = capacityMax - currentCapacity;

        for (int i = 0; i < capacityToFill; i++)
        {
            Instantiate(slotPrefab, transform);
        }

        foreach (Transform child in transform)
        {
            children.Add(child);
            if (child.childCount > 0)
            {
                Transform grandChild = child.GetChild(0);
                
                if (grandChild.TryGetComponent(out DragAndDrop dragAndDrop))
                {
                    dragAndDrop.Canvas = canvas;
                }
        
                SnapToParentObject snapToParentObject = grandChild.GetComponent<SnapToParentObject>();
                snapToParentObject.Initialize(canvas, this);
            }
            
        }
        
    }

    public bool TryDequeue(out TrackSO trackBase)
    {
        TrackHolder trackHolder = GetNextTrackHolderInQueue();

        if (!trackHolder)
        {
            trackBase = null;
            return false;
        }

        if (HeldTrack && trackHolder.gameObject == HeldTrack.Value)
        {
            HeldTrack.Value = null;
        }
        
        trackBase = trackHolder.Track;
        
        trackHolder.transform.SetParent(null);
        
        Destroy(trackHolder.gameObject);

        foreach (Transform child in children)
        {
            if (child.childCount > 0)
            {
                Transform grandChild = child.GetChild(0);
                SnapToParentObject snapToParentObject = grandChild.GetComponent<SnapToParentObject>();
                snapToParentObject.Initialize(canvas, this);
                snapToParentObject.SnapToParent();
            }
        }
        
        return trackBase;
    }

    public bool TryEnqueue(TrackSO track)
    {
        Transform nextSlot = GetNextEmptySlot();
            
        if (!nextSlot) return false;
        
        GameObject prefab = Instantiate(trackHolderPrefab, children[^1].position, Quaternion.identity);
        
        TrackHolder trackHolder = prefab.GetComponent<TrackHolder>();
        
        trackHolder.Track = track;

        if (prefab.TryGetComponent(out DragAndDrop dragAndDrop))
        {
            dragAndDrop.Canvas = canvas;
        }
        
        SnapToParentObject snapToParentObject = prefab.GetComponent<SnapToParentObject>();
        snapToParentObject.Initialize(canvas, this);
        snapToParentObject.SnapToParent();
        trackHolder.transform.localScale = Vector3.one;

        return true;
    }
    
    public TrackSO Get(int index)
    {
        throw new NotImplementedException();
    }
    
    public TrackSO GetNextInQueue()
    {
        foreach (Transform child in children)
        {
            if (child.childCount == 1)
            {
                TrackHolder trackHolder = child.GetChild(0).GetComponent<TrackHolder>();
                
                return trackHolder.Track;
            }
        }
        
        return null;
    }

    public TrackHolder GetNextTrackHolderInQueue()
    {
        foreach (Transform child in children)
        {
            if (child.childCount == 1)
            {
                TrackHolder trackHolder = child.GetChild(0).GetComponent<TrackHolder>();

                return trackHolder;
            }
        }
        
        Debug.Log(HeldTrack);
        
        if (HeldTrack && HeldTrack.Value)
        {
            Debug.Log(HeldTrack.Value.name);
            
            TrackHolder trackHolder = HeldTrack.Value.GetComponent<TrackHolder>();
            return trackHolder;
        }

        return null;
    }

    public void RemoveAll()
    {
        if (HeldTrack && HeldTrack.Value)
        {
            Destroy(HeldTrack.Value);
            HeldTrack.Value = null;
        }

        foreach (Transform child in children)
        {
            if (child.childCount > 0)
            {
                Transform grandChild = child.GetChild(0);
                grandChild.SetParent(null);
                Destroy(grandChild.gameObject);
            }
        }
    }

    public TrackSO Remove(int index)
    {
        
        return null;
    }
    
    public TrackSO Remove(TrackSO removeObject){
        
        return null;
    }
    
    public Transform GetNextEmptySlot()
    {
        return children.FirstOrDefault(child => child.childCount == 0);
    }

    public List<TrackSO> GetAllTracks()
    {
        List<TrackSO> tracks = new List<TrackSO>();
        
        foreach (Transform child in children)
        {
            if (child.childCount > 0)
            {
                tracks.Add(child.GetChild(0).GetComponent<TrackHolder>().Track);
            }
        }
        
        return tracks;
    }

    public Transform GetLastChild()
    {
        return children[^1];
    }
}
