using Leopotam.Ecs;
using UnityEngine.AddressableAssets;

namespace Modules.LevelSpawner
{
    public class SpawnedLevelCleanupSystem : IEcsRunSystem
    {
        // auto injected
        readonly EcsFilter<LevelSpawnedSignal> _signal;
        readonly EcsFilter<LevelEntityTag, CleanEntityTag> _prevEntities;
        readonly EcsFilter<AddressableLevelAsset, CleanEntityTag> _prevLevel;

        public void Run() 
        {
            if (_signal.IsEmpty())
                return;

            // destroy prev level entities
            foreach (var i in _prevEntities)
            {
                _prevEntities.GetEntity(i).Destroy();
            }

            // release prev loaded asset
            foreach (var i in _prevLevel)
            {
                Addressables.Release(_prevLevel.Get1(i).Handle);
                _prevLevel.GetEntity(i).Destroy();
            }

            // release resouces and force garbage collector
            UnityEngine.Resources.UnloadUnusedAssets();
            UnityEngine.Scripting.GarbageCollector.CollectIncremental(100000000);
        }
    }
}
