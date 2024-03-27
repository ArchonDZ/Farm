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
    [SerializeField] private DateTime timeStartThirst;
    [SerializeField] private float remainingGrowthTime;
    [SerializeField] private float remainingThirstTime;

    public Growth(Plant plant) : base(plant)
    {
        InitializeGrowth();
        Recover();
    }

    public override void Initialize(Plant plant)
    {
        base.Initialize(plant);
        RecalculateTime();
    }

    public void InitializeFromThirst(Plant plant)
    {
        base.Initialize(plant);
    }

    public override void UpdateState()
    {
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

    public void Recover()
    {
        remainingThirstTime = plant.PlantItem.ThirstTimeSpan.Seconds;
        timeStartGrowth = DateTime.Now;
        timeStartThirst = DateTime.Now;
    }

    private void InitializeGrowth()
    {
        plant.Stage = plant.PlantItem.Stages[plant.PlantItem.Stages.IndexOf(plant.Stage) + 1];
        int indexNextStage = plant.PlantItem.Stages.IndexOf(plant.Stage) + 1;
        if (indexNextStage <= plant.PlantItem.Stages.Count - 1)
        {
            PlantStage nextPlantStage = plant.PlantItem.Stages[indexNextStage];
            TimeSpan endGrowthTime = new TimeSpan(nextPlantStage.timeGrowth.Days, nextPlantStage.timeGrowth.Hours, nextPlantStage.timeGrowth.Minutes, nextPlantStage.timeGrowth.Seconds);
            remainingGrowthTime = endGrowthTime.Seconds;
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
        DateTime thirstEndTime = timeStartThirst.AddSeconds(remainingThirstTime);
        DateTime growthEndTime = timeStartGrowth;

        for (int i = plant.PlantItem.Stages.IndexOf(plant.Stage); i < plant.PlantItem.Stages.Count - 1; i++)
        {
            growthEndTime = growthEndTime.AddSeconds(remainingGrowthTime);
            if (currentTime < growthEndTime && currentTime < thirstEndTime)
            {
                remainingGrowthTime = growthEndTime.Subtract(currentTime).Seconds;
                remainingThirstTime = thirstEndTime.Subtract(currentTime).Seconds;
                return;
            }
            else
            {
                if (thirstEndTime <= growthEndTime)
                {
                    remainingGrowthTime = growthEndTime.Subtract(thirstEndTime).Seconds;
                    plant.State = new Thirst(plant, this);
                    return;
                }
                else
                {
                    InitializeGrowth();
                }
            }
        }
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
        lastStateGrowth.InitializeFromThirst(plant);
    }

    public override void UpdateState() { }

    public void EndState()
    {
        lastStateGrowth.Recover();
        plant.State = lastStateGrowth;
    }
}

[Serializable]
public class WaitHarvest : PlantState
{
    public WaitHarvest(Plant plant) : base(plant) { }

    public override void UpdateState() { }
}