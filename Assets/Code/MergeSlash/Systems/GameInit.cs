using Leopotam.Ecs;

namespace Modules.MergeSlash
{
    public class GameInit : IEcsInitSystem
    {
        readonly EcsWorld _world;

        public void Init()
        {
            // load last saved level info
            PlayerLevel.ProgressionInfo.Load();
            
            // start next level
            EventGroup.StateFactory.CreateState<EventGroup.NextLevelState>(_world);
        }
    }
}
