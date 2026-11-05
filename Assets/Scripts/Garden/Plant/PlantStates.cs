using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class PlantState
{
	protected Plant _plant;

	public PlantState(Plant plant)
	{
		_plant = plant;
	}

	public virtual void Initialize(Plant plant)
	{
		_plant = plant;
	}

	public abstract void UpdateState();
}

[Serializable]
public class Growth : PlantState
{
	public Growth(Plant plant) : base(plant)
	{
		InitializeGrowth(DateTime.Now, true);
		RecoverThirst();
		RecoverPest();
	}

	public override void Initialize(Plant plant)
	{
		base.Initialize(plant);
		RecalculateTime();
	}

	public void InitializeFromState(Plant plant)
	{
		base.Initialize(plant);
	}

	public override void UpdateState()
	{
		DateTime now = DateTime.Now;
		if (_plant.Data.PestTime.CompareTo(now) <= 0)
		{
			if (ChanceOfPest())
			{
				_plant.SetState(new Pest(_plant, this));
				return;
			}
			RecoverPest();
		}

		if (_plant.Data.ThirstTime.CompareTo(now) <= 0)
		{
			_plant.SetState(new Thirst(_plant, this));
			return;
		}

		if (_plant.Data.GrowthTime.CompareTo(now) <= 0)
		{
			InitializeGrowth(now, true);
		}
	}

	public void RecoverThirst()
	{
		_plant.Data.ThirstTime = DateTime.Now.AddSeconds(_plant.Item.ThirstTimeSpan.TotalSeconds);
	}

	public void RecoverPest()
	{
		_plant.Data.PestTime = DateTime.Now.AddSeconds(_plant.Item.PestTimeSpan.TotalSeconds);
	}

	public void RecoverGrowth(DateTime datum)
	{
		double freezeTime = DateTime.Now.Subtract(datum).TotalSeconds;
		_plant.Data.GrowthTime = _plant.Data.GrowthTime.AddSeconds(freezeTime);
		CheckFertilizer();
	}

	private bool ChanceOfPest()
	{
		return Random.Range(1, 101) <= _plant.Item.ChanceOfPest;
	}

	private void InitializeGrowth(DateTime datum, bool animated)
	{
		int nextStageIndex = _plant.Item.Stages.IndexOf(_plant.Stage) + 1;

		if (nextStageIndex <= _plant.Item.Stages.Count - 1)
		{
			_plant.SetStage(_plant.Item.Stages[nextStageIndex], animated);
		}

		int indexNextStage = nextStageIndex + 1;
		if (indexNextStage <= _plant.Item.Stages.Count - 1)
		{
			PlantStage nextStage = _plant.Item.Stages[indexNextStage];
			TimeSpan growthTime = new TimeSpan(nextStage.timeGrowth.Days, nextStage.timeGrowth.Hours, nextStage.timeGrowth.Minutes, nextStage.timeGrowth.Seconds);
			_plant.Data.GrowthTime = datum.AddSeconds(growthTime.TotalSeconds);
			CheckFertilizer();
		}
		else
		{
			_plant.SetState(new WaitHarvest(_plant));
		}
	}

	private void RecalculateTime()
	{
		DateTime now = DateTime.Now;
		DateTime thirstEndTime = _plant.Data.ThirstTime;
		DateTime pestEndTime = _plant.Data.PestTime;

		for (int i = _plant.Item.Stages.IndexOf(_plant.Stage); i < _plant.Item.Stages.Count - 1; i++)
		{
			DateTime growthEndTime = _plant.Data.GrowthTime;

			if (pestEndTime.CompareTo(now) <= 0 && pestEndTime.CompareTo(thirstEndTime) <= 0 &&
				pestEndTime.CompareTo(growthEndTime) <= 0)
			{
				if (ChanceOfPest())
				{
					_plant.SetState(new Pest(_plant, this));
					break;
				}

				RecoverPest();
				pestEndTime = _plant.Data.PestTime;
			}

			if (thirstEndTime.CompareTo(now) <= 0 && thirstEndTime.CompareTo(growthEndTime) <= 0)
			{
				_plant.SetState(new Thirst(_plant, this));
				break;
			}
			else if (growthEndTime.CompareTo(now) <= 0)
			{
				InitializeGrowth(growthEndTime, false);
			}
		}
	}

	private void CheckFertilizer()
	{
		if (_plant.TryGetDecorator(out Fertilized fertilized))
		{
			fertilized.RecalculateGrowth();
		}
	}
}

[Serializable]
public abstract class AwaitingPlantState : PlantState
{
	[SerializeField] protected readonly Growth lastStateGrowth;

	public AwaitingPlantState(Plant plant, Growth growth) : base(plant)
	{
		lastStateGrowth = growth;

		if (plant.TryGetDecorator(out Fertilized fertilized))
		{
			fertilized.CancelAccelerationGrowth();
		}
	}

	public override void Initialize(Plant plant)
	{
		base.Initialize(plant);
		lastStateGrowth.InitializeFromState(plant);
	}

	public override void UpdateState() { }

	public abstract void EndState();
}

