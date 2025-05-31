using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
        InitializeGrowth(DateTime.Now, true);
        RecoverThirst();
        RecoverPest();
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
        DateTime now = DateTime.Now;
        if (plant.Data.PestTime.CompareTo(now) <= 0)
        {
            if (ChanceOfPest())
            {
                plant.SetState(new Pest(plant, this));
                return;
            }
            RecoverPest();
        }

        if (plant.Data.ThirstTime.CompareTo(now) <= 0)
        {
            plant.SetState(new Thirst(plant, this));
            return;
        }

        if (plant.Data.GrowthTime.CompareTo(now) <= 0)
        {
            InitializeGrowth(now, true);
        }
    }

    public void RecoverThirst()
    {
        plant.Data.ThirstTime = DateTime.Now.AddSeconds(plant.Item.ThirstTimeSpan.TotalSeconds);
    }

    public void RecoverPest()
    {
        plant.Data.PestTime = DateTime.Now.AddSeconds(plant.Item.PestTimeSpan.TotalSeconds);
    }

    public void RecoverGrowth(DateTime datum)
    {
        double freezeTime = DateTime.Now.Subtract(datum).TotalSeconds;
        plant.Data.GrowthTime = plant.Data.GrowthTime.AddSeconds(freezeTime);
        CheckFertilizer();
    }

    private bool ChanceOfPest()
    {
        return Random.Range(1, 101) <= plant.Item.ChanceOfPest;
    }

    private void InitializeGrowth(DateTime datum, bool animated)
    {
        plant.SetStage(plant.Item.Stages[plant.Item.Stages.IndexOf(plant.Stage) + 1], animated);
        int indexNextStage = plant.Item.Stages.IndexOf(plant.Stage) + 1;
        if (indexNextStage <= plant.Item.Stages.Count - 1)
        {
            PlantStage nextStage = plant.Item.Stages[indexNextStage];
            TimeSpan growthTime = new TimeSpan(nextStage.timeGrowth.Days, nextStage.timeGrowth.Hours, nextStage.timeGrowth.Minutes, nextStage.timeGrowth.Seconds);
            plant.Data.GrowthTime = datum.AddSeconds(growthTime.TotalSeconds);
            CheckFertilizer();
        }
        else
        {
            plant.SetState(new WaitHarvest(plant));
        }
    }

    private void RecalculateTime()
    {
        DateTime now = DateTime.Now;
        DateTime thirstEndTime = plant.Data.ThirstTime;
        DateTime pestEndTime = plant.Data.PestTime;

        for (int i = plant.Item.Stages.IndexOf(plant.Stage); i < plant.Item.Stages.Count - 1; i++)
        {
            DateTime growthEndTime = plant.Data.GrowthTime;

            if (pestEndTime.CompareTo(now) <= 0 && pestEndTime.CompareTo(thirstEndTime) <= 0 &&
                pestEndTime.CompareTo(growthEndTime) <= 0 && ChanceOfPest())
            {
                plant.SetState(new Pest(plant, this));
                break;
            }
            else if (thirstEndTime.CompareTo(now) <= 0 && thirstEndTime.CompareTo(growthEndTime) <= 0)
            {
                plant.SetState(new Thirst(plant, this));
                break;
            }
            else if (growthEndTime.CompareTo(now) <= 0)
            {
                InitializeGrowth(growthEndTime, false);
            }
        }
    }

    private void CheckFertilizer()
    {
        if (plant.TryGetDecorator(out Fertilized fertilized))
        {
            fertilized.RecalculateGrowth();
        }
    }
}

[Serializable]
public abstract class AwaitingPlantState : PlantState
{
    [SerializeField] protected readonly Growth lastStateGrowth;

    public AwaitingPlantState(Plant plant, Growth growth) : base(plant)
    {
        lastStateGrowth = growth;

        if (plant.TryGetDecorator(out Fertilized fertilized))
        {
            fertilized.CancelAccelerationGrowth();
        }
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        lastStateGrowth.InitializeFromState(plant);
    }

