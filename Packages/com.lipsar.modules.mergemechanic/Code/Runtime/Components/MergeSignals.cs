using Leopotam.Ecs;

namespace Modules.MergeMechanic.Components
{
    public struct InitMergeUnitViewSignal : IEcsIgnoreInFilter { }
    public struct MergeTargetSignal : IEcsIgnoreInFilter { }
    public struct MergeVictimSignal : IEcsIgnoreInFilter { }
    public struct SwapPositionTargetSignal : IEcsIgnoreInFilter { }
    public struct SwapPositionVictimSignal : IEcsIgnoreInFilter { }
    public struct UpdateMergeUnitViewSignal : IEcsIgnoreInFilter { }
    public struct DragStartSignal : IEcsIgnoreInFilter { }
    public struct DragEndSignal : IEcsIgnoreInFilter { }
    public struct RepositionSignal : IEcsIgnoreInFilter { }
    public struct UnlockTileSignal : IEcsIgnoreInFilter { }
    public struct UnlockedTileSignal : IEcsIgnoreInFilter { }
    public struct UpdateTileViewSignal : IEcsIgnoreInFilter { }
    public struct MergeHintClearSignal : IEcsIgnoreInFilter { }
    public struct MergeHintAddedSignal : IEcsIgnoreInFilter { }
    public struct SellMergeUnitSignal : IEcsIgnoreInFilter { }

    public struct PurhaceMergeUnitSignal 
    {
        public int TypeID;
        public int Level;
        public int TileID;
    }

}
