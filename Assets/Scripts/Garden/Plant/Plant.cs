using System;
using UnityEngine;
using Zenject;

[Serializable]
public struct PlantStage
{
    public TimePeriod timeGrowth;
    public Sprite sprite;
}

public class Plant : InitializableObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlacebleObject placebleObject;
    [SerializeField] private PlantStateIndicator stateIndicator;

    [Inject] CollectionSystem collectionSystem;

    private PlantItem plantItem;
    private PlantState state;
    private PlacebleData placebleData;

    public PlantStage Stage { get; set; }
    public PlantState State { get => state; set { state = value; stateIndicator.UpdateState(value); } }
    public PlantItem PlantItem => plantItem;

    void Awake()
    {
        placebleObject.Place();
    }

    void Update()
    {
        State.UpdateState();
    }

    void OnApplicationQuit()
    {
        if (placebleData == null)
        {
            placebleData = new PlantPlacebleData(plantItem.Id, transform.position, State);
            collectionSystem.AddPlaceble(placebleData);
        }
    }

    public override void Initialize(InitializableItem initializableItem, PlacebleData placebleData)
    {
        plantItem = initializableItem as PlantItem;
        if (placebleData == null)
        {
            State = new Growth(this);
        }
        else
        {
            this.placebleData = placebleData;
            State = (placebleData as PlantPlacebleData)?.State;
        }
    }

    public void UpdateSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void Dig()
    {
        Debug.Log("Dig");
        placebleObject.Clear();
        Destroy(gameObject);
    }

    public void Irrigate()
    {
        Debug.Log("Irrigate");
        if (State is Thirst thirst)
        {
            thirst.EndState();
        }
    }

    public void Harvest()
    {
        Debug.Log("Harvest");
        if (State is WaitHarvest)
        {
            collectionSystem.AddDrops(plantItem.Drops);
            placebleObject.Clear();
            Destroy(gameObject);
        }
    }
}
