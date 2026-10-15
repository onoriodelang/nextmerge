using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class BloodScreenSystem : IEcsRunSystem
    {
        private EcsFilter<BloodScreenView> _filter;
        private EcsFilter<NextLevelState, StateEnter> _gameplayEnter;

        private EcsWorld _world;
        
        public void Run()
        {
            if(_gameplayEnter.IsEmpty())
                return;

            foreach (var i in _filter)
            {
                _filter.Get1(i).View.HideBlood();
            }
        }
    }

}