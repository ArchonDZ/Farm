using UnityEngine;
using Zenject;

public class Seed : MonoBehaviour
{
    [SerializeField] private PlacebleObject placebleObject;
    [SerializeField] private Plant plant;

    [Inject] GridSystem gridSystem;

    void Awake()
    {
        placebleObject.OnPlaceEvent += PlacebleObject_OnPlaceEvent;
    }

    private void PlacebleObject_OnPlaceEvent()
    {
        gridSystem.InitiazeObjectOnPosition(plant.gameObject, transform.position);
    }
}
