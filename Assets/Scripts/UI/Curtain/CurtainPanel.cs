using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class CurtainPanel : MonoBehaviour
{
    [SerializeField] private Button buttonOpen;
    [SerializeField] protected RectTransform rectTransformPanel;

    [Inject] private readonly GameArea gameArea;

    protected bool isOpened;

    void Awake()
    {
        buttonOpen.onClick.AddListener(ChangePanel);
        gameArea.OnPointerClickEvent += ClosePanel;
    }

    public abstract void ClosePanel();

    protected abstract void OpenPanel();

    private void ChangePanel()
    {
        if (isOpened)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }
}
