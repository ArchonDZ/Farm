public class ScytheTool : Tool
{
    protected override void Apply(Plant plant)
    {
        plant.Harvest();
    }
}
