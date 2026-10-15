using Leopotam.Ecs;

namespace Modules.MergeMechanic
{
    public class SaveMergeUnitOnTiles : IEcsRunSystem
    {
        private readonly EcsFilter<Components.DragEndSignal> _dragEnd = null;
        private readonly EcsFilter<Components.SaveTilesTag> _signal = null;
        private readonly EcsFilter<Components.MergeUnit, Components.OnTile>.Exclude<Modules.Utils.DestroyTag> _units = null;

        private readonly Data.SaveConfig _saveConfig;
        public void Run()
        {
            if (_dragEnd.IsEmpty() && _signal.IsEmpty())
                return;

            int unitsCount = _units.GetEntitiesCount();

            int[] TypeID = new int[unitsCount];
            int[] CurrentLevel = new int[unitsCount];
            int[] TileID = new int[unitsCount];

            foreach (var i in _units)
            {
                var unit = _units.Get1(i);
                TypeID[i] = unit.TypeID;
                CurrentLevel[i] = unit.CurrentLevel;
                TileID[i] = _units.Get2(i).TileID;
            }

            Save(TypeID, CurrentLevel, TileID);

            foreach (var i in _signal)
            {
                _signal.GetEntity(i).Del<Components.SaveTilesTag>();
            }
        }

        private void Save(int[] typeID, int[] currentLevel, int[] tileID)
        {
            Utils.PlayerPrefsX.SetIntArray(_saveConfig.TypeIDSaveKey, typeID);
            Utils.PlayerPrefsX.SetIntArray(_saveConfig.CurrentLevelSaveKey, currentLevel);
            Utils.PlayerPrefsX.SetIntArray(_saveConfig.TileIDSaveKey, tileID);
            UnityEngine.PlayerPrefs.Save();
        }
    }
}