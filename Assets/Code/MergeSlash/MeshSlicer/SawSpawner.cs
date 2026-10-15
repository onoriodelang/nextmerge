using Leopotam.Ecs;
using Modules.MergeSlash.EntityTemplates;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class SawSpawner : IEcsRunSystem
    {
        // private EcsFilter<SpawnSawSignal>.Exclude<WaitTimer> _filter;
        private EcsFilter<SpawnSawPoint, UnityView> _spawnPoint;
        private EcsFilter<EventGroup.RoundCompletedState> _roundCompleted;
        private EcsFilter<EventGroup.RoundFailedState> _roundFailed;
        private EcsFilter<CameraUtils.VirtualCamera> _camera;

        private EcsWorld _world;
        
        public void Run()
        {
            if(!_roundCompleted.IsEmpty() || !_roundFailed.IsEmpty() )
                return;

            bool spawned = false;
            
            foreach (var pointIdx in _spawnPoint)
            {
                ref var spawnPoint = ref _spawnPoint.Get1(pointIdx);
                
                if(spawnPoint.Spawned)
                    continue;
                
                Transform spawnPointTransform = _spawnPoint.Get2(pointIdx).Transform;
                Vector3 spawnPos = spawnPointTransform.position;
                Quaternion spawnRot = spawnPointTransform.rotation;

                // spawn saw
                var sawEntity = _world.NewEntity();
                var saw = GameObject.Instantiate(spawnPoint.Saw, spawnPos, spawnRot);
                saw.transform.parent = spawnPointTransform;
                saw.Spawn(sawEntity, _world);
                
                // change camera
                _world.NewEntity().Get<CameraUtils.SwitchCameraSignal>() = new CameraUtils.SwitchCameraSignal
                {
                    CameraId = 0,
                    Follow = null,
                    LookAt = null
                };

                spawned = true;
                spawnPoint.Spawned = true;
            }

            if (!spawned)
                return;
            
            // foreach (var i in _filter)
            // {
            //     _filter.GetEntity(i).Del<SpawnSawSignal>();
            // }
        }
    }

    // public struct SpawnSawSignal : IEcsIgnoreInFilter
    // {
    //     
    // }
    
    public struct SpawnSawPoint 
    {
        public SawTemplate Saw;
        public bool Spawned;
    }
}