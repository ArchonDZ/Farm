using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantItem", menuName = "Farm/InitializableItem/PlantItem")]
public class PlantItem : InitializableItem
{
    public float ThirstTime;
    public Sprite HarvestSprite;
    public List<PlantStage> Stages = new List<PlantStage>();
    public List<Drop> Drops = new List<Drop>();
}
