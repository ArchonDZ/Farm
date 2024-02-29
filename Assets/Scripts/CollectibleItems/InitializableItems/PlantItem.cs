using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantItem", menuName = "Farm/InitializableItem/PlantItem")]
public class PlantItem : InitializableItem
{
    public List<PlantStage> stages = new List<PlantStage>();
    public List<Drop> drops = new List<Drop>();
}
