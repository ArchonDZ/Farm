public class ScytheTool : DragTool
{
    protected override void Apply(Plant plant)
    {
        plant.Harvest();
    }
}
