using R3;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PlaceableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnPlaceEvent;

    [SerializeField] private BoundsInt area;
    [SerializeField] private bool isReplaceable;
    [SerializeField] private float replacementInterval = 1f;

    [Inject] private GridSystem gridSystem;
    [Inject] private PlacementHelper placementHelper;

    private IDisposable disposableTimerToReplace;

    public bool IsPlaced { get; private set; }

    #region IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isReplaceable)
        {
            disposableTimerToReplace = Observable.Timer(TimeSpan.FromSeconds(replacementInterval))
                .Subscribe((_) => placementHelper.ActivateReplacePlaceable(this));
        }
    }
    #endregion

    #region IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isReplaceable)
        {
            disposableTimerToReplace?.Dispose();
        }
    }
    #endregion

    public bool CanBePlaced()
    {
        BoundsInt areaTemp = area;
        areaTemp.position = gridSystem.GridLayout.WorldToCell(transform.position);
        return gridSystem.CanTakeArea(areaTemp);
    }

    public void Place()
    {
        BoundsInt areaTemp = area;
        areaTemp.position = gridSystem.GridLayout.WorldToCell(transform.position);
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

    public void Clear()
    {
        if (IsPlaced)
        {
            BoundsInt areaTemp = area;
            areaTemp.position = gridSystem.GridLayout.WorldToCell(transform.position);
            gridSystem.ClearArea(areaTemp);
            IsPlaced = false;
        }
    }
}
