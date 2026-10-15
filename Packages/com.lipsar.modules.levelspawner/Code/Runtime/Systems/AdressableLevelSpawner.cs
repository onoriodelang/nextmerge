using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Modules.ViewHub;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Modules.LevelSpawner
{
    public struct AddressableLevelAsset 
    {
        public AsyncOperationHandle<GameObject> Handle;
        public float StartTime;
        public string TargetAddres;
    }


    public class AddressableLevelSpawner : IEcsRunSystem
    {
        // auto injected fields
        readonly EcsFilter<SpawnLevelSignal> _signal;
        readonly EcsFilter<AddressableLevelAsset>.Exclude<AssetSpawnedTag> _currentAsset;
        readonly EcsFilter<AddressableLevelAsset, AssetSpawnedTag> _prevAsset;
        readonly EcsFilter<LevelEntityTag> _levelEntity;
        readonly Utils.TimeService _time;
        readonly EcsWorld _world;

        readonly LevelLoadingView _loadingView;
        readonly LevelsCollection _levelsCollection;
        readonly bool _loadingViewPresents;

        public AddressableLevelSpawner(LevelsCollection levelsCollection, LevelLoadingView view) 
        {
            _levelsCollection = levelsCollection;
            _loadingView = view;
            _loadingViewPresents = _loadingView != null;
        }
        
        public void Run() 
        {
            // spawn logic
            foreach (var i in _currentAsset)
            {
                if (_currentAsset.Get1(i).Handle.Status == AsyncOperationStatus.Failed) 
                {
                    Debug.LogError($"<color=darkblue>LevelSpawner:</color> Addresables load operation failed" +
                        $" <color=red>FailedAdress: {_currentAsset.Get1(i).TargetAddres}</color>" +
                        "\nLOOK AT: http://youtrack.lipsar.studio/articles/LS-A-151/LevelSpawner:-Addresables-load-error \n\n");

                    continue;
                }

                // if loading completed
                if (_currentAsset.Get1(i).Handle.IsDone && (_time.Time - _currentAsset.Get1(i).StartTime) > 0.3f) 
                {
                    // spawn prefab
                    GameObject level = GameObject.Instantiate(_currentAsset.Get1(i).Handle.Result);
                    // collect child entities
                    ViewElement[] viewElements = level.GetComponentsInChildren<ViewElement>();

                    // allocate entity for prefab root
                    EcsEntity entity = _world.NewEntity();
                    ref var view = ref entity.Get<UnityView>();
                    view.GameObject = level;
                    view.Transform = level.transform;
                    entity.Get<LevelEntityTag>();

                    // allocate entities for view elements of prefab
                    for (int j = 0; j < viewElements.Length; j++)
                    {
                        entity = _world.NewEntity();
                        entity.Get<LevelEntityTag>();

                        viewElements[j].Spawn(entity, _world);
                        viewElements[j] = null;
                    }
                    viewElements = null;

                    // indicate asset as spawned asset and tell to the world that spawn completed
                    _currentAsset.GetEntity(i).Get<AssetSpawnedTag>();
                    _currentAsset.GetEntity(i).Get<LevelSpawnedSignal>();

                    // hide loading panel
                    if (_loadingViewPresents)
                        _loadingView.Hide();

                }
                else if(_loadingViewPresents)
                {
                    // else update loading progress view
                    if(!_currentAsset.Get1(i).Handle.IsDone && (_currentAsset.Get1(i).Handle.PercentComplete < (_time.Time - _currentAsset.Get1(i).StartTime) / 0.3f)) 
                    {
                        _loadingView.RecordProgress(_currentAsset.Get1(i).Handle.PercentComplete);
                    }
                    else 
                    {
                        _loadingView.RecordProgress((_time.Time - _currentAsset.Get1(i).StartTime) / 0.3f);
                    }
                }
            }

            // if there is new level spawn request
            if (_signal.IsEmpty())
                return;

            // delegate prev asset for cleanup
            foreach (var i in _prevAsset)
            {
                _prevAsset.GetEntity(i).Get<CleanEntityTag>();
            }

            ref var asset = ref _world.NewEntity().Get<AddressableLevelAsset>();

            // retrieve target level id to load
            int targetLevelID = 0;
            foreach (var i in _signal)
            {
                targetLevelID = _signal.Get1(i).LevelID;
            }

            // receive target adress to load
            string targetAsset = _levelsCollection.Get(targetLevelID);

            // pass load data
            asset.Handle = Addressables.LoadAssetAsync<GameObject>(targetAsset);
            asset.TargetAddres = targetAsset;

            asset.StartTime = _time.Time;

            // delegate prev entities for cleanup
            foreach (var i in _levelEntity)
            {
                _levelEntity.GetEntity(i).Get<CleanEntityTag>();
            }

            // show loading panel
            if(_loadingView != null)
                _loadingView.Show();
        }
    }
}
