using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Tile tilePrefab;

    private Tile[,] gridArray;

    void Awake()
    {
        Initialize();
    }

    public Tile GetTile(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return gridArray[x, y];
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }

    private void Initialize()
    {
        gridArray = new Tile[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                gridArray[x, y] = Instantiate(tilePrefab, GetWorldPosition(x, y) + GetOffset(), Quaternion.identity);
    }

    private Vector3 GetWorldPosition(int x, int y) => new Vector3(x, y) * cellSize;

    private Vector3 GetOffset() => .5f * cellSize * new Vector3(1, 1);
}
