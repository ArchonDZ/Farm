using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FertilizerTool : DropTool
{
	[SerializeField] private FertilizerCommonCollectibleItem _collectibleItem;
	[SerializeField] private TMP_Text _countText;
	[SerializeField] private Image _image;

	[Inject] private readonly CollectionSystem _collectionSystem;

	private CollectibleData _data;

	void Start()
	{
		_image.sprite = _collectibleItem.Icon;
		_collectionSystem.AddDrop(new Drop(0, _collectibleItem));
		if (_collectionSystem.TryGetCollectibleData(_collectibleItem, out CollectibleData data))
		{
			_data = data;
			_data.Count.Subscribe(CheckRemainCount).AddTo(this);
		}
	}

	private void CheckRemainCount(int count)
	{
		_countText.text = count.ToString();
		gameObject.SetActive(0 < count);
	}

	protected override void Apply(Plant plant)
	{
		if (_data.TrySpend(1))
			plant.Fertilize(_collectibleItem.AccelerationGrowth);
	}
}
