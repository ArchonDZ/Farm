using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class CompostBin : MonoBehaviour, IInteractionPlace, IPointerClickHandler
{
	[SerializeField] private int _maxFillAmount = 3;
	[SerializeField] private float _fillInterval = 1f;
	[SerializeField] private GameObject _fullIndicator;
	[SerializeField] private SpriteTransition _spriteTransition;
	[SerializeField] private CollectibleItem _fertilizerCollectibleItem;
	[SerializeField] private ParticleSystem _particle;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private List<Sprite> _stages = new();

	[Inject] private readonly SaveSystem _saveSystem;
	[Inject] private readonly CollectionSystem _collectionSystem;

	private CompostBinData _data;
	private float _lastInteractionTime;

	void Start()
	{
		if (_saveSystem.TryGetSave(out CompostBinData compostBinData))
		{
			_data = compostBinData;
		}
		else
		{
			_data = new CompostBinData();
			_saveSystem.AddSave(_data);
		}
		_data.ReactiveAmount.Subscribe(CheckAmount).AddTo(this);
	}

	#region IInteractionPlace
	public void Interaction(PlacementHelper placementHelper, CollectibleObject collectibleObject)
	{
		if (_data.Amount >= _maxFillAmount) return;
		if (Time.time - _lastInteractionTime < _fillInterval) return;

		_lastInteractionTime = Time.time;

		if (collectibleObject.CollectibleItem is SeedCollectibleItem ||
			collectibleObject.CollectibleItem is CropCollectibleItem)
		{
			collectibleObject.Spend();
			_data.Amount++;
			_spriteTransition.Play(collectibleObject.CollectibleItem.Icon, placementHelper.transform.position, Vector3.up, transform.position, Vector3.up, 0.8f);
		}
	}

	public void Interaction(PlacementHelper placementHelper, PlaceableObject placeableObject)
	{
		if (_data.Amount >= _maxFillAmount)
		{
			placementHelper.FinishReplacePlaceable();
			return;
		}

		if (placeableObject.TryGetComponent(out Plant plant))
		{
			plant.Dig();
			_data.Amount++;
			_spriteTransition.Play(plant.Stage.sprite, placementHelper.transform.position, Vector3.up, transform.position, Vector3.up, 0.8f);
		}
	}
	#endregion

	#region IPointerClickHandler
	public void OnPointerClick(PointerEventData eventData)
	{
		if (_data.Amount >= _maxFillAmount)
		{
			_collectionSystem.AddDrop(new Drop(1, _fertilizerCollectibleItem));
			_fullIndicator.SetActive(false);
			_particle.Stop();
			_data.Amount = 0;
		}
	}
	#endregion

	private void CheckAmount(int amount)
	{
		if (amount >= _maxFillAmount)
		{
			_fullIndicator.SetActive(true);
			_particle.Play();
		}
		SetStage(amount);
	}

	private void SetStage(int amount)
	{
		int index = Mathf.Clamp(amount, 0, _maxFillAmount);
		if (_stages != null && _stages.Count > 0 && index < _stages.Count)
			_spriteRenderer.sprite = _stages[index];
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
