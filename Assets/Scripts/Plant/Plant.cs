using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlantStage
{
    public float timeGrowthMin;
    public float timeGrowthMax;
    public Sprite sprite;
}

public class Plant : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlacebleObject placebleObject;
    [SerializeField] private List<PlantStage> stages = new List<PlantStage>();

    public PlantStage Stage { get; set; }
    public PlantState State { get; set; }
    public List<PlantStage> PlantStages => stages;

    void Awake()
    {
        placebleObject.Place();
    }

    void Start()
    {
        State = new Growth(this, UnityEngine.Random.Range(stages[0].timeGrowthMin, stages[0].timeGrowthMax));
    }

    void Update()
    {
        State.UpdateState();
    }

    public void UpdateSpriteStage()
    {
        spriteRenderer.sprite = Stage.sprite;
    }

    public void Dig()
    {
        Debug.Log("Dig");
        placebleObject.Clear();
        Destroy(gameObject);
    }

    public void Irrigate()
    {
        Debug.Log("Irrigate");
    }

    public void Harvest()
    {
        Debug.Log("Harvest");
        if (State is WaitHarvest)
        {
            placebleObject.Clear();
            Destroy(gameObject);
        }
    }
}
