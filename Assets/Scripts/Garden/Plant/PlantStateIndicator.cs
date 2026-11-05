using UnityEngine;

public class PlantStateIndicator : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _stateSpriteRenderer;

	[Header("StateSprites")]
	[SerializeField] private Sprite _spriteThirst;
	[SerializeField] private Sprite _spritePest;
	[SerializeField] private Sprite _spriteHarvest;

	[Header("Decorators")]
	[SerializeField] private ParticleSystem _particleFertilizedState;

	public void UpdateState(PlantState plantState)
	{
		_stateSpriteRenderer.sprite = plantState switch
		{
			Thirst => _spriteThirst,
			Pest => _spritePest,
			WaitHarvest => _spriteHarvest,
			_ => null
		};
	}

	public void SetDecorator(StateDecorator stateDecorator)
	{
		switch (stateDecorator)
		{
			case Fertilized:
				_particleFertilizedState.Play();
				break;
			default:
				break;
		}
	}

	public void ResetDecorator(StateDecorator stateDecorator)
	{
		switch (stateDecorator)
		{
			case Fertilized:
				_particleFertilizedState.Stop();
				break;
			default:
				break;
		}
	}
}
