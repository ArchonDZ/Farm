using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRandomPlantItem", menuName = "Farm/InitializableItem/Plants/RandomPlantItem")]
public class RandomPlantItem : InitializableItem
{
    public List<PlantItem> Plants = new List<PlantItem>();
}
