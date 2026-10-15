using Leopotam.Ecs;

namespace Modules.MergeSlash.UserInput
{
    public struct OnScreenHold
    {
        // normalised displacement counted by pointer displacement system
        public float XDisplacement;
        public float YDisplacement;
        public bool DragStarted;
    }
}