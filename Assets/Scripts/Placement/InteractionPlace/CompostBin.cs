using R3;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CompostBin : MonoBehaviour, IInteractionPlace, IPointerClickHandler
{
    [SerializeField] private int maxFillAmount = 10;
    [SerializeField] private float fillInterval = 1f;
    [SerializeField] private GameObject fullIndicator;
    [SerializeField] private SpriteTransition spriteTransition;
    [SerializeField] private CollectibleItem fertilizerCollectibleItem;
    [SerializeField] private ParticleSystem particle;

    [Inject] private SaveSystem saveSystem;
    [Inject] private CollectionSystem collectionSystem;

    private CompostBinData data;
    private float lastInteractionTime;

    void Start()
    {
        if (saveSystem.TryGetSave(out CompostBinData compostBinData))
        {
            data = compostBinData;
        }
        else
        {
            data = new CompostBinData();
            saveSystem.AddSave(data);
        }
        data.ReactiveAmount.Subscribe(CheckAmount);
    }

    #region IInteractionPlace
    public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
    {
        if (data.Amount >= maxFillAmount) return;
        if (Time.time - lastInteractionTime < fillInterval) return;
        lastInteractionTime = Time.time;

        if (collectibleObject.CollectibleItem is SeedCollectibleItem ||
        collectibleObject.CollectibleItem is CropCollectibleItem)
        {
            collectibleObject.Spend();
            data.Amount++;
            spriteTransition.Play(collectibleObject.CollectibleItem.Icon, placementHelper.transform.position, Vector3.up, transform.position, Vector3.up, 0.8f);
        }
    }
    #endregion

    #region IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (data.Amount >= maxFillAmount)
        {
            collectionSystem.AddDrop(new Drop(1, fertilizerCollectibleItem));
            fullIndicator.SetActive(false);
            particle.Stop();
            data.Amount = 0;
        }
    }
    #endregion

    private void CheckAmount(int amount)
    {
        if (amount >= maxFillAmount)
        {
            fullIndicator.SetActive(true);
            particle.Play();
        }
    }

    #region Data
    [Serializable]
    private class CompostBinData : ISaveData
    {
        [SerializeField] private readonly SerializableReactiveProperty<int> amount = new SerializableReactiveProperty<int>();

        public ReadOnlyReactiveProperty<int> ReactiveAmount => amount;
        public int Amount { get => amount.Value; set => amount.Value = value; }
    }
    #endregion
}
