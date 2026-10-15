using Leopotam.Ecs;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.MergeMechanic
{
    public class TileHighlightSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilter<Components.DragStartSignal> _startDrag = null;
        private readonly EcsFilter<Components.MergeUnit, Components.DragTargetTag> _drag = null;
        private readonly EcsFilter<Components.DragEndSignal> _endDrag = null;

        private readonly EcsFilter<Components.MergeUnit>.Exclude<Components.DragTargetTag> _fieldUnits = null;
        private readonly EcsFilter<Components.TileViewRef>.Exclude<Components.LockedTag> _availableTiles = null;
        private readonly EcsFilter<Components.DumpTag> _dumpTiles = null;

        private readonly Data.TileConfig _tileConfig;

        private readonly Modules.Utils.TimeService _time;
        private readonly float _runDeltaTime;
        private float _lastRunTime;

        private Camera camera;
        private static readonly int Selected = Animator.StringToHash("Selected");

        public TileHighlightSystem()
        {
            _runDeltaTime = 0.05f;
            _lastRunTime = -Mathf.Infinity;
        }

        public TileHighlightSystem(float runDeltaTime)
        {
            _runDeltaTime = runDeltaTime;
            _lastRunTime = -Mathf.Infinity;
        }

        public void Init()
        {
            camera = Camera.main;
        }

        public void Run()
        {
            if (_startDrag.IsEmpty() == false)
            {
                foreach (var i in _drag)
                {
                    HighlightMergableTiles(CollectMergeableTile(_drag.Get1(i)));
                    HighlightOriginTile(_drag.GetEntity(i).Get<Components.OnTile>().TileID);
                }
            }

            if (_endDrag.IsEmpty() == false)
            {
                TurnOffHighligt();
                TurnOffTilesHoverView();
                return;
            }
            
            if(_drag.IsEmpty())
                foreach (var i in _dumpTiles)
                {
                    _dumpTiles.Get1(i).Animator.SetBool(Selected, false);
                }

            if (_drag.IsEmpty() || _time.Time - _lastRunTime < _runDeltaTime)
                return;

            _lastRunTime = _time.Time;

            RaycastCheck();
        }

        private void RaycastCheck()
        {
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, float.PositiveInfinity, _tileConfig.TileLayer, QueryTriggerInteraction.Collide))
            {
                var entityRef = hit.transform.GetComponent<ViewHub.EntityRef>();
                if (entityRef != null && entityRef.Entity.Has<Components.TileViewRef>() && !entityRef.Entity.Has<Components.LockedTag>())
                {
                    TurnOffTilesHoverView();
                    entityRef.Entity.Get<Components.TileViewRef>().View.SwitchOnHoverView(true);
                }

                if (entityRef != null && entityRef.Entity.Has<Components.DumpTag>())
                {
                    entityRef.Entity.Get<Components.DumpTag>().Animator.SetBool(Selected, true);
                    entityRef.Entity.Get<Components.DumpTag>().DefaultView.SetActive(false);
                    entityRef.Entity.Get<Components.DumpTag>().HoverView.SetActive(true);
                }
                    
            }
            else
            {
                TurnOffTilesHoverView();
            }
        }

        private void TurnOffHighligt()
        {
            foreach (var i in _availableTiles)
            {
                _availableTiles.Get1(i).View.ResetView();
            }
            
        }

        private void HighlightOriginTile(int tileID)
        {
            foreach (var i in _availableTiles)
            {
                _availableTiles.Get1(i).View.SwitchOnUnitPickedView(_availableTiles.Get1(i).ID == tileID);
            }
        }

        private void TurnOffTilesHoverView()
        {
            foreach (var i in _availableTiles)
            {
                _availableTiles.Get1(i).View.SwitchOnHoverView(false);
            }
            
            foreach (var i in _dumpTiles)
            {
                _dumpTiles.Get1(i).Animator.SetBool(Selected, false);
                _dumpTiles.Get1(i).DefaultView.SetActive(true);
                _dumpTiles.Get1(i).HoverView.SetActive(false);
            }
        }

        private void HighlightMergableTiles(List<int> tilesID)
        {
            foreach (var i in _availableTiles)
            {
                if (tilesID.Contains(_availableTiles.Get1(i).ID))
                    _availableTiles.Get1(i).View.SwitchOnMergeAvailableView(true);
            }
        }

        private List<int> CollectMergeableTile(Components.MergeUnit draggableUnit)
        {
            List<int> tilesID = new List<int>();
            foreach (var i in _fieldUnits)
            {
                var target = _fieldUnits.Get1(i);

                if (target.TypeID == draggableUnit.TypeID && target.CurrentLevel == draggableUnit.CurrentLevel && target.CurrentLevel < target.MaxLevel)
                    tilesID.Add(_fieldUnits.GetEntity(i).Get<Components.OnTile>().TileID);
            }

            return tilesID;
        }
    }
}