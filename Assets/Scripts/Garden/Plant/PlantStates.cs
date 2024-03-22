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

    public abstract void UpdateState();
}

[Serializable]
public class Growth : PlantState
{
    private float remainingGrowthTime;
    private float remainingThirstTime;

    public Growth(Plant plant) : base(plant)
    {
        InitializeGrowth();
        Recover();
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
            if (plant.PlantItem.Stages.IndexOf(plant.Stage) + 1 <= plant.PlantItem.Stages.Count - 1)
            {
                InitializeGrowth();
            }
            else
            {
                plant.State = new WaitHarvest(plant);
            }
        }
    }

    public void Recover()
    {
        remainingThirstTime = plant.PlantItem.ThirstTimeSpan.Seconds;
    }

    private void InitializeGrowth()
    {
        plant.Stage = plant.PlantItem.Stages[plant.PlantItem.Stages.IndexOf(plant.Stage) + 1];
        plant.UpdateSprite(plant.Stage.sprite);
        TimeSpan endGrowthTime = new TimeSpan(plant.Stage.timeGrowth.Days, plant.Stage.timeGrowth.Hours, plant.Stage.timeGrowth.Minutes, plant.Stage.timeGrowth.Seconds);
        remainingGrowthTime = endGrowthTime.Seconds;
    }
}

[Serializable]
public class Thirst : PlantState
{
    private readonly Growth lastStateGrowth;

    public Thirst(Plant plant, Growth growth) : base(plant)
    {
        lastStateGrowth = growth;
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

    public override void UpdateState()
    {
        plant.UpdateSprite(plant.PlantItem.HarvestSprite);
    }
}