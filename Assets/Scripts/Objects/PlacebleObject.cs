using System;
using UnityEngine;
using Zenject;

public class PlacebleObject : MonoBehaviour
{
    public event Action OnPlaceEvent;

    [SerializeField] private BoundsInt area;

    [Inject] private GridSystem gridSystem;

    public bool IsPlaced { get; private set; }

    public bool CanBePlaced()
    {
        BoundsInt areaTemp = area;
        areaTemp.position = gridSystem.GridLayout.LocalToCell(transform.position);
        return gridSystem.CanTakeArea(areaTemp);
    }

    public void Place()
    {
        BoundsInt areaTemp = area;
        areaTemp.position = gridSystem.GridLayout.LocalToCell(transform.position);
        gridSystem.TakeArea(areaTemp);
        IsPlaced = true;
        OnPlaceEvent?.Invoke();
    }

    public bool TryPlace()
    {
        if (CanBePlaced())
        {
            Place();
            return true;
        }

        return false;
    }
}
