using System;
using UnityEngine;

[RequireComponent(typeof(PlaceableObject))]
public class PlacementHelper : MonoBehaviour
{
    public event Action OnCanBePlacedEvent;

    [SerializeField, Min(1)] private int intervalCheck = 5;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlaceableObject placeableObject;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Deactivate();
            return;
        }

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        if (Time.frameCount % intervalCheck == 0)
        {
            if (placeableObject.CanBePlaced())
            {
                OnCanBePlacedEvent?.Invoke();
            }
        }
    }

    public void Activate(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        OnCanBePlacedEvent = null;
        gameObject.SetActive(false);
    }
}
