using DG.Tweening;
using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public struct PlantStage
{
	public TimePeriod timeGrowth;
	public Sprite sprite;
}

public class Plant : InitializableObject
{
	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private PolygonCollider2D _polygonCollider;
	[SerializeField] private PlaceableObject _placeableObject;
	[SerializeField] private PlantStateIndicator _stateIndicator;

	[Header("Stage Transition Settings")]
	[SerializeField] private float _transitionEndScaleY = 1.1f;
	[SerializeField] private float _transitionScaleDurationTime = 0.5f;
	[SerializeField] private Ease _transitionScaleEase;

	[Inject] readonly CollectionSystem _collectionSystem;

	private PlantItem _plantItem;
	private PlantPlaceableData _placeableData;
	private PlantStage _stage;
	private PlantState _state;
	private Tween _tweenTransitionScale;

	public PlantItem Item => _plantItem;
	public PlantPlaceableData Data => _placeableData;
	public PlantStage Stage => _stage;

	void Awake()
	{
		_placeableObject.Place();
		_placeableObject.OnPlaceEvent += PlaceableObject_OnPlaceEvent;
	}

	void Update()
	{
		_state.UpdateState();
	}

	void OnDestroy()
	{
		_tweenTransitionScale?.Kill();
	}

	public override void Initialize(InitializableItem initializableItem, PlaceableData savedPlaceableData)
	{
		_plantItem = initializableItem as PlantItem;
		if (savedPlaceableData == null)
		{
			_placeableData = new PlantPlaceableData(_plantItem.Id, transform.position, _state);
			SetState(_plantItem.Stages.Count > 1 ? new Growth(this) : new WaitHarvest(this));
			SetStage(_plantItem.Stages[0], false);
			_collectionSystem.AddPlaceable(_placeableData);
		}
		else
		{
			_placeableData = savedPlaceableData as PlantPlaceableData;
			LoadState(_placeableData.State);
			int stageIndex = Mathf.Clamp(_placeableData.Stage, 0, _plantItem.Stages.Count - 1);
			_placeableData.Stage = stageIndex;
			SetStage(_plantItem.Stages[stageIndex], false);
			_placeableData.State.Initialize(this);
		}
	}

	public void Dig()
	{
		Destroy();
	}

	public void Irrigate()
	{
		if (_state is Thirst thirst)
		{
			thirst.EndState();
		}
		else if (_state is StateDecorator decorator && decorator.TryGetPlantState(out Thirst decoratedThirst))
		{
			decoratedThirst.EndState();
		}
	}

	public void Spray()
	{
		if (_state is Pest pest)
		{
			pest.EndState();
		}
		else if (_state is StateDecorator decorator && decorator.TryGetPlantState(out Pest decoratedPest))
		{
			decoratedPest.EndState();
		}
	}

	public void Harvest()
	{
		if (_state is WaitHarvest || (_state is StateDecorator decorator && decorator.TryGetPlantState<WaitHarvest>(out _)))
		{
			_collectionSystem.AddDrops(_plantItem.DefinitelyDrops);
			if (_placeableData.IsFertilized && 0 < _plantItem.DefinitelyDrops.Count)
			{
				_collectionSystem.AddDrop(_plantItem.DefinitelyDrops[Random.Range(0, _plantItem.DefinitelyDrops.Count)]);
			}

			for (int i = 0; i < _plantItem.CountRandomDrop; i++)
			{
				_collectionSystem.AddDrop(_plantItem.RandomDrop.GetRandomValue());
			}
			Destroy();
		}
	}

	public void Fertilize(float accelerationGrowth)
	{
		if (!TryGetDecorator(out Fertilized fertilized))
		{
			SetDecorator(new Fertilized(this, _state, accelerationGrowth));
		}
		else
		{
			fertilized.ProlongFertilize(accelerationGrowth);
		}
	}

	public void SetStage(PlantStage plantStage, bool animated)
	{
		_stage = plantStage;

		if (_placeableData != null)
			_placeableData.Stage = _plantItem.Stages.IndexOf(_stage);

		if (animated)
			TweenUpdateSprite();
		else
			UpdateSprite();
	}

	public void SetState(PlantState plantState)
	{
		if (_state is StateDecorator decorator)
		{
			decorator.PackState(plantState);
		}
		else
		{
			_state = plantState;
			UpdateStateData(_state);
		}

		_stateIndicator.UpdateState(plantState);
	}

	public void SetDecorator(StateDecorator stateDecorator)
	{
		_state = stateDecorator;
		UpdateStateData(_state);
		_stateIndicator.SetDecorator(stateDecorator);
	}

	public void ResetDecorator(StateDecorator stateDecoratorReset)
	{
		if (_state is StateDecorator decorator)
		{
			if (ReferenceEquals(decorator, stateDecoratorReset))
			{
				_state = decorator.GetState();
				UpdateStateData(_state);
				_stateIndicator.ResetDecorator(decorator);
			}
			else
			{
				FindResetDecorator(decorator, stateDecoratorReset);
			}
		}
	}

	public bool TryGetDecorator<T>(out T result) where T : StateDecorator
	{
		if (_state is not StateDecorator decorator || !(decorator is T desired || decorator.TryGetPlantState(out desired)))
		{
			result = null;
			return false;
		}
		else
		{
			result = desired;
			return true;
		}
	}

	private void FindResetDecorator(StateDecorator upperDecorator, StateDecorator stateDecoratorReset)
	{
		PlantState packedState = upperDecorator.GetState();
		if (packedState is StateDecorator decorator)
		{
			if (ReferenceEquals(decorator, stateDecoratorReset))
			{
				PlantState lowerState = decorator.GetState();
				upperDecorator.SetState(lowerState);
				_stateIndicator.ResetDecorator(decorator);
			}
			else
			{
				FindResetDecorator(decorator, stateDecoratorReset);
			}
		}
	}

	private void LoadState(PlantState plantState)
	{
		_state = plantState;
		IndicateUnpackState(plantState);

		void IndicateUnpackState(PlantState plantState)
		{
			if (plantState is StateDecorator decorator)
			{
				_stateIndicator.SetDecorator(decorator);
				IndicateUnpackState(decorator.GetState());
			}
			else
			{
				_stateIndicator.UpdateState(plantState);
			}
		}
	}

	private void TweenUpdateSprite()
	{
		_tweenTransitionScale = _spriteRenderer.transform.DOScaleY(_transitionEndScaleY, _transitionScaleDurationTime)
			.SetEase(_transitionScaleEase)
			.OnComplete(() =>
			{
				_tweenTransitionScale = null;
				_spriteRenderer.transform.localScale = Vector3.one;
				UpdateSprite();
			});
	}

	private void UpdateSprite()
	{
		_spriteRenderer.sprite = _stage.sprite;
		_polygonCollider.UpdateColliderToSprite(_stage.sprite);
	}

	private void UpdateStateData(PlantState plantState)
	{
		if (_placeableData != null)
		{
			_placeableData.State = plantState;
		}
	}

	private void PlaceableObject_OnPlaceEvent()
	{
		_placeableData.Position = transform.position;
	}

	private void Destroy()
	{
		_collectionSystem.RemovePlaceable(_placeableData);
		_placeableObject.Clear();
		Destroy(gameObject);
	}
}
