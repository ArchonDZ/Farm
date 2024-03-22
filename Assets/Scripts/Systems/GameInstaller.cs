using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameArea gameArea;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private CollectionSystem collectionSystem;
    [SerializeField] private PlacementHelper placementHelper;

    public override void InstallBindings()
    {
        Container.Bind<GameArea>().FromInstance(gameArea).AsSingle().NonLazy();
        Container.Bind<GridSystem>().FromInstance(gridSystem).AsSingle().NonLazy();
        Container.Bind<CollectionSystem>().FromInstance(collectionSystem).AsSingle().NonLazy();
        Container.Bind<PlacementHelper>().FromInstance(placementHelper).AsSingle().NonLazy();
    }
}