using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            return instance ? instance : (instance = (new GameObject("GameManagerObject")).AddComponent<GameManager>());
        }
    }

    public GameState CurrentState = null;
    public bool isHost = false;
    public string MyId;
    public string OpId;

    private bool IsNewState(GameState prevState, GameState newState)
    {
        if (prevState == null && newState == null) return false;
        if (
            (prevState == null && newState != null)
            || (prevState != null && newState == null)
            || (prevState.CurrentState != newState.CurrentState)
        )
        {
            return true;
        }
        return false;
    }

    private void ChangeLocalState(GameState newState)
    {
        if (IsNewState(CurrentState, newState))
        {
            switch (newState.CurrentState)
            {
                case State.ARRANGE_SHIPS:
                    {
                        SceneManager.LoadScene(Constants.SCENE_INDEX_PREGAME);
                        break;
                    }
                default: break;
            }
        }
        CurrentState = newState;
        string JsonifiedState = JsonUtility.ToJson(newState);
        Logger.Log("GameManager: ChangeLocalState - " + JsonifiedState);
    }

    private void NotifyChangeState(GameState newState)
    {
        string JsonifiedState = JsonUtility.ToJson(newState);
        Connection.Instance.Send(new Message(MessageTypes.STATE_CHANGE, JsonifiedState));
        Logger.Log("GameManager: NotifyChangeState - " + JsonifiedState);
    }

    public void ChangeState(GameState newState)
    {
        if (isHost)
        {
            ChangeLocalState(newState);
        }
        NotifyChangeState(newState);
    }

    public void OnRequestChangeState(GameState newState)
    {
        ChangeLocalState(newState);
        if (isHost)
        {
            NotifyChangeState(newState);
        }
    }

    public void StartGame()
    {
        GameState newState = new GameState();
        newState.CurrentState = State.ARRANGE_SHIPS;
        ChangeState(newState);
    }
}
