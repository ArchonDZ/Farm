using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
	[SerializeField] private GridSystem _gridSystem;
	[SerializeField] private SaveSystem _saveSystem;
	[SerializeField] private CollectionSystem _collectionSystem;
	[SerializeField] private PlacementHelper _placementHelper;
	[SerializeField] private Camera _gameCamera;

	public override void InstallBindings()
	{
		Container.Bind<GridSystem>().FromInstance(_gridSystem).AsSingle().NonLazy();
		Container.Bind<SaveSystem>().FromInstance(_saveSystem).AsSingle().NonLazy();
		Container.Bind<CollectionSystem>().FromInstance(_collectionSystem).AsSingle().NonLazy();
		Container.Bind<PlacementHelper>().FromInstance(_placementHelper).AsSingle().NonLazy();
		Container.Bind<Camera>().FromInstance(_gameCamera).AsSingle().NonLazy();
	}
}
