using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private GameArea gameArea;

    public override void InstallBindings()
    {
        Container.Bind<GridSystem>().FromInstance(gridSystem).AsSingle().NonLazy();
        Container.Bind<GameArea>().FromInstance(gameArea).AsSingle().NonLazy();
    }
}