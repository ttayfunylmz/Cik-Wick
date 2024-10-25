using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller<ProjectInstaller>
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private BackgroundMusic _backgroundMusic;

    public override void InstallBindings()
    {
        Container.Bind<AudioManager>().FromComponentInNewPrefab(_audioManager).AsSingle().NonLazy();
        Container.Bind<BackgroundMusic>().FromComponentInNewPrefab(_backgroundMusic).AsSingle().NonLazy();
    }
}
