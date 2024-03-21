using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private GridLayout gridLayout;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase tileBase;

    [Inject] private DiContainer diContainer;

    public GridLayout GridLayout => gridLayout;

    #region Building
    public GameObject InitializeObjectOnCellPosition(GameObject build, Vector3 position)
    {
        position.z = 0;
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        Vector3 pos = gridLayout.CellToLocalInterpolated(cellPos) + Vector3.up * gridLayout.cellSize.y / 2f;
        return InitializeObjectOnPosition(build, pos);
    }

    public GameObject InitializeObjectOnPosition(GameObject build, Vector3 position)
    {
        return diContainer.InstantiatePrefab(build, position, Quaternion.identity, null);
    }

    public T InitializeObjectOnCellPosition<T>(T build, Vector3 position) where T : Object
    {
        position.z = 0;
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        Vector3 pos = gridLayout.CellToLocalInterpolated(cellPos) + Vector3.up * gridLayout.cellSize.y / 2f;
        return InitializeObjectOnPosition(build, pos);
    }

    public T InitializeObjectOnPosition<T>(T build, Vector3 position) where T : Object
    {
        return diContainer.InstantiatePrefabForComponent<T>(build, position, Quaternion.identity, null);
    }
    #endregion

    #region Tilemap Management
    public bool CanTakeArea(BoundsInt area)
    {
        return GetTilesBlock(area, tilemap).All(x => x != tileBase);
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, tilemap, tileBase);
    }

    public void ClearArea(BoundsInt area)
    {
        SetTilesBlock(area, tilemap, null);
    }

    private TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] tilesBase = new TileBase[area.size.x * area.size.y];

        int counter = 0;
        foreach (Vector3Int posWithin in area.allPositionsWithin)
        {
            tilesBase[counter] = tilemap.GetTile(new Vector3Int(posWithin.x, posWithin.y, 0));
            counter++;
        }

        return tilesBase;
    }

    private void SetTilesBlock(BoundsInt area, Tilemap tilemap, TileBase tileBase)
    {
        TileBase[] tilesBase = new TileBase[area.size.x * area.size.y];
        FillTiles(tilesBase, tileBase);
        tilemap.SetTilesBlock(area, tilesBase);
    }

    private void FillTiles(TileBase[] tilesBase, TileBase tileBase)
    {
        for (int i = 0; i < tilesBase.Length; i++)
        {
            tilesBase[i] = tileBase;
        }
    }
    #endregion
}
