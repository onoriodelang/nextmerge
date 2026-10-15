using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.Trackers;
using Modules.ST.EntityTemplates;
using Modules.UserInput;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class SawHorizontalMover : IEcsRunSystem
    {
        private EcsFilter<SawView, SawHorizontalMove> _sawView;
        private EcsFilter<LevelData> _levelData;
        private EcsFilter<EventGroup.GamePlayState> _gameplay;

        private Utils.TimeService _timeService;

        public void Run()
        {
            if(_gameplay.IsEmpty() || _sawView.IsEmpty())
                return;
            
            foreach (var sawView in _sawView)
            {
                ref var saw = ref _sawView.Get1(sawView);
                var position = saw.Transform.position;
                var displacement = _sawView.Get2(sawView).XDisplacement;
                var border =_levelData.IsEmpty()? saw.HorizontalMaxChange : _levelData.Get1(0).HorizontalMaxChange;
                position.x = position.x + saw.HorizontalSpeed * displacement;

                if (Mathf.Abs(position.x) > border)
                    return;

                saw.Transform.eulerAngles += new Vector3(0f, 0f,
                    (saw.HorizontalRotationAngle / saw.HorizontalMaxChange) * displacement * 100 * _timeService.DeltaTime);
                saw.Transform.position = position;
            }
        }
    }
}