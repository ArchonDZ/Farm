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
    [SerializeField] private DateTime timeStartGrowth;
    [SerializeField] private DateTime timeStartPest;
    [SerializeField] private DateTime timeStartThirst;
    [SerializeField] private double remainingGrowthTime;
    [SerializeField] private double remainingPestTime;
    [SerializeField] private double remainingThirstTime;

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
        remainingPestTime -= Time.deltaTime;
        if (remainingPestTime <= 0)
        {
            if (ChanceOfPest())
            {
                plant.State = new Pest(plant, this);
                return;
            }
            RecoverPest();
        }

        remainingThirstTime -= Time.deltaTime;
        if (remainingThirstTime <= 0)
        {
            plant.State = new Thirst(plant, this);
            return;
        }

        remainingGrowthTime -= Time.deltaTime;
        if (remainingGrowthTime <= 0)
        {
            InitializeGrowth();
        }
    }

    public void RecoverThirst()
    {
        remainingThirstTime = plant.PlantItem.ThirstTimeSpan.TotalSeconds;
    }

    public void RecoverPest()
    {
        remainingPestTime = plant.PlantItem.PestTimeSpan.TotalSeconds;
    }

    public void RecoverTimers()
    {
        timeStartGrowth = DateTime.Now;
        timeStartPest = DateTime.Now;
        timeStartThirst = DateTime.Now;
    }

    private bool ChanceOfPest()
    {
        return UnityEngine.Random.Range(1, 101) <= plant.PlantItem.chanceOfPest;
    }

    private void InitializeGrowth()
    {
        plant.Stage = plant.PlantItem.Stages[plant.PlantItem.Stages.IndexOf(plant.Stage) + 1];
        int indexNextStage = plant.PlantItem.Stages.IndexOf(plant.Stage) + 1;
        if (indexNextStage <= plant.PlantItem.Stages.Count - 1)
        {
            PlantStage nextPlantStage = plant.PlantItem.Stages[indexNextStage];
            TimeSpan endGrowthTime = new TimeSpan(nextPlantStage.timeGrowth.Days, nextPlantStage.timeGrowth.Hours, nextPlantStage.timeGrowth.Minutes, nextPlantStage.timeGrowth.Seconds);
            remainingGrowthTime = endGrowthTime.TotalSeconds;
            timeStartGrowth = DateTime.Now;
        }
        else
        {
            plant.State = new WaitHarvest(plant);
        }
    }

    private void RecalculateTime()
    {
        DateTime currentTime = DateTime.Now;
        DateTime pestEndTime = timeStartPest.AddSeconds(remainingPestTime);
        DateTime thirstEndTime = timeStartThirst.AddSeconds(remainingThirstTime);
        DateTime growthEndTime = timeStartGrowth;

        for (int i = plant.PlantItem.Stages.IndexOf(plant.Stage); i < plant.PlantItem.Stages.Count - 1; i++)
        {
            growthEndTime = growthEndTime.AddSeconds(remainingGrowthTime);

            remainingGrowthTime = growthEndTime.Subtract(currentTime).TotalSeconds;
            remainingPestTime = pestEndTime.Subtract(currentTime).TotalSeconds;
            remainingThirstTime = thirstEndTime.Subtract(currentTime).TotalSeconds;

            if (remainingPestTime <= 0 && remainingPestTime <= remainingThirstTime && remainingPestTime <= remainingGrowthTime && ChanceOfPest())
            {
                plant.State = new Pest(plant, this);
                break;
            }
            else if (remainingThirstTime <= 0 && remainingThirstTime <= remainingGrowthTime)
            {
                plant.State = new Thirst(plant, this);
                break;
            }
            else if (remainingGrowthTime <= 0)
            {
                InitializeGrowth();
                break;
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
        plant.State = lastStateGrowth;
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
        plant.State = lastStateGrowth;
    }
}

[Serializable]
public class WaitHarvest : PlantState
{
    public WaitHarvest(Plant plant) : base(plant) { }

    public override void UpdateState() { }
}