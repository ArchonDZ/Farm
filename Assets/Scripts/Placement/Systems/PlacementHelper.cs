using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlaceableObject))]
public class PlacementHelper : MonoBehaviour
{
    public event Action OnCanBePlacedEvent;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlaceableObject placeableObject;

    [Inject] private GridSystem gridSystem;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
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
            if (placeableObject.CanBePlaced())
            {
                OnCanBePlacedEvent?.Invoke();
            }
            OnCanBePlacedEvent = null;
            gameObject.SetActive(false);
        }
    }

    public void Activate(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(true);
    }
}
