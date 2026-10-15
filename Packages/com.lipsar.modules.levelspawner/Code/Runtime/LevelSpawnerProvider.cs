using UnityEngine;
using Leopotam.Ecs;
using Modules.Root.ECS;

namespace Modules.LevelSpawner
{
    public class LevelSpawnerProvider : MonoBehaviour, ISystemsProvider
    {
        public enum LevelSpawnerMode 
        {
            addresable, 
            sceneAdditive
        }
        
        [SerializeField] private LevelSpawnerMode _spawnerMode;
        [SerializeField] private AdditiveSceneMode.AdditiveSceneModeSpawnConfig _additiveSceneSpawnerConfig;
        [SerializeField] private LevelsCollection _levels;
        [SerializeField] private LevelLoadingView _levelLoadingView;

        public EcsSystems GetSystems(EcsWorld world, EcsSystems endFrame, EcsSystems ecsSystems)
        {
            EcsSystems systems = new EcsSystems(world, "LevelSpawner");
            InitSpawnerConfigByMode(_spawnerMode);
            systems
                .OneFrame<LevelSpawnedSignal>()
                .Add(GetSystemsByMode(_spawnerMode))
                .OneFrame<SpawnLevelSignal>()
                .Add(new SpawnedLevelCleanupSystem())
                ;

            return systems;
        }
        
        public IEcsRunSystem GetSystemsByMode(LevelSpawnerMode mode) 
        {
            switch (mode)
            {
                case LevelSpawnerMode.addresable:
                    return new AddressableLevelSpawner(_levels, _levelLoadingView);
                case LevelSpawnerMode.sceneAdditive:
                    return new AdditiveSceneMode.AdditiveSceneLoadSpawner(_additiveSceneSpawnerConfig, _levelLoadingView);
                default:
                    return new AddressableLevelSpawner(_levels, _levelLoadingView);
            }
        }

        public void InitSpawnerConfigByMode(LevelSpawnerMode mode)
        {
            switch (mode)
            {
                case LevelSpawnerMode.addresable:
                    _levels.Init();
                    break;
                case LevelSpawnerMode.sceneAdditive:
                    _additiveSceneSpawnerConfig.Init();
                    break;
                default:
                    _levels.Init();
                    _additiveSceneSpawnerConfig.Init();
                    break;
            }
        }
    }
}
