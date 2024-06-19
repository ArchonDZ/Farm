using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    [SerializeField] private int layerMaskPlant = 6;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    protected void RaycastTool()
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
