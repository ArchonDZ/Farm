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
        countText.text = collectiblePackage.CollectibleData.Count.ToString();

        dragable.Initialize(curtainPanel);
        dragable.OnLeftCurtainEvent += Dragable_OnLeftCurtainEvent;
        placementHelper.OnCanBePlacedEvent += Spend;
    }

    private void Dragable_OnLeftCurtainEvent()
    {
        placementHelper.Activate(collectiblePackage.CollectibleItem.Icon);
    }

    private void Spend()
    {
        collectiblePackage.CollectibleData.Count--;
        countText.text = collectiblePackage.CollectibleData.Count.ToString();
        gridSystem.InitiazeObjectOnPosition(collectiblePackage.CollectibleItem.Prefab, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
