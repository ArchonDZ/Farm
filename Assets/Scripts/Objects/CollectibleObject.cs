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

    private CollectiblePackage collectiblePackage;

    public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
    {
        collectiblePackage = package;

        iconImage.sprite = collectiblePackage.CollectibleItem.Icon;
        titleText.text = collectiblePackage.CollectibleItem.Name;
        countText.text = collectiblePackage.CollectibleData.Count.ToString();

        dragable.Initialize(curtainPanel);
        dragable.OnEndDragEvent += Dragable_OnEndDragEvent;
    }

    public void Spend()
    {
        collectiblePackage.CollectibleData.Count--;
        countText.text = collectiblePackage.CollectibleData.Count.ToString();
    }

    private void Dragable_OnEndDragEvent()
    {
        gridSystem.InitiazeObjectOnPosition(collectiblePackage.CollectibleItem.Prefab, Camera.main.ScreenToWorldPoint(Input.mousePosition))
            .Initialize(this, collectiblePackage.CollectibleItem.Icon);
    }
}
