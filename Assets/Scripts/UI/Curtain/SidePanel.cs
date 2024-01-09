using DG.Tweening;

public class SidePanel : CurtainPanel
{
    public override void ClosePanel()
    {
        rectTransformPanel.DOAnchorPosX(0, 0.5f);
        isOpened = false;
    }

    protected override void OpenPanel()
    {
        rectTransformPanel.DOAnchorPosX(rectTransformPanel.rect.width, 0.5f);
        isOpened = true;
    }
}
