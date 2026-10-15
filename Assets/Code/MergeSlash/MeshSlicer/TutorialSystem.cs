using Leopotam.Ecs;
using Modules.EventGroup;
using Modules.LevelSpawner;
using Modules.MergeMechanic.Components;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.EntityTemplates;
using Modules.MergeSlash.UI;
using Modules.PlayerLevel;
using Modules.Utils;
using Modules.ViewHub;
using UnityEngine;

namespace Modules.MergeSlash.Interactors
{
    public class TutorialSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter<MergeUnit, UnityView> _mergeUnits;
        private EcsFilter<PlaceInHands> _placeSignal;
        private EcsFilter<MergeUnit, UnityView, OnTile> _mergeUnitsOnTile;
        private EcsFilter<TileViewRef, UnityView> _tiles;
        private EcsFilter<MergeTargetSignal> _mergeSignal;
        private EcsFilter<DragTargetTag> _drag;
        private EcsFilter<RepositionSignal> _repositionSignal;
        private EcsFilter<TutorialPanelView> _panelView;
        private EcsFilter<SellMergeUnitSignal> _sell;
        private EcsFilter<GamePlayState> _gameplayState;

        private EcsWorld _world;
        private bool _mergeTutorialCompleted;
        private bool _replaceTutorialCompleted;
        private bool _mergeTutorialProcess;
        private bool _replaceTutorialProcess;
        private bool _tutorialPaused;
        private bool _frameSkiped;

        private const string MergeTutorialSaveKey = "MSlash_merge_tutorial_completed";
        private const string ReplaceTutorialSaveKey = "MSlash_replace_tutorial_completed";
        
        public void Init()
        {
            _mergeTutorialCompleted = PlayerPrefs.GetInt(MergeTutorialSaveKey, 0) != 0;
            _replaceTutorialCompleted = PlayerPrefs.GetInt(ReplaceTutorialSaveKey, 0) != 0;
        }

        public void Run()
        {
            if(_mergeTutorialCompleted && _replaceTutorialCompleted)
                return;
            
            if(_gameplayState.IsEmpty())
                return;

            if (ProgressionInfo.CurrentLevel == 0 && !_mergeTutorialProcess && !_replaceTutorialProcess)
            {
                if (!_mergeTutorialCompleted)
                    EnableMergeTutorial();
                else
                    EnableReplaceTutorial();
            }
            

            if (ProgressionInfo.CurrentLevel != 0)
            {
                DisableAllTutorials();
            }

            if (_replaceTutorialProcess || _mergeTutorialProcess)
            {
                if (_tutorialPaused)
                {
                    if(_drag.IsEmpty() && !_mergeUnits.IsEmpty())
                        ResumeTutorial();
                }
                else
                {
                    if(!_drag.IsEmpty())
                        PauseTutorial();

                    if (_mergeUnits.IsEmpty())
                    {
                        _mergeTutorialProcess = false;
                        _replaceTutorialProcess = false;
                        PauseTutorial();
                    }
                }
            }

            if (!_placeSignal.IsEmpty())
            {
                CompleteMergeTutorial();
                CompleteReplaceTutorial();
            }

            if (_replaceTutorialProcess && !_repositionSignal.IsEmpty() && HasUnitInActiveTile())
            {
                CompleteReplaceTutorial();
            }

            if (_mergeTutorialProcess && (!_mergeSignal.IsEmpty() || _mergeUnits.GetEntitiesCount() < 2))
            {
                CompleteMergeTutorial();
            }
        }

        private void EnableMergeTutorial()
        {
            if(_mergeTutorialCompleted || _panelView.IsEmpty())
                return;
            
            if (_mergeUnits.GetEntitiesCount() < 2)
            {
                CompleteMergeTutorial();
                return;
            }
            
            Vector3 target1 = Vector3.zero;
            Vector3 target2 = Vector3.zero;
            bool target1Founded = false;

            foreach (var i in _mergeUnits)
            {
                if (target1Founded)
                {
                    target2 = _mergeUnits.Get2(i).Transform.position;
                    break;
                }
                
                target1 = _mergeUnits.Get2(i).Transform.position;
                target1Founded = true;
            }
            
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.SetTutorialTargets(target1, target2);
                _panelView.Get1(i).View.EnableMergeTutorial();
            }
            
            _mergeTutorialProcess = true;
            _replaceTutorialProcess = false;
        }

        private void EnableReplaceTutorial()
        {
            if(_replaceTutorialCompleted || _panelView.IsEmpty() || _mergeUnits.GetEntitiesCount() > 1 || !_sell.IsEmpty())
                return;

            if (HasUnitInActiveTile())
            {
                CompleteReplaceTutorial();
                return;
            }
            
            Vector3 emptyTilePos = Vector3.zero;
            bool emptyTileFounded = false;

            foreach (var tileIdx in _tiles)
            {
                if (_tiles.Get1(tileIdx).ID > 2)
                    continue;

                bool freeTile = true;

                foreach (var unitIdx in _mergeUnitsOnTile)
                {
                    if (_mergeUnitsOnTile.Get3(unitIdx).TileID != _tiles.Get1(tileIdx).ID)
                        continue;

                    freeTile = false;
                    break;
                }

                if (freeTile)
                {
                    emptyTileFounded = true;
                    emptyTilePos = _tiles.Get2(tileIdx).Transform.position;
                }
            }

            Vector3 target1 = Vector3.zero;
            bool targetFounded = false;

            foreach (var i in _mergeUnits)
            {
                target1 = _mergeUnits.Get2(i).Transform.position;
                targetFounded = true;
                break;
            }

            if (!emptyTileFounded || !targetFounded)
            {
                CompleteReplaceTutorial();
                return;
            }
            
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.SetTutorialTargets(target1, emptyTilePos);
                _panelView.Get1(i).View.EnableReplaceTutorial();
            }
            
            _mergeTutorialProcess = false;
            _replaceTutorialProcess = true;
        }
        
        private void CompleteMergeTutorial()
        {
            PlayerPrefs.SetInt(MergeTutorialSaveKey, 1);
            PlayerPrefs.Save();

            _mergeTutorialCompleted = true;
            _mergeTutorialProcess = false;

            EnableReplaceTutorial();
        }

        private void CompleteReplaceTutorial()
        {
            PlayerPrefs.SetInt(ReplaceTutorialSaveKey, 1);
            PlayerPrefs.Save();

            _replaceTutorialCompleted = true;
            _replaceTutorialProcess = false;
            
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.DisableTutorial();
            }
        }

        private void DisableAllTutorials()
        {
            _replaceTutorialProcess = false;
            _mergeTutorialProcess = false;
            
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.DisableTutorial();
            }
        }

        private void PauseTutorial()
        {
            _tutorialPaused = true;
            
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.PauseTutorial();
            }
        }
        
        private void ResumeTutorial()
        {
            _tutorialPaused = false;
            foreach (var i in _panelView)
            {
                _panelView.Get1(i).View.ResumeTutorial();
            }
            
            if (_replaceTutorialProcess)
            {
                EnableReplaceTutorial();
            }
            
            if (_mergeTutorialProcess)
            {
                EnableMergeTutorial();
            }
        }

        private bool HasUnitInActiveTile()
        {
            foreach (var i in _mergeUnitsOnTile)
            {
                if (_mergeUnitsOnTile.Get3(i).TileID <= 2)
                    return true;
            }

            return false;
        }
    }

}