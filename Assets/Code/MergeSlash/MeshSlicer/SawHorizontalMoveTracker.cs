using Leopotam.Ecs;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using UnityEngine;

namespace Modules.MergeSlash.Trackers
{
    public class SawHorizontalMoveTracker : IEcsRunSystem
    {
        private EcsFilter<EventGroup.GamePlayState> _gameplay;
        private EcsFilter<SawView, SawThrownTag> _sawView;
        private EcsFilter<UserInput.OnScreenHold> _hold;

        public void Run()
        {
            if (_gameplay.IsEmpty())
                return;

            SetBulletHorizontalDirection();
        }

        private void SetBulletHorizontalDirection()
        {
            foreach (var holdIdx in _hold)
            {
                foreach (var sawView in _sawView)
                {
                    _sawView.GetEntity(sawView).Get<SawHorizontalMove>().XDisplacement = _hold.Get1(holdIdx).XDisplacement;
                }
            }
        }
    }
    
    public struct SawHorizontalMove
    {
        public float XDisplacement; 
    }
}