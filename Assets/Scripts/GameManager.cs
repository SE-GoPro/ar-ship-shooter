using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            return instance ? instance : (instance = (new GameObject("GameManagerObject")).AddComponent<GameManager>());
        }
    }

    public GameState CurrentState = null;
    public bool isHost = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private bool IsFieldUpdated(GameState prevState, GameState newState, string fieldName)
    {
        if (prevState == null && newState == null) return false;
        if (
            (prevState == null && newState != null)
            || (prevState != null && newState == null)
            || (!prevState.GetAttribute<object>(fieldName).Equals(newState.GetAttribute<object>(fieldName)))
        )
        {
            return true;
        }
        return false;
    }

    private void ChangeLocalState(GameState newState)
    {
        GameState oldState = CurrentState;
        CurrentState = newState;
        string JsonifiedState = JsonUtility.ToJson(newState);
        Logger.Log("GameManager: ChangeLocalState - " + JsonifiedState);
        // Changes if needed
        if (IsFieldUpdated(oldState, newState, "CurrentState"))
        {
            switch (newState.CurrentState)
            {
                case State.ARRANGE_SHIPS:
                    {
                        Logger.Log("GameManager: ARRANGE_SHIPS");
                        SceneManager.LoadScene(Constants.SCENE_INDEX_PREGAME);
                        break;
                    }

                case State.BEGIN_BATTLE:
                    {
                        Logger.Log("GameManager: BEGIN_BATTLE");
                        SceneManager.LoadScene(Constants.SCENE_INDEX_INGAME);
                        break;
                    }

                case State.PICK_CELL:
                    {
                        Logger.Log("GameManager: PICK_CELL");
                        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
                        bool isMyTurn = Connection.Instance.MyId.Equals(newState.CurrentTurn);
                        controller.StartTurn(isMyTurn);
                        break;
                    }

                default: break;
            }
        }
        if (
            IsFieldUpdated(oldState, newState, "HostPlayerShips")
            || IsFieldUpdated(oldState, newState, "GuestPlayerShips")
        )
        {
            if (isHost)
            {
                if (!newState.HostPlayerShips.Equals("") && !newState.GuestPlayerShips.Equals(""))
                {
                    Logger.Log("GameManager: All player are ready");
                    BeginBattle();
                }
            }
        }
    }

    private void NotifyChangeState(GameState newState)
    {
        string JsonifiedState = JsonUtility.ToJson(newState);
        Connection.Instance.Send(new Message(MessageTypes.STATE_CHANGE, JsonifiedState));
        Logger.Log("GameManager: NotifyChangeState - " + JsonifiedState);
    }

    public void ChangeState(GameState newState)
    {
        NotifyChangeState(newState);
        if (isHost)
        {
            ChangeLocalState(newState);
        }
    }

    public void OnRequestChangeState(GameState newState)
    {
        if (isHost)
        {
            NotifyChangeState(newState);
        }
        ChangeLocalState(newState);
    }

    public void StartGame()
    {
        GameState newState = new GameState();
        newState.CurrentState = State.ARRANGE_SHIPS;
        ChangeState(newState);
    }

    public void Ready(string serializedShips)
    {
        GameState newState = CurrentState.Copy();
        if (isHost)
        {
            newState.HostPlayerShips = serializedShips;
        } else
        {
            newState.GuestPlayerShips = serializedShips;
        }
        ChangeState(newState);
    }

    public void BeginBattle()
    {
        GameState newState = CurrentState.Copy();
        // The host claim the first turn
        newState.CurrentTurn = Connection.Instance.MyId;
        newState.CurrentState = State.BEGIN_BATTLE;
        ChangeState(newState);
    }

    public void BeginTurn()
    {
        if (isHost)
        {
            GameState newState = CurrentState.Copy();
            newState.CurrentState = State.PICK_CELL;
            ChangeState(newState);
        }
    }
}
