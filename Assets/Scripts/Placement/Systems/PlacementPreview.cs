using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public class PlacementPreview : MonoBehaviour
{
	[SerializeField] private Tilemap _highlight;
	[SerializeField] private TileBase _highlightTile;

	[Inject] private readonly GridSystem _gridSystem;

	private PlaceableObject _replaceableObject;

	void Awake()
	{
		enabled = false;
	}

	public void Enable(PlaceableObject placeableObject)
	{
		_replaceableObject = placeableObject;
		enabled = true;
	}

	public void Disable()
	{
		enabled = false;
	}

	private void Update()
	{
		if (_replaceableObject == null) return;

		ClearPreview();

		BoundsInt areaTemp = _replaceableObject.Area;
		areaTemp.position = _gridSystem.GridLayout.WorldToCell(transform.position);
		bool canPlace = _gridSystem.CanTakeArea(areaTemp);

		Color color = canPlace
			? new Color(0.2f, 1f, 0.2f, 0.45f)
			: new Color(1f, 0.2f, 0.2f, 0.45f);

		for (int x = 0; x < areaTemp.size.x; x++)
		{
			for (int y = 0; y < areaTemp.size.y; y++)
			{
				Vector3Int cell = areaTemp.position + new Vector3Int(x, y, 0);

				_highlight.SetTile(cell, _highlightTile);
				_highlight.SetTileFlags(
					cell,
					_highlight.GetTileFlags(cell) & ~TileFlags.LockColor
				);
				_highlight.SetColor(cell, color);
			}
		}
	}

	private void ClearPreview()
	{
		_highlight.ClearAllTiles();
	}

	private void OnDisable()
	{
		ClearPreview();
	}
}
