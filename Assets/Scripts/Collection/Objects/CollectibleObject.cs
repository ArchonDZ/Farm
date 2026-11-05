using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(DragableUIFromCurtain))]
public class CollectibleObject : MonoBehaviour
{
	[SerializeField] private DragableUIFromCurtain _dragable;
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private TextMeshProUGUI _countText;
	[SerializeField] private Image _iconImage;

	[Inject] private readonly PlacementHelper _placementHelper;

	private CollectiblePackage _collectiblePackage;

	public CollectibleItem CollectibleItem => _collectiblePackage.CollectibleItem;

	public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
	{
		_collectiblePackage = package;
		_iconImage.sprite = _collectiblePackage.CollectibleItem.Icon;
		_titleText.text = _collectiblePackage.CollectibleItem.Name;
		_collectiblePackage.CollectibleData.Count.Subscribe(UpdateObject).AddTo(this);

		_dragable.Initialize(curtainPanel);
		_dragable.OnLeftCurtainEvent += Dragable_OnLeftCurtainEvent;
	}

	public void Spend()
	{
		if (_collectiblePackage.CollectibleData.TrySpend(1))
		{
			if (_collectiblePackage.CollectibleData.Count.CurrentValue == 0)
			{
				_placementHelper.Deactivate();
			}
		}
	}

	void OnDestroy()
	{
		_dragable.OnLeftCurtainEvent -= Dragable_OnLeftCurtainEvent;
	}

	private void UpdateObject(int count)
	{
		if (count > 0)
		{
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}

			_countText.text = count.ToString();
		}
		else
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void Dragable_OnLeftCurtainEvent()
	{
		_placementHelper.ActivateCollectibleForEveryFrame(this, _collectiblePackage.CollectibleItem.Icon);
	}
}
