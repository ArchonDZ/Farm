using UnityEngine;

public class PlantStateIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("StateSprites")]
    [SerializeField] private Sprite spriteThirst;
    [SerializeField] private Sprite spritePest;
    [SerializeField] private Sprite spriteHarvest;

    public void UpdateState(PlantState plantState)
    {
        spriteRenderer.sprite = plantState switch
        {
            Thirst => spriteThirst,
            Pest => spritePest,
            WaitHarvest => spriteHarvest,
            _ => null
        };
    }
}
