using Zenject;

public class LevelInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<LevelGameplayManager>()
            .AsSingle()
            .NonLazy();
    }
}