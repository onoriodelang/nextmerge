using UICoreECS;
using UnityEngine;

namespace Modules.MergeSlash.UI.UnityComponents
{
    /// <summary>
    ///  generally for debug purposes
    /// </summary>
    public class StateTransitionButton : WordEventButton
    {
        private enum States
        {
            NextLevelState,
            GamePlayState,
            RoundCompletedState,
            RoundFailedState,
            RestartRoundState
        }

        [SerializeField] private States _state;

        public override void Do()
        {
            switch (_state)
            {
                case States.NextLevelState:
                    EventGroup.StateFactory.CreateState<EventGroup.NextLevelState>(_world);
                    break;
                case States.GamePlayState:
                    EventGroup.StateFactory.CreateState<EventGroup.GamePlayState>(_world);
                    break;
                case States.RoundCompletedState:
                    EventGroup.StateFactory.CreateState<EventGroup.RoundCompletedState>(_world);
                    break;
                case States.RoundFailedState:
                    EventGroup.StateFactory.CreateState<EventGroup.RoundFailedState>(_world);
                    break;
                case States.RestartRoundState:
                    EventGroup.StateFactory.CreateState<EventGroup.RestartRoundState>(_world);
                    break;
                default:
                    break;
            }
        }
    }
}
