public class ShovelTool : Tool
{
    protected override void Apply(Plant plant)
    {
        plant.Dig();
    }
}
