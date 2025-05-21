using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : CurtainPanel
{
    [SerializeField] private Button closeAreaButton;
    [SerializeField] private RectTransform rectTransformPanel;

    protected override void VirtualAwake()
    {
        closeAreaButton.onClick.AddListener(ChangePanel);
    }

    public override void ClosePanel()
    {
        closeAreaButton.gameObject.SetActive(false);
        rectTransformPanel.DOAnchorPosX(0, 0.5f);
        isOpened = false;
    }

    public override void OpenPanel()
    {
        closeAreaButton.gameObject.SetActive(true);
        rectTransformPanel.DOAnchorPosX(rectTransformPanel.rect.width, 0.5f);
        isOpened = true;
    }
}
