using System;
using System.Collections.Generic;
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
    
    public int Capacity => capacityMax;
    
    
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

    private TrackHolder GetNextTrackHolderInQueue()
    {
        foreach (Transform child in children)
        {
            if (child.childCount == 1)
            {
                TrackHolder trackHolder = child.GetChild(0).GetComponent<TrackHolder>();

                return trackHolder;
            }
        }

        return null;
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
        foreach (Transform child in children)
        {
            if (child.childCount == 0)
            {
                return child;
            }
        }

        return null;
    }
}
