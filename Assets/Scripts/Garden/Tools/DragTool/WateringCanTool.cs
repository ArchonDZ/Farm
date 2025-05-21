public class WateringCanTool : DragTool
{
    protected override void Apply(Plant plant)
    {
        plant.Irrigate();
    }
}
