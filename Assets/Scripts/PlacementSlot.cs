using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlacementSlot : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField] Transform placementLocation;
    
    private IPlaceable placeable;
    
    private void Awake()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.canceled += MouseClickOncanceled;
    }

    private void MouseClickOncanceled(InputAction.CallbackContext obj)
    {
        if (placeable != null)
        {
            placeable.Place(placementLocation.position);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IPlaceable placeable))
        {
            this.placeable = placeable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        placeable = null;
    }
}
