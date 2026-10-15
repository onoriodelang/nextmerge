using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.ST;
using Modules.ST.EntityTemplates;
using Modules.ST.MeshSlicer;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class FailedTracker : IEcsRunSystem
    {
        private EcsFilter<InHandTag> _saws;
        private EcsFilter<SawThrownTag> _thrown;
        private EcsFilter<GamePlayState> _gameplay;
        private EcsFilter<EnemyView> _enemies;
        private EcsFilter<Player> _player;

        private EcsWorld _world;
        private TimeService _timeService;
        
        private PrototypeConfig _prototypeConfig;
        
        private float _timer;
        private bool _shooted;
        private bool _smile;
        

        public FailedTracker(PrototypeConfig prototypeConfig)
        {
            _prototypeConfig = prototypeConfig;
        }

        public void Run()
        {
            if (!_gameplay.IsEmpty() && _saws.IsEmpty() && !_thrown.IsEmpty())
            {
                _timer -= _timeService.DeltaTime;
                
                if (_timer <= _prototypeConfig.LooseWindowTiming - _prototypeConfig.EnemySmileTiming)
                {
                    _world.NewEntity().Get<EnemySmileSignal>();
                    _smile = true;
                }
                
                if (!_shooted && _timer <= _prototypeConfig.LooseWindowTiming - _prototypeConfig.EnemyShootTiming)
                {
                    _world.NewEntity().Get<EnemyShootTag>();
                    _shooted = true;
                }
                
                if (_timer <= _prototypeConfig.LooseWindowTiming - _prototypeConfig.CameraReturnTiming)
                {
                    ref Player player = ref _player.Get1(0);
                    
                    _world.NewEntity().Get<CameraUtils.SwitchCameraSignal>() = new CameraUtils.SwitchCameraSignal
                    {
                        CameraId = 2,
                        Follow = player.CameraFocus,
                        LookAt = player.CameraFocus
                    };
                }

                if (_timer <= 0)
                {
                    _timer = _prototypeConfig.LooseWindowTiming;
                    StateFactory.CreateState<RoundFailedState>(_world);
                    if (VibroService.VibroSettings.VibroEnabled)
                    {
                        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Warning);
                    }
                }
            }
            else
            {
                _timer = _prototypeConfig.LooseWindowTiming;
                _shooted = false;
                _smile = false;
            }
        }
    }

}