[Serializable]
public class Pest : AwaitingPlantState
{
	public Pest(Plant plant, Growth growth) : base(plant, growth) { }

	public override void EndState()
	{
		lastStateGrowth.RecoverGrowth(_plant.Data.PestTime);
		lastStateGrowth.RecoverPest();
		_plant.SetState(lastStateGrowth);
	}
}

[Serializable]
public class Thirst : AwaitingPlantState
{
	public Thirst(Plant plant, Growth growth) : base(plant, growth) { }

	public override void EndState()
	{
		lastStateGrowth.RecoverGrowth(_plant.Data.ThirstTime);
		lastStateGrowth.RecoverThirst();
		_plant.SetState(lastStateGrowth);
	}
}

[Serializable]
public class WaitHarvest : PlantState
{
	public WaitHarvest(Plant plant) : base(plant) { }

	public override void UpdateState() { }
}

[Serializable]
public abstract class StateDecorator : PlantState
{
	[SerializeField] protected PlantState packedPlantState;

	public StateDecorator(Plant plant, PlantState _plantState) : base(plant)
	{
		SetState(_plantState);
	}

	public override void Initialize(Plant plant)
	{
		base.Initialize(plant);
		packedPlantState.Initialize(plant);
	}

	public override void UpdateState()
	{
		packedPlantState.UpdateState();
	}

	public bool TryGetPlantState<T>(out T state) where T : PlantState
	{
		if (packedPlantState is T)
		{
			state = packedPlantState as T;
			return true;
		}
		else if (packedPlantState is not StateDecorator decorator)
		{
			state = null;
			return false;
		}
		else
		{
			return decorator.TryGetPlantState(out state);
		}
	}

	public void SetState(PlantState _plantState)
	{
		packedPlantState = _plantState;
	}

	public PlantState GetState()
	{
		return packedPlantState;
	}

	public void PackState(PlantState _plantState)
	{
		if (packedPlantState is StateDecorator decorator)
			decorator.PackState(_plantState);
		else
			SetState(_plantState);
	}
}

[Serializable]
public class Fertilized : StateDecorator
{
	[SerializeField] private float accelerationGrowth = 1f;
	[SerializeField] private DateTime fertilizeTime;

	public Fertilized(Plant plant, PlantState _plantState, float accelerationGrowth) : base(plant, _plantState)
	{
		InitializeFertilized(accelerationGrowth);
	}

	public override void Initialize(Plant plant)
	{
		base.Initialize(plant);
		CheckFertilizeTime();
	}

	public override void UpdateState()
	{
		base.UpdateState();
		CheckFertilizeTime();
	}

	public void CancelAccelerationGrowth()
	{
		DateTime currentTime = DateTime.Now;
		double remainGrowthTime = _plant.Data.GrowthTime.Subtract(currentTime).TotalSeconds;
		_plant.Data.GrowthTime = currentTime.AddSeconds(remainGrowthTime * accelerationGrowth);
	}

	public void RecalculateGrowth()
	{
		RecalculateGrowth(fertilizeTime.Subtract(DateTime.Now).TotalSeconds);
	}

	public void ProlongFertilize(float accelerationGrowth)
	{
		DateTime previousFertilizeTime = fertilizeTime;
		fertilizeTime = DateTime.Now.AddSeconds(_plant.Item.FertilizeTimeSpan.TotalSeconds);
		double additiveFertilizeSeconds = fertilizeTime.Subtract(previousFertilizeTime).TotalSeconds;

		AccelerateGrowthTime(accelerationGrowth, additiveFertilizeSeconds);
	}

	private void InitializeFertilized(float accelerationGrowth)
	{
		_plant.Data.IsFertilized = true;
		fertilizeTime = DateTime.Now.AddSeconds(_plant.Item.FertilizeTimeSpan.TotalSeconds);

		AccelerateGrowthTime(accelerationGrowth, _plant.Item.FertilizeTimeSpan.TotalSeconds);
	}

	private void AccelerateGrowthTime(float acceleration, double fertilizedTime)
	{
		accelerationGrowth = Mathf.Max(accelerationGrowth, _plant.Item.FertilizerMultiplier * acceleration);
		RecalculateGrowth(fertilizedTime);
	}

	private void RecalculateGrowth(double fertilizedTime)
	{
		if (fertilizeTime.CompareTo(_plant.Data.GrowthTime) < 0)
		{
			double savedTime = fertilizedTime - (fertilizedTime * accelerationGrowth);
			_plant.Data.GrowthTime = _plant.Data.GrowthTime.AddSeconds(savedTime);
		}
		else
		{
			DateTime currentTime = DateTime.Now;
			double remainingTime = _plant.Data.GrowthTime.Subtract(currentTime).TotalSeconds;
			double newTime = remainingTime / accelerationGrowth;
			_plant.Data.GrowthTime = currentTime.AddSeconds(newTime);
		}
	}

	private void CheckFertilizeTime()
	{
		if (fertilizeTime.CompareTo(DateTime.Now) <= 0)
		{
			_plant.ResetDecorator(this);
		}
	}
}
