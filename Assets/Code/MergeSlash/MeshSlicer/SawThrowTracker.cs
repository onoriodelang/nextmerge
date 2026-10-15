using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.Utils;
using UnityEngine;

namespace Modules.MergeSlash.Trackers
{
    public class SawThrowTracker : IEcsRunSystem
    {
        private EcsFilter<SlashButtonSignal> _signal;
        private EcsFilter<EventGroup.GamePlayState> _gameplay;
        private EcsFilter< MergeUnit, OnTile>.Exclude<ThrowTag> _sawView;

        private EcsWorld _world;

        public void Run()
        {
            if (_signal.IsEmpty() || _gameplay.IsEmpty() || _sawView.IsEmpty())
                return;

            float remainigTime = 0.45f;
            int inHandNumber = 0;

            foreach (var i in _sawView)
            {
                if(_sawView.Get2(i).TileID > 2)
                    continue;
                
                var sawEntity = _sawView.GetEntity(i);
                sawEntity.Get<PlaceInHands>().Number = inHandNumber;
                inHandNumber++;
            }
            
            if(inHandNumber > 2 || _sawView.GetEntitiesCount() <= inHandNumber)
                return;

            int maxLevel = -1;
            int maxIdx = -1;

            foreach (var i in _sawView)
            {
                if (_sawView.GetEntity(i).Has<PlaceInHands>())
                    continue;

                if (_sawView.Get1(i).CurrentLevel > maxLevel)
                {
                    maxLevel = _sawView.Get1(i).CurrentLevel;
                    maxIdx = i;
                }
            }

            _sawView.GetEntity(maxIdx).Get<PlaceInHands>().Number = inHandNumber;
            inHandNumber++;
            
            if(inHandNumber > 2 || _sawView.GetEntitiesCount() <= inHandNumber)
                return;

            maxLevel = -1;
            maxIdx = -1;

            foreach (var i in _sawView)
            {
                if (_sawView.GetEntity(i).Has<PlaceInHands>())
                    continue;

                if (_sawView.Get1(i).CurrentLevel > maxLevel)
                {
                    maxLevel = _sawView.Get1(i).CurrentLevel;
                    maxIdx = i;
                }
            }

            _sawView.GetEntity(maxIdx).Get<PlaceInHands>().Number = inHandNumber;
            inHandNumber++;
        }
    }
}