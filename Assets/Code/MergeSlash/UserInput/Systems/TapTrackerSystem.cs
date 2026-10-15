using Leopotam.Ecs;
using Modules.Utils;
using UnityEngine;

namespace Modules.MergeSlash.UserInput
{
    public sealed class TapTrackerSystem : IEcsRunSystem
    {
        private EcsFilter<OnScreenTapUp> _up;
        private EcsFilter<OnScreenTapDown> _down;

        private EcsFilter<OnScreenHold> _holdFilter;
        private EcsFilter<OnScreenLongClick> _longClickFilter;
        
        private EcsWorld _ecsWorld;
        private TimeService _time;
        
        private float tapStartTime;

        private readonly float _clickCheckTime;
        
        public TapTrackerSystem(float clickCheckTime)
        {
            _clickCheckTime = clickCheckTime;
        }

        public void Run()
        {
            if(!_up.IsEmpty())
            {
                foreach (var i in _up)
                {
                    _up.GetEntity(i).Del<OnScreenTapUp>();

                    // onScreenClick
                    if (_time.Time - tapStartTime < _clickCheckTime)
                    {
                        _ecsWorld.NewEntity().Get<OnScreenClick>().ScreenClickPos = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && _up.IsEmpty())
            {
                _ecsWorld.NewEntity().Get<OnScreenTapUp>();
            }
            else if(!_up.IsEmpty())
            {
                foreach (var i in _up)
                {
                    _up.GetEntity(i).Del<OnScreenTapUp>();
                    
                    // onScreenClick
                    if (_time.Time - tapStartTime < _clickCheckTime)
                    {
                        _ecsWorld.NewEntity().Get<OnScreenClick>().ScreenClickPos = Input.mousePosition;
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && _down.IsEmpty())
            {
                _ecsWorld.NewEntity().Get<OnScreenTapDown>();
                tapStartTime = _time.Time;
            }
            else if (!_down.IsEmpty())
            {
                foreach (var i in _down)
                {
                    _down.GetEntity(i).Del<OnScreenTapDown>();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (_holdFilter.IsEmpty())
                    _ecsWorld.NewEntity().Get<OnScreenHold>();
            }
            else
            {
                if (!_holdFilter.IsEmpty())
                {
                    foreach (var i in _holdFilter)
                    {
                        // onScreenLongClick
                        if ((_holdFilter.Get1(i).DragStarted == false) && (_time.Time - tapStartTime > _clickCheckTime))
                        {
                            _ecsWorld.NewEntity().Get<OnScreenLongClick>().ScreenClickPos = Input.mousePosition;
                        }
                        
                        _holdFilter.GetEntity(i).Destroy();
                    }
                }  
            }
        }
    }
}