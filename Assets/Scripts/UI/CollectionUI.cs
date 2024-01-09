using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMeshProUGUI;
    [SerializeField] private Image image;

    [Inject] private CollectionSystem collectionSystem;

    void Start()
    {
        image.sprite = collectionSystem.SeedList[0].Icon;
        m_TextMeshProUGUI.text = collectionSystem.SeedList[0].Name;
    }
}
