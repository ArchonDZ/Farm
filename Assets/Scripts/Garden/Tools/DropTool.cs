using UnityEngine.EventSystems;

public abstract class DropTool : Tool, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        RaycastTool();
    }
}
