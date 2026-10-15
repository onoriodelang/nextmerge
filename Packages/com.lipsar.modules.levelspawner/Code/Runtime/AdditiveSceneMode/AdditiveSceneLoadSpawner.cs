using UnityEngine;
using UnityEngine.SceneManagement;
using Leopotam.Ecs;
using Modules.Utils;

namespace Modules.LevelSpawner.AdditiveSceneMode
{
    public enum SceneSwitchMode 
    {
        KeepMainSceneAsActive,
        SetLevelSceneAsActive
    }

    public struct AdditiveSceneLoadOperation
    {
        public string TargetSceneName;
        public float StartTime;
        public bool IsPrevSceneUnloaded;
        public AsyncOperation CurrentOp;
        public bool EntitiesCleared;
    }

    public class AdditiveSceneLoadSpawner : IEcsRunSystem
    {
        readonly EcsFilter<SpawnLevelSignal> _signal;
        readonly EcsFilter<AdditiveSceneLoadOperation> _op;
        readonly EcsFilter<LevelEntityTag> _levelEntities;
        
        readonly TimeService _time;
        readonly EcsWorld _world;
        
        readonly AdditiveSceneModeSpawnConfig _config;
        readonly LevelLoadingView _loadingView;
        readonly bool _loadingViewPresents;
        readonly SceneSwitchMode _sceneSwitchMode;

        private string lastLoadedScene;
        private Scene initialScene;

        public AdditiveSceneLoadSpawner(AdditiveSceneModeSpawnConfig config, LevelLoadingView view) 
        {
            _config = config;
            _loadingView = view;
            _loadingViewPresents = (view != null);
            lastLoadedScene = "";
            initialScene = SceneManager.GetActiveScene();
            _sceneSwitchMode = config.SwitchMode;
        }

        public void Run()
        {
            HandleLoadingOp();


            if (_signal.IsEmpty())
                return;

            foreach (var signal in _signal)
            {
                StartLevelSpawn(_signal.Get1(signal).LevelID);
            }
        }

        public void HandleLoadingOp() 
        {
            foreach (var i in _op)
            {
                ref var op = ref _op.Get1(i);
                if (!op.EntitiesCleared && (_time.Time - op.StartTime) > 0.2f) 
                {
                    // force cleanup of level entities
                    foreach (var levelEntity in _levelEntities)
                    {
                        _levelEntities.GetEntity(levelEntity).Destroy();
                    }

                    op.EntitiesCleared = true;

                    // unload prev scene
                    if (lastLoadedScene.Length > 0)
                    {
                        op.CurrentOp = SceneManager.UnloadSceneAsync(lastLoadedScene);
                    }
                    else 
                    {
                        // no need to unload scene, just pass as unloaded && load next scene
                        op.IsPrevSceneUnloaded = true;
                        op.CurrentOp = LoadTargetScene(op.TargetSceneName);
                    }

                    if (_sceneSwitchMode == SceneSwitchMode.SetLevelSceneAsActive) 
                    {
                        SceneManager.SetActiveScene(initialScene);
                    }
                        
                }
                else if (op.EntitiesCleared && !op.IsPrevSceneUnloaded && op.CurrentOp.isDone )
                {
                    // prev scene unloaded, start loading next scene
                    op.IsPrevSceneUnloaded = true;
                    op.CurrentOp = LoadTargetScene(op.TargetSceneName);
                }
                else if(op.EntitiesCleared && op.IsPrevSceneUnloaded && op.CurrentOp.isDone)
                {
                    lastLoadedScene = op.TargetSceneName;
                    _world.NewEntity().Get<LevelSpawner.LevelSpawnedSignal>();
                    _op.GetEntity(i).Destroy();
                    InitializeObjectsOnLastLoadedScene();
                    if (_loadingViewPresents)
                        _loadingView.Hide();

                    if (_sceneSwitchMode == SceneSwitchMode.SetLevelSceneAsActive) 
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
                    }
                }
                else if (_loadingViewPresents) 
                {
                    // update view
                    if (op.IsPrevSceneUnloaded)
                    {
                        _loadingView.RecordProgress(0.5f + op.CurrentOp.progress * 0.5f);
                    }
                    else if (!op.IsPrevSceneUnloaded && op.EntitiesCleared)
                    {
                        _loadingView.RecordProgress(0.2f + op.CurrentOp.progress * 0.3f);
                    }
                    else 
                    {
                        _loadingView.RecordProgress(Mathf.Min(Time.time - op.StartTime, 0.2f));
                    }
                    
                }
            }
        }


        public void StartLevelSpawn(int level) 
        {
            ref var op = ref _world.NewEntity().Get<AdditiveSceneLoadOperation>();
            op.IsPrevSceneUnloaded = false;
            op.EntitiesCleared = false;
            op.TargetSceneName = _config.GetCurrentSceneIndex(level);
            op.StartTime = _time.Time;

            if (_loadingViewPresents)
                _loadingView.Show();
        }

        public AsyncOperation LoadTargetScene(string sceneName) 
        {
            return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void InitializeObjectsOnLastLoadedScene()
        {
            GameObject[] rootObjects = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                foreach (var viewElement in rootObject.GetComponentsInChildren<ViewHub.ViewElement>())
                {
                    EcsEntity entity = _world.NewEntity();
                    viewElement.Spawn(entity, _world);
                    entity.Get<LevelEntityTag>();
                }
            }
        }
    }
}
