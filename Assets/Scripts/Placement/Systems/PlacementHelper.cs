using System;
using UnityEngine;

[RequireComponent(typeof(PlaceableObject))]
public class PlacementHelper : MonoBehaviour
{
    [SerializeField, Min(1)] private int intervalCheck = 5;
    [SerializeField] private int layerMaskInteractionPlace = 7;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlaceableObject placeableObject;

    private CollectibleObject collectibleObject;
    private Camera mainCamera;
    private Action action;

    public bool CanBePlacedOnTile => placeableObject.CanBePlaced();

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        action.Invoke();
    }

    public void ActivateForEveryFrame(CollectibleObject collectibleObject, Sprite sprite)
    {
        action = EveryFrameInterval;
        Activate(collectibleObject, sprite);
    }

    public void ActivateForMouseUp(CollectibleObject collectibleObject, Sprite sprite)
    {
        action = MouseUp;
        Activate(collectibleObject, sprite);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void Activate(CollectibleObject collectibleObject, Sprite sprite)
    {
        this.collectibleObject = collectibleObject;
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(true);
    }

    private void EveryFrameInterval()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Deactivate();
            return;
        }

        if (Time.frameCount % intervalCheck == 0)
        {
            Raycast();
        }
    }

    private void MouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Raycast();
            Deactivate();
        }
    }

    private void Raycast()
    {
        Vector2 dragPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(dragPos, Vector2.zero, float.PositiveInfinity, 1 << layerMaskInteractionPlace);
        if (hit.transform != null)
        {
            if (hit.transform.TryGetComponent(out IInteractionPlace interactionPlace))
            {
                interactionPlace.Interaction(this, collectibleObject);
            }
        }
    }
}
