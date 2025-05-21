public class ShovelTool : DropTool
{
    protected override void Apply(Plant plant)
    {
        plant.Dig();
    }
}
