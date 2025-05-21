using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DragTool : Tool, IDragHandler
{
    [SerializeField, Min(1)] private int intervalDrag = 5;

    public void OnDrag(PointerEventData eventData)
    {
        if (Time.frameCount % intervalDrag == 0)
        {
            RaycastTool();
        }
    }
}
