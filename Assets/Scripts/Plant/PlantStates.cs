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
    private readonly float startStageTime;
    private readonly float endStageTime;

    public Growth(Plant plant, float endTime) : base(plant)
    {
        startStageTime = Time.time;
        endStageTime = endTime;
    }

    public override void UpdateState()
    {
        if (Time.time - startStageTime > endStageTime)
        {
            plant.Stage = plant.PlantStages[plant.PlantStages.IndexOf(plant.Stage) + 1];
            plant.UpdateSpriteStage();

            int nextIndexStage = plant.PlantStages.IndexOf(plant.Stage) + 1;
            if (nextIndexStage <= plant.PlantStages.Count - 1)
            {
                plant.State = new Growth(plant, Random.Range(plant.PlantStages[nextIndexStage].timeGrowthMin, plant.PlantStages[nextIndexStage].timeGrowthMax));
            }
            else
            {
                plant.State = new WaitHarvest(plant);
            }
        }
    }
}

public class WaitHarvest : PlantState
{
    public WaitHarvest(Plant plant) : base(plant) { }

    public override void UpdateState() { }
}