using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class StateManager : StateNotifier
{
    public StateManager()
    {
    }

    private State CurrentState;

    public State GetCurrentState => CurrentState;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public State ChangeState(State nextState)
    {
        Dictionary<object, object> Data = new Dictionary<object, object>();
        switch (nextState)
        {
            case State.SETTINGS:
                if (CurrentState == State.MAIN_MENU) {
                    // Show settings
                    break;
                }
                return CurrentState;

            case State.SELECTING_GAME_MODE:
                if (CurrentState == State.MAIN_MENU)
                {
                    // Show main menu
                    break;
                }
                return CurrentState;

            case State.FINDING_NEARBY:
                if (CurrentState == State.SELECTING_GAME_MODE)
                {
                    // Find nearby devices that is openning this game
                    break;
                }
                return CurrentState;

            case State.NEARBY_LIST:
                if (CurrentState == State.FINDING_NEARBY)
                {
                    // Show selecting modal
                    break;
                }
                return CurrentState;

            case State.MATCHING:
                if (CurrentState == State.NEARBY_LIST)
                {
                    // Match 2 player, set up connection
                    break;
                }
                if (CurrentState == State.POST_GAME)
                {
                    // rematch
                    break;
                }
                return CurrentState;

            case State.PRE_GAME:
                if (CurrentState == State.MATCHING)
                {
                    // Use PreGameStateManager
                    break;
                }
                return CurrentState;

            case State.IN_GAME:
                if (CurrentState == State.PRE_GAME)
                {
                    // Use InGameStateManager
                    break;
                }
                return CurrentState;

            case State.POST_GAME:
                if (CurrentState == State.IN_GAME)
                {
                    // Show result
                    break;
                }
                return CurrentState;

            case State.MAIN_MENU:
                if (
                    CurrentState == State.SETTINGS
                    || CurrentState == State.POST_GAME
                )
                {
                    // Show result
                    break;
                }
                return CurrentState;

            default:
                return CurrentState;
        }

        CurrentState = nextState;
        return CurrentState;
    }

    private List<StateListener> StateListeners = new List<StateListener>();

    public void RegisterListener(StateListener listener)
    {
        StateListeners.Add(listener);
    }

    public void NotifyNewState(State prevState, State currentState)
    {
        StateListeners.ForEach(delegate (StateListener listener)
        {
            listener.OnNewState(prevState, currentState);
        });
    }
}
