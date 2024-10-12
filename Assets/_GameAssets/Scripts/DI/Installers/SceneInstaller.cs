using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        //Controller Bindings
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<StateController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CatController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<HealthManager>().FromComponentInHierarchy().AsSingle();

        //UI Bindings
        Container.Bind<PlayerStateUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EggCounterUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHealthUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WinLoseUI>().FromComponentInHierarchy().AsSingle();
    }
}
