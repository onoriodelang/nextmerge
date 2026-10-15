using Leopotam.Ecs;

namespace Modules.LevelSpawner
{
    // SIGNALS
    // one frame action components


    /// <summary>
    /// order to begin level spawn process
    /// </summary>
    public struct SpawnLevelSignal
    {
        public int LevelID;
    }

    /// <summary>
    /// signal which indicates completion of level spawn process
    /// </summary>
    public struct LevelSpawnedSignal: IEcsIgnoreInFilter { }


}