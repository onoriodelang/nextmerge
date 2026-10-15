using Leopotam.Ecs;

namespace Modules.LevelSpawner
{
    // TAGS
    // represents specific state of an entity


    /// <summary>
    /// tag attached to level related entities
    /// </summary>
    public struct LevelEntityTag : IEcsIgnoreInFilter { }

    /// <summary>
    /// tag attached to previous level entity
    /// </summary>
    public struct CleanEntityTag : IEcsIgnoreInFilter { }


    public struct AssetSpawnedTag : IEcsIgnoreInFilter { }


}