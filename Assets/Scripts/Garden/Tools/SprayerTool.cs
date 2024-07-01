public class SprayerTool : DragTool
{
    protected override void Apply(Plant plant)
    {
        plant.Spray();
    }
}
