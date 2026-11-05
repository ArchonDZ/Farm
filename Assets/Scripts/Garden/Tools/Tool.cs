using UnityEngine;
using Zenject;

public abstract class Tool : MonoBehaviour
{
	[SerializeField] private int _layerMaskPlant = 6;

	[Inject] private readonly Camera _mainCamera;

	protected void RaycastTool()
	{
		Vector2 dragPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(dragPos, Vector2.zero, float.PositiveInfinity, 1 << _layerMaskPlant);
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
