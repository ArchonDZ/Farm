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

    private PlantItem plantItem;
    private PlantPlaceableData placeableData;
    private PlantStage stage;
    private PlantState state;

    public PlantItem Item => plantItem;
    public PlantPlaceableData Data => placeableData;
    public PlantStage Stage => stage;

    void Awake()
    {
        placeableObject.Place();
    }

    void Update()
    {
        state.UpdateState();
    }

    public override void Initialize(InitializableItem initializableItem, PlaceableData savedPlaceableData)
    {
        plantItem = initializableItem as PlantItem;
        if (savedPlaceableData == null)
        {
            placeableData = new PlantPlaceableData(plantItem.Id, transform.position, state);
            SetState(new Growth(this));
            SetStage(plantItem.Stages[0]);
            collectionSystem.AddPlaceable(placeableData);
        }
        else
        {
            placeableData = savedPlaceableData as PlantPlaceableData;
            SetState(placeableData.State);
            SetStage(plantItem.Stages[placeableData.Stage]);
            placeableData.State.Initialize(this);
        }
    }

    public void Dig()
    {
        Destroy();
    }

    public void Irrigate()
    {
        if (state is Thirst thirst)
        {
            thirst.EndState();
        }
        else if (state is StateDecorator decorator && decorator.TryGetPlantState<Thirst>(out PlantState decoratedThirst))
        {
            (decoratedThirst as Thirst)?.EndState();
        }
    }

    public void Spray()
    {
        if (state is Pest pest)
        {
            pest.EndState();
        }
        else if (state is StateDecorator decorator && decorator.TryGetPlantState<Pest>(out PlantState decoratedPest))
        {
            (decoratedPest as Pest)?.EndState();
        }
    }

    public void Harvest()
    {
        if (state is WaitHarvest || (state is StateDecorator decorator && decorator.TryGetPlantState<WaitHarvest>(out _)))
        {
            collectionSystem.AddDrops(plantItem.DefinitelyDrops);

            int additiveFertilizeDrop = placeableData.IsFertilized ? 1 : 0;
            for (int i = 0; i < plantItem.CountRandomDrop + additiveFertilizeDrop; i++)
            {
                collectionSystem.AddDrop(plantItem.RandomDrop.GetRandomValue());
            }
            Destroy();
        }
    }

    public void Fertilize(float accelerationGrowth)
    {
        placeableData.TimeStartFertilize = DateTime.Now;
        placeableData.RemainingFertilizeTime = (float)plantItem.FertilizeTimeSpan.TotalSeconds;
        placeableData.AccelerationGrowth = Mathf.Max(placeableData.AccelerationGrowth, plantItem.FertilizerMultiplier * accelerationGrowth);
        placeableData.IsFertilized = true;

        SetDecorator(new Fertilized(this, state));
    }

    public void SetStage(PlantStage plantStage)
    {
        stage = plantStage;

        if (placeableData != null)
            placeableData.Stage = plantItem.Stages.IndexOf(stage);

        spriteRenderer.sprite = stage.sprite;
    }

    public void SetState(PlantState plantState)
    {
        if (state is StateDecorator decorator)
        {
            decorator.PackState(plantState);
        }
        else
        {
            state = plantState;
            UpdateStateData(state);
        }

        stateIndicator.UpdateState(plantState);
    }

    public void SetDecorator(StateDecorator stateDecorator)
    {
        state = stateDecorator;
        UpdateStateData(state);
        stateIndicator.SetDecorator(stateDecorator);
    }

    public void ResetDecorator(StateDecorator stateDecoratorReset)
    {
        if (state is StateDecorator decorator)
        {
            if (ReferenceEquals(decorator, stateDecoratorReset))
            {
                state = decorator.GetState();
                UpdateStateData(state);
                stateIndicator.ResetDecorator(decorator);
            }
            else
            {
                FindDecorator(decorator, stateDecoratorReset);
            }
        }
    }

    private void FindDecorator(StateDecorator upperDecorator, StateDecorator stateDecoratorReset)
    {
        PlantState packedState = upperDecorator.GetState();
        if (packedState is StateDecorator decorator)
        {
            if (ReferenceEquals(decorator, stateDecoratorReset))
            {
                PlantState lowerState = decorator.GetState();
                upperDecorator.SetState(lowerState);
                stateIndicator.ResetDecorator(decorator);
            }
            else
            {
                FindDecorator(decorator, stateDecoratorReset);
            }
        }
    }

    private void UpdateStateData(PlantState plantState)
    {
        if (placeableData != null)
        {
            placeableData.State = plantState;
        }
    }

    private void Destroy()
    {
        collectionSystem.RemovePlaceable(placeableData);
        placeableObject.Clear();
        Destroy(gameObject);
    }
}
