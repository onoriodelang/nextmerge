using Leopotam.Ecs;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class GoldForSellSystem : IEcsRunSystem
    {
        private EcsFilter<MergeUnit, SellMergeUnitSignal, UnityView> _filter;

        private EcsWorld _world;
        
        public void Run()
        {
            foreach (var i in _filter)
            {
                EntityRef[] sawRefs = _filter.Get3(i).Transform.GetComponentsInChildren<EntityRef>();

                foreach (var sawRef in sawRefs)
                {
                    if (sawRef.Entity.Has<SawView>())
                    {
                        ref var sawView = ref sawRef.Entity.Get<SawView>();

                        int gold = (int)(sawView.Durability * sawView.GoldForHealthRatio);

                        _world.NewEntity().Get<AddGold>().Gold = gold;
                    }

                }
            }
        }
    }

}