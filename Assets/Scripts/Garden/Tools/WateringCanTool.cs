public class WateringCanTool : Tool
{
    protected override void Apply(Plant plant)
    {
        plant.Irrigate();
    }
}
