using System;
using UnityEngine;
using Zenject;

[Serializable]
public struct PlantStage
{
    public float timeGrowthMin;
    public float timeGrowthMax;
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

    public PlantStage Stage { get; set; }
    public PlantState State { get => state; set { state = value; stateIndicator.UpdateState(value); } }
    public PlantItem PlantItem => plantItem;

    void Awake()
    {
        placebleObject.Place();
    }

    void Start()
    {
        State = new Growth(this);
    }

    void Update()
    {
        State.UpdateState();
    }

    public override void Initialize(InitializableItem initializableItem)
    {
        plantItem = initializableItem as PlantItem;
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
