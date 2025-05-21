using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FertilizerTool : DropTool
{
    [SerializeField] private FertilizerCommonCollectibleItem collectibleItem;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image image;

    [Inject] private CollectionSystem collectionSystem;

    private CollectibleData data;

    void Start()
    {
        image.sprite = collectibleItem.Icon;
        collectionSystem.AddDrop(new Drop(0, collectibleItem));
        if (collectionSystem.TryGetCollectibleData(collectibleItem, out CollectibleData collectibleData))
        {
            data = collectibleData;
            data.Count.Subscribe(CheckRemainCount);
        }
    }

    private void CheckRemainCount(int count)
    {
        countText.text = count.ToString();
        gameObject.SetActive(0 < count);
    }

    protected override void Apply(Plant plant)
    {
        if (data.TrySpend(1))
            plant.Fertilize(collectibleItem.AccelerationGrowth);
    }
}
