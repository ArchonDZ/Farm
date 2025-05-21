using UnityEngine;
using UnityEngine.UI;

public abstract class CurtainPanel : MonoBehaviour
{
    [SerializeField] private Button buttonOpen;

    protected bool isOpened;

    void Awake()
    {
        buttonOpen.onClick.AddListener(ChangePanel);
        VirtualAwake();
    }

    protected void ChangePanel()
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

    protected virtual void VirtualAwake() { }

    public abstract void ClosePanel();
    public abstract void OpenPanel();
}
