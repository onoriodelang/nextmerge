using UnityEngine;

using Leopotam.Ecs;
using Modules.Root.ECS;

namespace Modules.MergeSlash.UserInput
{
    [CreateAssetMenu(menuName = "Modules/MergeSlash/UserInput/Provider")]
    public class UserInputSystems : ScriptableObject, ISystemsProvider
    {
        [SerializeField] private float _minToDrag = 0.05f;
        [SerializeField] private float _clickCheckTime = 0.2f;

        public EcsSystems GetSystems(EcsWorld world, EcsSystems endFrame, EcsSystems ecsSystems)
        {
            EcsSystems systems = new EcsSystems(world, this.name);

            systems
                .Add(new TapTrackerSystem(_clickCheckTime))           // track events
                .Add(new PointerDisplacementSystem(_minToDrag))       // count displacement
                ;

            endFrame
                .OneFrame<PointerDown>()
                .OneFrame<PointerClick>()
                .OneFrame<PointerUp>()
                .OneFrame<OnDragStarted>()
                .OneFrame<OnScreenClick>()
                .OneFrame<OnScreenLongClick>()
                ;

            return systems;
        }
    }
}
