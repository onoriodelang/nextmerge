using Leopotam.Ecs;
using Modules.LevelSpawner;
using Modules.MergeSlash.Components;
using Modules.MergeSlash.Interactors;
using Modules.MergeSlash.Trackers;
using Modules.MergeSlash.UI.UnityComponents;
using Modules.Root.ECS;
using Modules.Root.ContainerComponentModel;
using Modules.ST;
using Modules.ST.MeshSlicer;
using Modules.Utils;
using UnityEngine;

namespace Modules.MergeSlash
{
    [CreateAssetMenu(menuName = "Modules/MergeSlash/Provider")]
    public class MSProvider : ScriptableObject, ISystemsProvider
    {
        [SerializeField] private PrototypeConfig _prototypeConfig;
        [SerializeField] private LevelsCollection _levelsCollection;
        
        public EcsSystems GetSystems(EcsWorld world, EcsSystems endFrame, EcsSystems mainSystems)
        {
            EcsSystems systems = new EcsSystems(world, "MSGame");

            #region AppContainerCheck
            if (AppContainer.Instance == null) 
            {
                // wrong behavior
                // app container not initialized, handle via init scene load
                Debug.LogWarning(
                    "<color=darkblue>CommonTemplate:</color> App container not initialized, resolved via init scene load\n" +
                    "LOOK AT: http://youtrack.lipsar.studio/articles/LS-A-149/App-container-not-initialized"
                    );

#if UNITY_IOS || UNITY_ANDROID
                Handheld.Vibrate();
#endif

                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                return systems;
            }
            #endregion

            systems
                .Add(new GameInit())
                
                .Add(new GoldSystem())
                .Add(new TutorialSystem())
                .Add(new LevelDataInitSystem())
                .Add(new FailedTracker(_prototypeConfig))
                .Add(new CompleteTracker(_prototypeConfig))
                .Add(new BuyWeaponSystem())
                .Add(new BuySlotSystem())
                .Add(new SawHorizontalMoveTracker())
                
                .Add(new WaitTimerSystem())
                .Add(new SawCollisionTracker())
                .Add(new SawThrowTracker())
                .Add(new SawSpawner())
                .Add(new SawThrower())
                .Add(new SawFlyProcessing())
                .Add(new SawHorizontalMover())
                .Add(new EnemyHitProcessing())
                .Add(new WeaponPreparationSystem(_prototypeConfig))
                .Add(new OpenDoorSystem())
                .Add(new MergeFxSystem())
                .Add(new SwitchPlayerViewSystem())
                .Add(new ExplosionProcessor())
                .Add(new MergeHelpSystem())
                .Add(new GoldForSellSystem())
                .Add(new CameraFocusSystem())
                .Add(new ReplaceWeaponsAfterFightSystem())
                .Add(new EnemyShootSystem())
                .Add(new EmoteSystem(_prototypeConfig))
                .Add(new WalkingSystem())
                
                
                .Add(new DelayedScaleSystem())
                .Add(new ProgressBarSystem(_levelsCollection))
                
                .Add(new GameplayButtonsSystem(_prototypeConfig))
                .Add(new VictoryFxSystem())
                .Add(new BloodScreenSystem())
                
                // mesh slicer
                .Add(new SliceProcessor())

                // event group
                .Add(new EventGroup.StateCleanupSystem())       // remove entity with prev state component
                .Add(new EventHandlers.OnRestartRoundEnter())   // on click at restart button
                .Add(new EventHandlers.OnNextLevelEnter())      // start next level
                .Add(new EventHandlers.OnGamePlayStateEnter())  // enter at gameplay stage
                .Add(new EventHandlers.OnRoundCompletedEnter()) // on round completed state enter
                .Add(new EventHandlers.OnRoundFailedEnter())    // on round failed state enter

                .Add(new Utils.TimedDestructorSystem());

            endFrame
              .OneFrame<SlashButtonSignal>()
              .OneFrame<BuySlotButtonSignal>()
              .OneFrame<BuyWeaponButtonSignal>()
              .OneFrame<ThrowButtonSignal>()
              .OneFrame<OpenDoorSignal>()
              .OneFrame<VictoryFxSignal>()
              .OneFrame<EnemyHitSignal>()
              .OneFrame<SawHorizontalMove>()
              .OneFrame<EnemyWalkSignal>()
              .OneFrame<EnemyShootTag>()
              .OneFrame<EnemyAngrySignal>()
              .OneFrame<EnemyScarySignal>()
              .OneFrame<EnemySmileSignal>()
              .OneFrame<EventGroup.StateEnter>();

            return systems;
        }
    }
}
