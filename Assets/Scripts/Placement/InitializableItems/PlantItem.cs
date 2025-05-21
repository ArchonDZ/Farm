using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantItem", menuName = "Farm/InitializableItem/PlantItem")]
public class PlantItem : InitializableItem
{
    [Header("PlantItem")]
    [Min(1)] public float FertilizerMultiplier = 1;
    public TimePeriod FertilizeTime;

    [Space]
    public int ChanceOfPest;
    public TimePeriod PestTime;

    [Space]
    public TimePeriod ThirstTime;

    [Header("Stages")]
    public List<PlantStage> Stages = new List<PlantStage>();

    [Header("Drops")]
    public List<Drop> DefinitelyDrops = new List<Drop>();
    public int CountRandomDrop;
    public WeightedList<Drop> RandomDrop = new WeightedList<Drop>();

    public TimeSpan FertilizeTimeSpan;
    public TimeSpan ThirstTimeSpan;
    public TimeSpan PestTimeSpan;

    void OnValidate()
    {
        FertilizeTimeSpan = new TimeSpan(FertilizeTime.Days, FertilizeTime.Hours, FertilizeTime.Minutes, FertilizeTime.Seconds);
        ThirstTimeSpan = new TimeSpan(ThirstTime.Days, ThirstTime.Hours, ThirstTime.Minutes, ThirstTime.Seconds);
        PestTimeSpan = new TimeSpan(PestTime.Days, PestTime.Hours, PestTime.Minutes, PestTime.Seconds);
    }
}
