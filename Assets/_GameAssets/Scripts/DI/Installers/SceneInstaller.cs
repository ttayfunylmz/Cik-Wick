using Zenject;

public class SceneInstaller : MonoInstaller<SceneInstaller>
{
    public override void InstallBindings()
    {
        //Controller Bindings
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerInteractionController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<StateController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CatController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<HealthManager>().FromComponentInHierarchy().AsSingle();

        //Helper Bindings
        Container.Bind<CameraShake>().FromComponentInHierarchy().AsSingle();

        //UI Bindings
        Container.Bind<PlayerStateUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<EggCounterUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerHealthUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WinLoseUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<TimerUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ChatBubbleUI>().FromComponentInHierarchy().AsSingle();
    }
}
