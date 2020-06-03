using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : StateNotifier
{
    public static StateManager instance = null;

    public bool isHost = false;

    private StateManager()
    {
        CurrentState = State.MAIN_MENU;
    }

    public static StateManager Init()
    {
        if (instance == null)
        {
            instance = new StateManager();
        }
        return instance;
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

            case State.FINDING_NEARBY:
                if (CurrentState == State.MAIN_MENU)
                {
                    // Find nearby devices that is openning this game
                    break;
                }
                return CurrentState;

            case State.PRE_GAME:
                if (CurrentState == State.FINDING_NEARBY)
                {
                    SceneManager.LoadScene(Constants.SCENE_INDEX_PREGAME);
                    break;
                }
                return CurrentState;

            case State.IN_GAME:
                if (CurrentState == State.PRE_GAME)
                {
                    SceneManager.LoadScene(Constants.SCENE_INDEX_INGAME);
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
