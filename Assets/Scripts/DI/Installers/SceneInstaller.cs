using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<StateController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerStateUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EggCounterUI>().FromComponentInHierarchy().AsSingle();

    }
}
