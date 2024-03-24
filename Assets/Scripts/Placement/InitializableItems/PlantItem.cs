using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantItem", menuName = "Farm/InitializableItem/PlantItem")]
public class PlantItem : InitializableItem
{
    public TimePeriod ThirstTime;
    public List<PlantStage> Stages = new List<PlantStage>();
    public List<Drop> Drops = new List<Drop>();

    public TimeSpan ThirstTimeSpan;

    void OnValidate()
    {
        ThirstTimeSpan = new TimeSpan(ThirstTime.Days, ThirstTime.Hours, ThirstTime.Minutes, ThirstTime.Seconds);
    }
}
