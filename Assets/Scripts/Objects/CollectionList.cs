using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SidePanel))]
public class CollectionList : MonoBehaviour
{
    [SerializeField] private SidePanel sidePanel;
    [SerializeField] private CollectibleObject prefabCollectibleObject;
    [SerializeField] private Transform parent;

    public void Load(List<CollectiblePackage> packages)
    {
        foreach (CollectiblePackage package in packages)
        {
            Instantiate(prefabCollectibleObject, parent).Initialize(package, sidePanel);
        }
    }
}
