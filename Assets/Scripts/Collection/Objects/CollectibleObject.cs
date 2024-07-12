using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(DragableUIFromCurtain))]
public class CollectibleObject : MonoBehaviour
{
    [SerializeField] private DragableUIFromCurtain dragable;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image iconImage;

    [Inject] private PlacementHelper placementHelper;

    private CollectiblePackage collectiblePackage;

    public CollectiblePackage CollectiblePackage => collectiblePackage;

    public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
    {
        collectiblePackage = package;

        iconImage.sprite = collectiblePackage.CollectibleItem.Icon;
        titleText.text = collectiblePackage.CollectibleItem.Name;
        UpdateObject();

        dragable.Initialize(curtainPanel);
        dragable.OnLeftCurtainEvent += Dragable_OnLeftCurtainEvent;
    }

    public void UpdateObject()
    {
        if (collectiblePackage.CollectibleData.Count > 0)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            countText.text = collectiblePackage.CollectibleData.Count.ToString();
        }
        else
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Spend()
    {
        if (collectiblePackage.CollectibleData.Count > 0)
        {
            collectiblePackage.CollectibleData.Count--;
            UpdateObject();

            if (collectiblePackage.CollectibleData.Count == 0)
            {
                placementHelper.Deactivate();
            }
        }
    }

    private void Dragable_OnLeftCurtainEvent()
    {
        placementHelper.ActivateForMouseUp(this, collectiblePackage.CollectibleItem.Icon);
    }
}
