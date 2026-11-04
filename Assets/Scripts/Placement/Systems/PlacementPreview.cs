using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class PlacementPreview : MonoBehaviour
{
    [SerializeField] private Tilemap highlight;
    [SerializeField] private TileBase highlightTile;

    [Inject] private GridSystem gridSystem;

    private PlaceableObject replaceableObject;

    void Awake()
    {
        enabled = false;
    }

    public void Enable(PlaceableObject placeableObject)
    {
        replaceableObject = placeableObject;
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
    }

    private void Update()
    {
        if (replaceableObject == null) return;

        ClearPreview();

        BoundsInt areaTemp = replaceableObject.Area;
        areaTemp.position = gridSystem.GridLayout.WorldToCell(transform.position);
        bool canPlace = gridSystem.CanTakeArea(areaTemp);

        Color color = canPlace
            ? new Color(0.2f, 1f, 0.2f, 0.45f)
            : new Color(1f, 0.2f, 0.2f, 0.45f);

        for (int x = 0; x < areaTemp.size.x; x++)
        {
            for (int y = 0; y < areaTemp.size.y; y++)
            {
                Vector3Int cell = areaTemp.position + new Vector3Int(x, y, 0);

                highlight.SetTile(cell, highlightTile);
                highlight.SetTileFlags(
                    cell,
                    highlight.GetTileFlags(cell) & ~TileFlags.LockColor
                );
                highlight.SetColor(cell, color);
            }
        }
    }

    private void ClearPreview()
    {
        highlight.ClearAllTiles();
    }

    private void OnDisable()
    {
        ClearPreview();
    }
}