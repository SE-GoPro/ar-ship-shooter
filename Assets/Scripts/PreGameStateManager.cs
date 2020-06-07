using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreGameStateManager : StateNotifier
{
    public static PreGameStateManager instance = null;
    public int RemainingTime;
    public bool SelfReady = false;
    public bool OpReady = false;

    private PreGameStateManager()
    {
        CurrentState = State.NULL;
    }

    public static PreGameStateManager Init()
    {
        if (instance == null)
        {
            instance = new PreGameStateManager();
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
            case State.ARRANGE_SHIPS:
                if (CurrentState == State.NULL)
                {
                    RemainingTime = Constants.ARRANGE_SHIP_TIME;
                    break;
                }
                return CurrentState;

            case State.WAITING_OP:
                if (CurrentState == State.ARRANGE_SHIPS)
                {
                    OnReady(true);
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("type", "ready");
                    GameObject.FindGameObjectWithTag("CONNECTION").GetComponent<Connection>().Send(dict);
                    break;
                }
                return CurrentState;

            case State.NULL:
                if (CurrentState == State.WAITING_OP)
                {
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

    public void OnReady(bool isSelf)
    {
        if (isSelf)
            SelfReady = true;
        else
            OpReady = true;
        if (SelfReady && OpReady)
        {
            ChangeState(State.NULL);
            StateManager.instance.ChangeState(State.IN_GAME);
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //dict.Add("type", "ready");
            //GameObject.FindObjectOfType<Connection>().Send(dict);
        }

    }
}
