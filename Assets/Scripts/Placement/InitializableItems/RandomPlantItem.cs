using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRandomPlantItem", menuName = "Farm/InitializableItem/PlantItem/Random")]
public class RandomPlantItem : InitializableItem
{
    public List<PlantItem> Plants = new List<PlantItem>();
}
