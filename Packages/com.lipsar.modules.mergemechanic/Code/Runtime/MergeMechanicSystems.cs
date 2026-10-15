using Leopotam.Ecs;
using Modules.Root.ECS;
using UnityEngine;

namespace Modules.MergeMechanic
{
    [CreateAssetMenu(menuName = "Modules/MergeMechanic/Provider")]
    public class MergeMechanicSystems : ScriptableObject, ISystemsProvider
    {
        [SerializeField] private Data.UnitViewConfig _unitViewConfig;
        [SerializeField] private Data.DragConfig _dragConfig;
        [SerializeField] private Data.SaveConfig _saveConfig;
        [SerializeField] private Data.TileConfig _tileConfig;
        [SerializeField] private Data.HintConfig _hintConfig;
        [SerializeField] private Data.InititialUnitsConfig _inititialUnitsConfig;

        public EcsSystems GetSystems(EcsWorld world, EcsSystems endFrame, EcsSystems ecsSystems)
        {
            EcsSystems systems = new EcsSystems(world, this.name);

            systems

                .Add(new RestoreTileState())
                .Add(new RestoreMergeUnitOnTiles())

                .Add(new MergeUnitPurchaseSystem())

                .Add(new StartDragMergeUnitSystem())
                .Add(new EndDragMergeUnitSystem())
                .Add(new DragMergeUnitSystem())

                .Add(new MergeResultProcessor())
                .Add(new SwapPositionProcessor())

                .Add(new RepositionMergeUnitOnTiles())
                .Add(new MergeUnitViewUpdateSystem())

                .Add(new MergeHintSystem())

                .Add(new TileUnlockSystem())
                .Add(new TileLockedViewUpdateSystem())

                .Add(new SaveMergeUnitOnTiles())
                .Add(new SaveTileState())

                .Add(new TileHighlightSystem(Time.fixedDeltaTime))

                .OneFrame<Components.UnlockTileSignal>()
                .OneFrame<Components.PurhaceMergeUnitSignal>()
                ;

            systems
                .Inject(_unitViewConfig, typeof(Data.UnitViewConfig))
                .Inject(_dragConfig, typeof(Data.DragConfig))
                .Inject(_saveConfig, typeof(Data.SaveConfig))
                .Inject(_tileConfig, typeof(Data.TileConfig))
                .Inject(_hintConfig, typeof(Data.HintConfig))
                .Inject(_inititialUnitsConfig, typeof(Data.InititialUnitsConfig))
                ;

            endFrame
                .OneFrame<Components.InitMergeUnitViewSignal>()
                .OneFrame<Components.MergeTargetSignal>()
                .OneFrame<Components.MergeVictimSignal>()
                .OneFrame<Components.SwapPositionTargetSignal>()
                .OneFrame<Components.SwapPositionVictimSignal>()
                .OneFrame<Components.UpdateMergeUnitViewSignal>()
                .OneFrame<Components.DragStartSignal>()
                .OneFrame<Components.DragEndSignal>()
                .OneFrame<Components.RepositionSignal>()
                .OneFrame<Components.UnlockedTileSignal>()
                .OneFrame<Components.UpdateTileViewSignal>()
                .OneFrame<Components.MergeHintAddedSignal>()
                .OneFrame<Components.MergeHintClearSignal>()
                .OneFrame<Components.SellMergeUnitSignal>()
                ;

            return systems;
        }
    }
}
