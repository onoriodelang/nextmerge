using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.ST;
using Modules.ST.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class CompleteTracker : IEcsRunSystem
    {
        private EcsFilter<TargetWithHealth, Enemy> _enemies;
        private EcsFilter<Player> _player;
        private EcsFilter<GamePlayState> _gameplay;
        

        private EcsWorld _world;
        private TimeService _timeService;
        
        private PrototypeConfig _prototypeConfig;

        private float _timer;
        private bool _winAnimActive;
        private static readonly int VictoryKeyHash = Animator.StringToHash("Victory");

        public CompleteTracker(PrototypeConfig prototypeConfig)
        {
            _prototypeConfig = prototypeConfig;
        }

        public void Run()
        {
            if (!_gameplay.IsEmpty() && _enemies.IsEmpty())
            {
                _timer -= _timeService.DeltaTime;

                if (!_winAnimActive && _timer <= _prototypeConfig.WinWindowTiming - _prototypeConfig.WinAnimationTiming)
                {
                    _winAnimActive = true;
                    foreach (var i in _player)
                    {
                        _player.Get1(i).HandsAnimator.SetBool(VictoryKeyHash, true);

                        foreach (var placeholder in _player.Get1(i).WeaponPlaceholders)
                        {
                            placeholder.gameObject.SetActive(false);
                        }
                    }

                    // _world.NewEntity().Get<ReplaceWeaponsAfterFightTag>();
                }

                if (_timer <= 0)
                {
                    _timer = _prototypeConfig.WinWindowTiming;
                    StateFactory.CreateState<RoundCompletedState>(_world);
                    _world.NewEntity().Get<VictoryFxSignal>();
                    
                    _world.NewEntity().Get<ReplaceWeaponsAfterFightTag>();
                    
                    if (VibroService.VibroSettings.VibroEnabled)
                    {
                        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Success);
                    }
                }
            }
            else
            {
                _timer = _prototypeConfig.WinWindowTiming;
                _winAnimActive = false;
            }
        }
    }

}