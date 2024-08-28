using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<StateController>().FromComponentInHierarchy().AsSingle();
    }
}
