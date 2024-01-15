using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(SidePanel))]
public class CollectionList : MonoBehaviour
{
    [SerializeField] private SidePanel sidePanel;
    [SerializeField] private CollectibleObject prefabCollectibleObject;
    [SerializeField] private Transform parent;

    [Inject] private DiContainer diContainer;

    public void Load(List<CollectiblePackage> packages)
    {
        foreach (CollectiblePackage package in packages)
        {
            diContainer.InstantiatePrefabForComponent<CollectibleObject>(prefabCollectibleObject, parent).Initialize(package, sidePanel);
        }
    }
}
