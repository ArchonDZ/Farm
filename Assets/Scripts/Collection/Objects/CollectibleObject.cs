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

    [Inject] private GridSystem gridSystem;
    [Inject] private PlacementHelper placementHelper;

    private CollectiblePackage collectiblePackage;

    public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
    {
        collectiblePackage = package;

        iconImage.sprite = collectiblePackage.CollectibleItem.Icon;
        titleText.text = collectiblePackage.CollectibleItem.Name;
        UpdateObject();

        dragable.Initialize(curtainPanel);
        dragable.OnLeftCurtainEvent += Dragable_OnLeftCurtainEvent;
        placementHelper.OnCanBePlacedEvent += Spend;
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

    private void Dragable_OnLeftCurtainEvent()
    {
        placementHelper.Activate(collectiblePackage.CollectibleItem.Icon);
    }

    private void Spend()
    {
        collectiblePackage.CollectibleData.Count--;
        UpdateObject();
        gridSystem.InitializeObjectOnCellPosition(collectiblePackage.CollectibleItem.InitializableItem.InitializableObject, Camera.main.ScreenToWorldPoint(Input.mousePosition))
            .Initialize(collectiblePackage.CollectibleItem.InitializableItem, null);
    }
}
