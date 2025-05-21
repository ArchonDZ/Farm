using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private CollectionSystem collectionSystem;
    [SerializeField] private PlacementHelper placementHelper;
    [SerializeField] private Camera gameCamera;

    public override void InstallBindings()
    {
        Container.Bind<GridSystem>().FromInstance(gridSystem).AsSingle().NonLazy();
        Container.Bind<SaveSystem>().FromInstance(saveSystem).AsSingle().NonLazy();
        Container.Bind<CollectionSystem>().FromInstance(collectionSystem).AsSingle().NonLazy();
        Container.Bind<PlacementHelper>().FromInstance(placementHelper).AsSingle().NonLazy();
        Container.Bind<Camera>().FromInstance(gameCamera).AsSingle().NonLazy();
    }
}