using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantItem", menuName = "Farm/InitializableItem/PlantItem")]
public class PlantItem : InitializableItem
{
    public int chanceOfPest;
    public TimePeriod PestTime;
    public TimePeriod ThirstTime;
    [Header("Stages")]
    public List<PlantStage> Stages = new List<PlantStage>();
    [Header("Drops")]
    public List<Drop> DefinitelyDrops = new List<Drop>();
    public int countRandomDrop;
    public WeightedList<Drop> RandomDrop = new WeightedList<Drop>();

    public TimeSpan ThirstTimeSpan;
    public TimeSpan PestTimeSpan;

    void OnValidate()
    {
        ThirstTimeSpan = new TimeSpan(ThirstTime.Days, ThirstTime.Hours, ThirstTime.Minutes, ThirstTime.Seconds);
        PestTimeSpan = new TimeSpan(PestTime.Days, PestTime.Hours, PestTime.Minutes, PestTime.Seconds);
    }
}
