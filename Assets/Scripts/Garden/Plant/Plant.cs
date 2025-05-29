using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public struct PlantStage
{
    public TimePeriod timeGrowth;
    public Sprite sprite;
}

public class Plant : InitializableObject
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider;
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
            LoadState(placeableData.State);
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
        else if (state is StateDecorator decorator && decorator.TryGetPlantState(out Thirst decoratedThirst))
        {
            decoratedThirst.EndState();
        }
    }

    public void Spray()
    {
        if (state is Pest pest)
        {
            pest.EndState();
        }
        else if (state is StateDecorator decorator && decorator.TryGetPlantState(out Pest decoratedPest))
        {
            decoratedPest.EndState();
        }
    }

    public void Harvest()
    {
        if (state is WaitHarvest || (state is StateDecorator decorator && decorator.TryGetPlantState<WaitHarvest>(out _)))
        {
            collectionSystem.AddDrops(plantItem.DefinitelyDrops);
            if (placeableData.IsFertilized && 0 < plantItem.DefinitelyDrops.Count)
            {
                collectionSystem.AddDrop(plantItem.DefinitelyDrops[Random.Range(0, plantItem.DefinitelyDrops.Count)]);
            }

            for (int i = 0; i < plantItem.CountRandomDrop; i++)
            {
                collectionSystem.AddDrop(plantItem.RandomDrop.GetRandomValue());
            }
            Destroy();
        }
    }

    public void Fertilize(float accelerationGrowth)
    {
        if (!TryGetDecorator(out Fertilized fertilized))
        {
            SetDecorator(new Fertilized(this, state, accelerationGrowth));
        }
        else
        {
            fertilized.ProlongFertilize(accelerationGrowth);
        }
    }

    public void SetStage(PlantStage plantStage)
    {
        stage = plantStage;

        if (placeableData != null)
            placeableData.Stage = plantItem.Stages.IndexOf(stage);

        spriteRenderer.sprite = stage.sprite;
        polygonCollider.UpdateColliderToSprite(stage.sprite);
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
                FindResetDecorator(decorator, stateDecoratorReset);
            }
        }
    }

    public bool TryGetDecorator<T>(out T result) where T : StateDecorator
    {
        if (state is not StateDecorator decorator || !(decorator is T desired || decorator.TryGetPlantState(out desired)))
        {
            result = null;
            return false;
        }
        else
        {
            result = desired;
            return true;
        }
    }

    private void FindResetDecorator(StateDecorator upperDecorator, StateDecorator stateDecoratorReset)
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
                FindResetDecorator(decorator, stateDecoratorReset);
            }
        }
    }

    private void LoadState(PlantState plantState)
    {
        state = plantState;
        IndicateUnpackState(plantState);

        void IndicateUnpackState(PlantState plantState)
        {
            if (plantState is StateDecorator decorator)
            {
                stateIndicator.SetDecorator(decorator);
                IndicateUnpackState(decorator.GetState());
            }
            else
            {
                stateIndicator.UpdateState(plantState);
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
