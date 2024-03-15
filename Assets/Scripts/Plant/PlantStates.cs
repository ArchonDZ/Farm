using UnityEngine;

public abstract class PlantState
{
    protected Plant plant;

    public PlantState(Plant plant)
    {
        this.plant = plant;
    }

    public abstract void UpdateState();
}

public class Growth : PlantState
{
    private float startGrowthTime;
    private float endGrowthTime;
    private float lastThirstTime;

    public Growth(Plant plant) : base(plant)
    {
        Recover();
        InitializeGrowth();
    }

    public override void UpdateState()
    {
        if (Time.time - startGrowthTime > endGrowthTime)
        {
            if (plant.PlantItem.Stages.IndexOf(plant.Stage) + 1 <= plant.PlantItem.Stages.Count - 1)
            {
                startGrowthTime = Time.time;
                InitializeGrowth();
            }
            else
            {
                plant.State = new WaitHarvest(plant);
            }
        }

        if (Time.time - lastThirstTime > plant.PlantItem.ThirstTime)
        {
            plant.State = new Thirst(plant, this);
            endGrowthTime -= Time.time - startGrowthTime;
        }
    }

    public void Recover()
    {
        startGrowthTime = Time.time;
        lastThirstTime = Time.time;
    }

    private void InitializeGrowth()
    {
        plant.Stage = plant.PlantItem.Stages[plant.PlantItem.Stages.IndexOf(plant.Stage) + 1];
        plant.UpdateSprite(plant.Stage.sprite);
        endGrowthTime = Random.Range(plant.Stage.timeGrowthMin, plant.Stage.timeGrowthMax);
    }
}

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

public class WaitHarvest : PlantState
{
    public WaitHarvest(Plant plant) : base(plant) { }

    public override void UpdateState()
    {
        plant.UpdateSprite(plant.PlantItem.HarvestSprite);
    }
}