    public override void UpdateState() { }

    public abstract void EndState();
}

[Serializable]
public class Pest : AwaitingPlantState
{
    public Pest(Plant plant, Growth growth) : base(plant, growth) { }

    public override void EndState()
    {
        lastStateGrowth.RecoverGrowth(plant.Data.PestTime);
        lastStateGrowth.RecoverPest();
        plant.SetState(lastStateGrowth);
    }
}

[Serializable]
public class Thirst : AwaitingPlantState
{
    public Thirst(Plant plant, Growth growth) : base(plant, growth) { }

    public override void EndState()
    {
        lastStateGrowth.RecoverGrowth(plant.Data.ThirstTime);
        lastStateGrowth.RecoverThirst();
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

    public bool TryGetPlantState<T>(out T state) where T : PlantState
    {
        if (packedPlantState is T)
        {
            state = packedPlantState as T;
            return true;
        }
        else if (packedPlantState is not StateDecorator decorator)
        {
            state = null;
            return false;
        }
        else
        {
            return decorator.TryGetPlantState(out state);
        }
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

[Serializable]
public class Fertilized : StateDecorator
{
    [SerializeField] private float accelerationGrowth = 1f;
    [SerializeField] private DateTime fertilizeTime;

    public Fertilized(Plant plant, PlantState _plantState, float accelerationGrowth) : base(plant, _plantState)
    {
        InitializeFertilized(accelerationGrowth);
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        CheckFertilizeTime();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        CheckFertilizeTime();
    }

    public void CancelAccelerationGrowth()
    {
        DateTime currentTime = DateTime.Now;
        double remainGrowthTime = plant.Data.GrowthTime.Subtract(currentTime).TotalSeconds;
        plant.Data.GrowthTime = currentTime.AddSeconds(remainGrowthTime * accelerationGrowth);
    }

    public void RecalculateGrowth()
    {
        RecalculateGrowth(fertilizeTime.Subtract(DateTime.Now).TotalSeconds);
    }

    public void ProlongFertilize(float accelerationGrowth)
    {
        DateTime previousFertilizeTime = fertilizeTime;
        fertilizeTime = DateTime.Now.AddSeconds(plant.Item.FertilizeTimeSpan.TotalSeconds);
        double additiveFertilizeSeconds = fertilizeTime.Subtract(previousFertilizeTime).TotalSeconds;

        AccelerateGrowthTime(accelerationGrowth, additiveFertilizeSeconds);
    }

    private void InitializeFertilized(float accelerationGrowth)
    {
        plant.Data.IsFertilized = true;
        fertilizeTime = DateTime.Now.AddSeconds(plant.Item.FertilizeTimeSpan.TotalSeconds);

        AccelerateGrowthTime(accelerationGrowth, plant.Item.FertilizeTimeSpan.TotalSeconds);
    }

    private void AccelerateGrowthTime(float acceleration, double fertilizedTime)
    {
        accelerationGrowth = Mathf.Max(accelerationGrowth, plant.Item.FertilizerMultiplier * acceleration);
        RecalculateGrowth(fertilizedTime);
    }

    private void RecalculateGrowth(double fertilizedTime)
    {
        if (fertilizeTime.CompareTo(plant.Data.GrowthTime) < 0)
        {
            double savedTime = fertilizedTime - (fertilizedTime * accelerationGrowth);
            plant.Data.GrowthTime = plant.Data.GrowthTime.AddSeconds(savedTime);
        }
        else
        {
            DateTime currentTime = DateTime.Now;
            double remainingTime = plant.Data.GrowthTime.Subtract(currentTime).TotalSeconds;
            double newTime = remainingTime / accelerationGrowth;
            plant.Data.GrowthTime = currentTime.AddSeconds(newTime);
        }
    }

    private void CheckFertilizeTime()
    {
        if (fertilizeTime.CompareTo(DateTime.Now) <= 0)
        {
            plant.ResetDecorator(this);
        }
    }
}