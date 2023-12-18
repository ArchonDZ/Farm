using UnityEngine;

public enum TileState
{
    Default,
    Free,
    Busy
}

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite preparedSprite;

    public TileState State { get; private set; }

    public void ActionInvoke()
    {
        switch (State)
        {
            case TileState.Default:
                State = TileState.Free;
                spriteRenderer.sprite = preparedSprite;
                break;
            case TileState.Free:
                break;
            case TileState.Busy:
                break;
        }
    }
}
