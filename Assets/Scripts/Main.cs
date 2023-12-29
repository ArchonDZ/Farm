using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private GridSystem grid;
    [SerializeField] private Seed seed;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 positionClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //if (grid.TryGetTile(positionClick, out Tile tile))
            //{
            //    switch (tile.State)
            //    {
            //        case TileState.Default:
            //            tile.ActionInvoke();
            //            break;
            //        case TileState.Free:
            //            seed.Plant(tile.transform.position);
            //            break;
            //        case TileState.Busy:
            //            break;
            //    }
            //}
        }
    }
}
