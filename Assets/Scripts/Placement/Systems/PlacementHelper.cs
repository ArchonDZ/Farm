using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlaceableObject))]
public class PlacementHelper : MonoBehaviour
{
    [SerializeField, Min(1)] private int intervalCheck = 5;
    [SerializeField] private int layerMaskInteractionPlace = 7;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlaceableObject placeableObject;
    [SerializeField] private PlacementPreview placementPreview;

    [Inject] private Camera mainCamera;

    private Action result;
    private Action<Action> action;
    private Func<bool> canPlace;
    private CollectibleObject collectibleObject;
    private PlaceableObject replaceableObject;

    public bool CanBePlacedOnTile => canPlace?.Invoke() ?? false;

    void Update()
    {
        UpdatePosition();
        action?.Invoke(result);
    }

    public void ActivateCollectibleForEveryFrame(CollectibleObject collectibleObject, Sprite sprite)
    {
        action = EveryFrameInterval;
        ActivateCollectible(collectibleObject, sprite);
    }

    public void ActivateCollectibleForMouseUp(CollectibleObject collectibleObject, Sprite sprite)
    {
        action = MouseUp;
        ActivateCollectible(collectibleObject, sprite);
    }

    public void ActivateReplacePlaceable(PlaceableObject placeableObject)
    {
        if (!placeableObject.gameObject.TryGetComponent(out SpriteRenderer spriteRenderer)) return;

        replaceableObject = placeableObject;
        replaceableObject.gameObject.SetActive(false);
        replaceableObject.Clear();

        action = MouseUp;
        canPlace = replaceableObject.CanBePlaced;
        result = InteractionPlaceableObject;
        Activate(spriteRenderer.sprite);
        EnablePreview();
    }

    public void FinishReplacePlaceable()
    {
        replaceableObject.Place();
        replaceableObject.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        action = null;
        canPlace = null;
        result = null;
        gameObject.SetActive(false);
    }

    private void ActivateCollectible(CollectibleObject collectibleObject, Sprite sprite)
    {
        this.collectibleObject = collectibleObject;
        canPlace = placeableObject.CanBePlaced;
        result = InteractionCollectible;
        Activate(sprite);
    }

    private void Activate(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        gameObject.SetActive(true);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    private void EveryFrameInterval(Action action)
    {
        if (Input.GetMouseButtonUp(0))
        {
            Deactivate();
            return;
        }

        if (Time.frameCount % intervalCheck == 0)
        {
            action?.Invoke();
        }
    }

    private void MouseUp(Action action)
    {
        if (Input.GetMouseButtonUp(0))
        {
            action?.Invoke();
            Deactivate();
        }
    }

    private void InteractionCollectible()
    {
        if (TryRaycast(out IInteractionPlace interactionPlace))
        {
            interactionPlace.Interaction(this, collectibleObject);
        }
    }

    private void InteractionPlaceableObject()
    {
        if (TryRaycast(out IInteractionPlace interactionPlace))
        {
            interactionPlace.Interaction(this, replaceableObject);
        }
        else
        {
            FinishReplacePlaceable();
        }
        DisablePreview();
    }

    private bool TryRaycast(out IInteractionPlace interactionPlace)
    {
        Vector2 dragPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(dragPos, Vector2.zero, float.PositiveInfinity, 1 << layerMaskInteractionPlace);
        if (hit.transform != null)
        {
            return hit.transform.TryGetComponent(out interactionPlace);
        }
        interactionPlace = null;
        return false;
    }

    private void EnablePreview()
    {
        if (placementPreview != null && replaceableObject != null)
            placementPreview.Enable(replaceableObject);
    }

    private void DisablePreview()
    {
        if (placementPreview != null)
            placementPreview.Disable();
    }
}
