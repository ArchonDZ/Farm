using R3;
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

    public CollectibleItem CollectibleItem => collectiblePackage.CollectibleItem;

    public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
    {
        collectiblePackage = package;
        iconImage.sprite = collectiblePackage.CollectibleItem.Icon;
        titleText.text = collectiblePackage.CollectibleItem.Name;
        collectiblePackage.CollectibleData.Count.Subscribe(UpdateObject).AddTo(this);

        dragable.Initialize(curtainPanel);
        dragable.OnLeftCurtainEvent += Dragable_OnLeftCurtainEvent;
    }

    public void Spend()
    {
        if (collectiblePackage.CollectibleData.TrySpend(1))
        {
            if (collectiblePackage.CollectibleData.Count.CurrentValue == 0)
            {
                placementHelper.Deactivate();
            }
        }
    }

    private void UpdateObject(int count)
    {
        if (count > 0)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            countText.text = count.ToString();
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
        placementHelper.ActivateForMouseUp(this, collectiblePackage.CollectibleItem.Icon);
    }
}
