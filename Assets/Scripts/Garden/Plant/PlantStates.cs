using System;
using UnityEngine;

[Serializable]
public abstract class PlantState
{
    protected Plant plant;

    public PlantState(Plant plant)
    {
        this.plant = plant;
    }

    public virtual void Initialize(Plant plant)
    {
        this.plant = plant;
    }

    public abstract void UpdateState();
}

[Serializable]
public class Growth : PlantState
{
    public Growth(Plant plant) : base(plant)
    {
        InitializeGrowth();
        RecoverThirst();
        RecoverPest();
        RecoverTimers();
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        RecalculateTime();
    }

    public void InitializeFromState(Plant plant)
    {
        base.Initialize(plant);
    }

    public override void UpdateState()
    {
        plant.Data.RemainingPestTime -= Time.deltaTime;
        if (plant.Data.RemainingPestTime <= 0)
        {
            if (ChanceOfPest())
            {
                plant.Data.RemainingPestTime = 0f;
                plant.SetState(new Pest(plant, this));
                return;
            }
            RecoverPest();
        }

        plant.Data.RemainingThirstTime -= Time.deltaTime;
        if (plant.Data.RemainingThirstTime <= 0)
        {
            plant.Data.RemainingThirstTime = 0f;
            plant.SetState(new Thirst(plant, this));
            return;
        }

        plant.Data.RemainingGrowthTime -= Time.deltaTime;
        if (plant.Data.RemainingGrowthTime <= 0)
        {
            InitializeGrowth();
        }
    }

    public void RecoverThirst()
    {
        plant.Data.RemainingThirstTime = (float)plant.Item.ThirstTimeSpan.TotalSeconds;
    }

    public void RecoverPest()
    {
        plant.Data.RemainingPestTime = (float)plant.Item.PestTimeSpan.TotalSeconds;
    }

    public void RecoverTimers()
    {
        plant.Data.TimeStartGrowth = DateTime.Now;
        plant.Data.TimeStartPest = DateTime.Now;
        plant.Data.TimeStartThirst = DateTime.Now;
    }

    private bool ChanceOfPest()
    {
        return UnityEngine.Random.Range(1, 101) <= plant.Item.ChanceOfPest;
    }

    private void InitializeGrowth()
    {
        plant.SetStage(plant.Item.Stages[plant.Item.Stages.IndexOf(plant.Stage) + 1]);
        int indexNextStage = plant.Item.Stages.IndexOf(plant.Stage) + 1;
        if (indexNextStage <= plant.Item.Stages.Count - 1)
        {
            PlantStage nextPlantStage = plant.Item.Stages[indexNextStage];
            TimeSpan endGrowthTime = new TimeSpan(nextPlantStage.timeGrowth.Days, nextPlantStage.timeGrowth.Hours, nextPlantStage.timeGrowth.Minutes, nextPlantStage.timeGrowth.Seconds);
            plant.Data.RemainingGrowthTime = (float)endGrowthTime.TotalSeconds;
            plant.Data.TimeStartGrowth = DateTime.Now;
        }
        else
        {
            plant.Data.RemainingGrowthTime = 0f;
            plant.SetState(new WaitHarvest(plant));
        }
    }

    private void RecalculateTime()
    {
        DateTime currentTime = DateTime.Now;
        DateTime pestEndTime = plant.Data.TimeStartPest.AddSeconds(plant.Data.RemainingPestTime);
        DateTime thirstEndTime = plant.Data.TimeStartThirst.AddSeconds(plant.Data.RemainingThirstTime);
        DateTime growthEndTime = plant.Data.TimeStartGrowth;

        for (int i = plant.Item.Stages.IndexOf(plant.Stage); i < plant.Item.Stages.Count - 1; i++)
        {
            growthEndTime = growthEndTime.AddSeconds(plant.Data.RemainingGrowthTime);

            plant.Data.RemainingGrowthTime = (float)growthEndTime.Subtract(currentTime).TotalSeconds;
            plant.Data.RemainingPestTime = (float)pestEndTime.Subtract(currentTime).TotalSeconds;
            plant.Data.RemainingThirstTime = (float)thirstEndTime.Subtract(currentTime).TotalSeconds;

            if (plant.Data.RemainingPestTime <= 0 && plant.Data.RemainingPestTime <= plant.Data.RemainingThirstTime &&
                plant.Data.RemainingPestTime <= plant.Data.RemainingGrowthTime && ChanceOfPest())
            {
                plant.Data.RemainingPestTime = 0f;
                plant.SetState(new Pest(plant, this));
                break;
            }
            else if (plant.Data.RemainingThirstTime <= 0 && plant.Data.RemainingThirstTime <= plant.Data.RemainingGrowthTime)
            {
                plant.Data.RemainingThirstTime = 0f;
                plant.SetState(new Thirst(plant, this));
                break;
            }
            else if (plant.Data.RemainingGrowthTime <= 0)
            {
                InitializeGrowth();
            }
        }
    }
}

[Serializable]
public class Pest : PlantState
{
    [SerializeField] private readonly Growth lastStateGrowth;

    public Pest(Plant plant, Growth growth) : base(plant)
    {
        lastStateGrowth = growth;
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        lastStateGrowth.InitializeFromState(plant);
    }

    public override void UpdateState() { }

    public void EndState()
    {
        lastStateGrowth.RecoverPest();
        lastStateGrowth.RecoverTimers();
        plant.SetState(lastStateGrowth);
    }
}

[Serializable]
public class Thirst : PlantState
{
    [SerializeField] private readonly Growth lastStateGrowth;

    public Thirst(Plant plant, Growth growth) : base(plant)
    {
        lastStateGrowth = growth;
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        lastStateGrowth.InitializeFromState(plant);
    }

    public override void UpdateState() { }

    public void EndState()
    {
        lastStateGrowth.RecoverThirst();
        lastStateGrowth.RecoverTimers();
        plant.SetState(lastStateGrowth);
    }
}

[Serializable]
public class WaitHarvest : PlantState
{
    public WaitHarvest(Plant plant) : base(plant) { }

    public override void UpdateState() { }
}

[Serializable]
public abstract class StateDecorator : PlantState
{
    [SerializeField] protected PlantState packedPlantState;

    public StateDecorator(Plant plant, PlantState _plantState) : base(plant)
    {
        SetState(_plantState);
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        packedPlantState.Initialize(plant);
    }

    public override void UpdateState()
    {
        packedPlantState.UpdateState();
    }

    public bool TryGetPlantState<T>(out PlantState state)
    {
        state = packedPlantState;

        if (packedPlantState is T)
            return true;
        else if (packedPlantState is StateDecorator decorator)
            return decorator.TryGetPlantState<T>(out state);
        else
            return false;
    }

    public void SetState(PlantState _plantState)
    {
        packedPlantState = _plantState;
    }

    public PlantState GetState()
    {
        return packedPlantState;
    }

    public void PackState(PlantState _plantState)
    {
        if (packedPlantState is StateDecorator decorator)
            decorator.PackState(_plantState);
        else
            SetState(_plantState);
    }
}

public class Fertilized : StateDecorator
{
    public Fertilized(Plant plant, PlantState _plantState) : base(plant, _plantState) { }

    public override void UpdateState()
    {
        plant.Data.RemainingFertilizeTime -= Time.deltaTime;
        if (plant.Data.RemainingFertilizeTime <= 0)
        {
            plant.Data.RemainingFertilizeTime = 0f;
            plant.ResetDecorator(this);
        }

        base.UpdateState();
    }
}