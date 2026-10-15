namespace Modules.MergeMechanic.Components
{
    [System.Serializable]
    public struct MergeUnit
    {
        public int TypeID;
        public int CurrentLevel;
        public int MaxLevel;
        public UnityEngine.Transform ViewRoot;
        public UnityEngine.BoxCollider Collider;
        public UnityEngine.Vector3 NormalColliderSize;
        public UnityEngine.Vector3 HelpColliderSize;
    }
}
