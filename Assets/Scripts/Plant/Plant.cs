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
    [SerializeField] private List<PlantStage> stages = new List<PlantStage>();

    private float startStageTime;
    private float endStageTime;
    private int indexStage;

    public PlantStage Stage { get; private set; }

    void Awake()
    {
        InitPlant(indexStage);
    }

    void Update()
    {
        if (indexStage == stages.Count - 1)
        {
            enabled = false;
            return;
        }

        Growth();
    }

    public void Dig()
    {
        Debug.Log("Dig");
    }

    public void Irrigate()
    {
        Debug.Log("Irrigate");
    }

    public void Harvest()
    {
        Debug.Log("Harvest");
    }

    private void InitPlant(int index)
    {
        Stage = stages[index];
        spriteRenderer.sprite = Stage.sprite;
        startStageTime = Time.time;
        endStageTime = UnityEngine.Random.Range(Stage.timeGrowthMin, Stage.timeGrowthMax);
        indexStage = index;
    }

    private void Growth()
    {
        if (Time.time - startStageTime > endStageTime)
        {
            InitPlant(indexStage + 1);
        }
    }
}
