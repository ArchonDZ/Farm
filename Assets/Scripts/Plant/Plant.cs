using System;
using System.Collections.Generic;
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

    [Inject] CollectionSystem collectionSystem;

    private PlantItem plantItem;

    public PlantStage Stage { get; set; }
    public PlantState State { get; set; }
    public List<PlantStage> PlantStages => plantItem.stages;

    void Awake()
    {
        placebleObject.Place();
    }

    void Start()
    {
        State = new Growth(this, UnityEngine.Random.Range(PlantStages[0].timeGrowthMin, PlantStages[0].timeGrowthMax));
    }

    void Update()
    {
        State.UpdateState();
    }

    public override void Initialize(InitializableItem initializableItem)
    {
        plantItem = initializableItem as PlantItem;
    }

    public void UpdateSpriteStage()
    {
        spriteRenderer.sprite = Stage.sprite;
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
    }

    public void Harvest()
    {
        Debug.Log("Harvest");
        if (State is WaitHarvest)
        {
            collectionSystem.AddDrops(plantItem.drops);
            placebleObject.Clear();
            Destroy(gameObject);
        }
    }
}
