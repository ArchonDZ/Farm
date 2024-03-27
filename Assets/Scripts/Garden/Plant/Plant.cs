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
    [SerializeField] private PlaceableObject placeableObject;
    [SerializeField] private PlantStateIndicator stateIndicator;

    [Inject] CollectionSystem collectionSystem;

    private PlantStage stage;
    private PlantState state;
    private PlantItem plantItem;
    private PlantPlaceableData placeableData;

    public PlantStage Stage { get => stage; set { stage = value; UpdateSprite(); } }
    public PlantState State { get => state; set { state = value; stateIndicator.UpdateState(value); } }
    public PlantItem PlantItem => plantItem;

    void Awake()
    {
        placeableObject.Place();
    }

    void Update()
    {
        State.UpdateState();
    }

    void OnApplicationQuit()
    {
        if (placeableData == null)
        {
            placeableData = new PlantPlaceableData(plantItem.Id, transform.position, plantItem.Stages.IndexOf(Stage), State);
            collectionSystem.AddPlaceable(placeableData);
        }
        else
        {
            placeableData.State = State;
            placeableData.Stage = plantItem.Stages.IndexOf(Stage);
        }
    }

    public override void Initialize(InitializableItem initializableItem, PlaceableData placeableData)
    {
        plantItem = initializableItem as PlantItem;
        if (placeableData == null)
        {
            State = new Growth(this);
        }
        else
        {
            this.placeableData = placeableData as PlantPlaceableData;
            State = this.placeableData.State;
            Stage = plantItem.Stages[this.placeableData.Stage];
            this.placeableData.State.Initialize(this);
        }
    }

    public void Dig()
    {
        Destroy();
    }

    public void Irrigate()
    {
        if (State is Thirst thirst)
        {
            thirst.EndState();
        }
    }

    public void Harvest()
    {
        if (State is WaitHarvest)
        {
            collectionSystem.AddDrops(plantItem.Drops);
            Destroy();
        }
    }

    private void Destroy()
    {
        collectionSystem.RemovePlaceable(placeableData);
        placeableObject.Clear();
        Destroy(gameObject);
    }

    private void UpdateSprite()
    {
        spriteRenderer.sprite = Stage.sprite;
    }
}
