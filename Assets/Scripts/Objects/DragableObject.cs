using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlacebleObject))]
public class DragableObject : MonoBehaviour
{
    [Inject] private GridSystem gridSystem;

    private PlacebleObject placebleObject;
    private Camera mainCamera;
    private Vector3 offset;

    void Awake()
    {
        placebleObject = GetComponent<PlacebleObject>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        offset = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
    }

    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3Int cellPos = gridSystem.GridLayout.WorldToCell(mousePos);
        transform.position = gridSystem.GridLayout.CellToLocalInterpolated(cellPos) + Vector3.up * gridSystem.GridLayout.cellSize.y / 2f;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            placebleObject.CheckPlacement();
            enabled = false;
        }
    }
}
