using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : CurtainPanel
{
	[SerializeField] private Button _closeAreaButton;
	[SerializeField] private RectTransform _rectTransformPanel;

	protected override void VirtualAwake()
	{
		_closeAreaButton.onClick.AddListener(ChangePanel);
	}

	public override void ClosePanel()
	{
		_closeAreaButton.gameObject.SetActive(false);
		_rectTransformPanel.DOAnchorPosX(0, 0.5f);
		_isOpened = false;
	}

	public override void OpenPanel()
	{
		_closeAreaButton.gameObject.SetActive(true);
		_rectTransformPanel.DOAnchorPosX(_rectTransformPanel.rect.width, 0.5f);
		_isOpened = true;
	}
}
