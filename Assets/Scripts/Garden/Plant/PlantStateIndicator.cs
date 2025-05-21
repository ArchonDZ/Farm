using UnityEngine;

public class PlantStateIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer stateSpriteRenderer;

    [Header("StateSprites")]
    [SerializeField] private Sprite spriteThirst;
    [SerializeField] private Sprite spritePest;
    [SerializeField] private Sprite spriteHarvest;

    [Header("Decorators")]
    [SerializeField] private ParticleSystem particleFertilizedState;

    public void UpdateState(PlantState plantState)
    {
        stateSpriteRenderer.sprite = plantState switch
        {
            Thirst => spriteThirst,
            Pest => spritePest,
            WaitHarvest => spriteHarvest,
            _ => null
        };
    }

    public void SetDecorator(StateDecorator stateDecorator)
    {
        switch (stateDecorator)
        {
            case Fertilized:
                particleFertilizedState.Play();
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
                particleFertilizedState.Stop();
                break;
            default:
                break;
        }
    }
}
