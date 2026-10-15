using Leopotam.Ecs;
using UnityEngine;

namespace Modules.ST.MeshSlicer.Components
{
    public struct Slice
    {
        public GameObject ToSlice;
        public EcsEntity ToSliceEntity;
        public Vector3 SlicerWorldPosition;
        public Vector3 SlicerWorldDirection;
        public Animator EnemyCanvasAnimator;
    }
}