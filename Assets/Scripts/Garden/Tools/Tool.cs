using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tool : MonoBehaviour, IDropHandler
{
    [SerializeField] private int layerMaskPlant;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Vector2 dragPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(dragPos, Vector2.zero, float.PositiveInfinity, 1 << layerMaskPlant);
        if (hit.transform != null)
        {
            if (hit.transform.TryGetComponent(out Plant plant))
            {
                Apply(plant);
            }
        }
    }

    protected abstract void Apply(Plant plant);
}
