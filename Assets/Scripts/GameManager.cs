using System;
using System.Collections;
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
        AfterChange(oldState);
    }

    private void AfterChange(GameState oldState)
    {
        if (IsFieldUpdated(oldState, CurrentState, "CurrentState"))
        {
            switch (CurrentState.CurrentState)
            {
                case State.ARRANGE_SHIPS:
                    {
                        Logger.Log("GameManager: ARRANGE_SHIPS");
                        if (Connection.Instance.OpId == null)
                        {
                            Logger.Log("GameManager: retrieve OpId from CurrentState");
                            Connection.Instance.OpId = isHost ? CurrentState.GuestPlayerId : CurrentState.HostPlayerId;
                        }
                        SceneManager.LoadSceneAsync(Constants.SCENE_INDEX_PREGAME);
                        break;
                    }

                case State.BEGIN_BATTLE:
                    {
                        Logger.Log("GameManager: BEGIN_BATTLE");
                        SceneManager.LoadSceneAsync(Constants.SCENE_INDEX_INGAME);
                        break;
                    }

                case State.PICK_CELL:
                    {
                        Logger.Log("GameManager: PICK_CELL");
                        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
                        controller.StartTurn(IsMyTurn());
                        break;
                    }

                case State.FIRE:
                    {
                        Logger.Log("GameManager: FIRE");
                        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
                        TargetModel target = JsonUtility.FromJson<TargetModel>(CurrentState.Target);
                        controller.DisplayAttack(target);
                        break;
                    }

                case State.CHECK_WIN:
                    {
                        Logger.Log("GameManager: CHECK_WIN");
                        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
                        int MyHp = isHost ? CurrentState.HostPlayerHealth : CurrentState.GuestPlayerHealth;
                        int OpHp = isHost ? CurrentState.GuestPlayerHealth : CurrentState.HostPlayerHealth;
                        controller.UpdateHPAndCheckWin(MyHp, OpHp);
                        break;
                    }

                case State.CHANGE_TURN:
                    {
                        Logger.Log("GameManager: CHANGE_TURN");
                        // Short delay before starting new turn
                        StartCoroutine(DelayBeginTurn(1));
                        break;
                    }

                case State.END_GAME:
                    {
                        Logger.Log("GameManager: END_GAME");
                        bool isWin = CurrentState.PlayerWin == Connection.Instance.MyId;
                        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
                        controller.HandleWinner(isWin);
                        break;
                    }

                default: break;
            }
        }
        if (
            IsFieldUpdated(oldState, CurrentState, "HostPlayerShips")
            || IsFieldUpdated(oldState, CurrentState, "GuestPlayerShips")
        )
        {
            if (isHost)
            {
                if (!CurrentState.HostPlayerShips.Equals("") && !CurrentState.GuestPlayerShips.Equals(""))
                {
                    Logger.Log("GameManager: All player are ready");
                    BeginBattle();
                }
            }
        }
    }

    private void NotifyChangeState(GameState newState)
    {
        if (Connection.Instance.isOnline)
        {
            string JsonifiedState = JsonUtility.ToJson(newState);
            Connection.Instance.Send(new Message(MessageTypes.STATE_CHANGE, JsonifiedState));
            Logger.Log("GameManager: NotifyChangeState - " + JsonifiedState);
        }
        else
        {
            BOTGameManager.Instance.OnRequestChangeState(newState);
        }
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
        if (Connection.Instance.MyId == null || Connection.Instance.OpId == null)
        {
            Logger.Log("Error: can not retrieve OpId");
        }
        newState.HostPlayerId = Connection.Instance.MyId;
        newState.GuestPlayerId = Connection.Instance.OpId;
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

    public bool IsMyTurn()
    {
        bool isMyTurn = Connection.Instance.MyId.Equals(CurrentState.CurrentTurn);
        return isMyTurn;
    }

    public string GetShips(bool isMyShip)
    {
        if (isMyShip)
        {
            if (isHost)
            {
                return CurrentState.HostPlayerShips;
            }
            else
            {
                return CurrentState.GuestPlayerShips;
            }
        }
        else
        {
            if (isHost)
            {
                return CurrentState.GuestPlayerShips;
            }
            else
            {
                return CurrentState.HostPlayerShips;
            }
        }
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

    public void Attack(TargetModel target)
    {
        GameState newState = CurrentState.Copy();
        newState.CurrentState = State.FIRE;
        newState.Target = JsonUtility.ToJson(target);
        ChangeState(newState);
    }

    public void DecreaseHealth(int offset, bool isMyHealth)
    {
        if (isHost)
        {
            GameState newState = CurrentState.Copy();
            newState.CurrentState = State.CHECK_WIN;
            newState.Hit = (offset == -1);
            if (isMyHealth)
            {
                newState.HostPlayerHealth = newState.HostPlayerHealth + offset;
            }
            else
            {
                newState.GuestPlayerHealth = newState.GuestPlayerHealth + offset;
            }
            ChangeState(newState);
        }
    }

    public void ChangeTurn()
    {
        if (isHost)
        {
            GameState newState = CurrentState.Copy();
            newState.CurrentState = State.CHANGE_TURN;
            string newTurn;
            if (CurrentState.CurrentTurn == Connection.Instance.MyId)
            {
                newTurn = Connection.Instance.OpId;
            }
            else
            {
                newTurn = Connection.Instance.MyId;
            }
            newState.CurrentTurn = newTurn;
            ChangeState(newState);
        }
    }

    IEnumerator DelayBeginTurn(float wait)
    {
        yield return new WaitForSeconds(wait);
        GameManager.Instance.BeginTurn();
    }

    public void EndGame(bool isSelfWin)
    {
        if (isHost)
        {
            GameState newState = CurrentState.Copy();
            newState.CurrentState = State.END_GAME;
            newState.PlayerWin = isSelfWin ? Connection.Instance.MyId : Connection.Instance.OpId;
            ChangeState(newState);
        }
    }
}
