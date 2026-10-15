using UnityEngine;
using Leopotam.Ecs;
using Modules.EventGroup;

namespace Modules.MergeSlash.EventHandlers
{
    /// <summary>
    /// process restart of a level
    /// </summary>
    public class OnRestartRoundEnter : IEcsRunSystem
    {
        readonly EcsFilter<RestartRoundState, StateEnter> _signal;
        readonly EcsWorld _world;

        public void Run()
        {
            if (_signal.IsEmpty())
                return;

            // in general enough to just trigger next level state
            EventGroup.StateFactory.CreateState<NextLevelState>(_world);
            
            foreach (var i in _signal)
            {
                // restart is quite tricky process
                // if there are systems with EcsFilter<NextLevelState, EventGroup.StateEnter> before OnRestartRoundEnter 
                // OnRestartRoundEnter system should be moved before them
                // in case of problems with restart processing, try to just reload current scene

                // если есть системы, обрабатывающие EcsFilter<NextLevelState, EventGroup.StateEnter> в основной логике
                // то система рестарта должна идти перед ними для корректной отработки события входа в NextLevelState у этих систем
                // если будут проблемы с обработкой рестарта, то стоит попробовать просто перезагружать сцену

                // force end restart state 
                _signal.GetEntity(i).Destroy();
            }
        }
    }
}