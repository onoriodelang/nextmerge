using Leopotam.Ecs;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.ST.EntityTemplates
{
    public class LevelDataTemplate : ViewElement
    {
        public int[] WeaponsForBuy;
        public int AllTargetsHealth;
        public int AllTargetsMoney;
        public float HorizontalMaxChange = 10f;
        public int[] WeaponsBuySequence;
        
        public override void OnSpawn(EcsEntity entity, EcsWorld world)
        {
            base.OnSpawn(entity, world);
            entity.Get<LevelSpawner.LevelEntityTag>();
            ref var data = ref entity.Get<LevelData>();
            data.WeaponsForBuy = WeaponsForBuy;
            data.AllTargetsHealth = AllTargetsHealth;
            data.AllTargetsGold = AllTargetsMoney;
            data.HorizontalMaxChange = HorizontalMaxChange;
            data.WeaponsBuySequence = WeaponsBuySequence;
            data.WeaponsBuySequenceIdx = 0;
        }
    }

    public struct LevelData
    {
        public int[] WeaponsForBuy;
        public int AllTargetsHealth;
        public int AllTargetsGold;
        public float HorizontalMaxChange;
        public int[] WeaponsBuySequence;
        public int WeaponsBuySequenceIdx;
        public bool Inited;
    }
}