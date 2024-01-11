using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DragableUIFromCurtain))]
public class CollectibleObject : MonoBehaviour
{
    [SerializeField] private DragableUIFromCurtain dragable;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image iconImage;
    
    private CollectiblePackage collectiblePackage;

    public void Initialize(CollectiblePackage package, CurtainPanel curtainPanel)
    {
        collectiblePackage = package;

        iconImage.sprite = collectiblePackage.CollectibleItem.Icon;
        titleText.text = collectiblePackage.CollectibleItem.Name;
        countText.text = collectiblePackage.CollectibleData.Count.ToString();

        dragable.Initialize(curtainPanel);
    }
}
