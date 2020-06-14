using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BOTGameManager : MonoBehaviour
{
    private static BOTGameManager instance = null;
    public static BOTGameManager Instance
    {
        get
        {
            return instance ? instance : (instance = (new GameObject("BOTGameManagerObject")).AddComponent<BOTGameManager>());
        }
    }

    public bool isHost = false;
    public GameState CurrentState = null;

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
        AfterChange(oldState);
    }

    public bool IsMyTurn()
    {
        return !GameManager.Instance.IsMyTurn();
    }

    private void AfterChange(GameState oldState)
    {
        if (IsFieldUpdated(oldState, CurrentState, "CurrentState"))
        {
            switch (CurrentState.CurrentState)
            {
                case State.PICK_CELL:
                    {
                        Logger.Log("BOTGameManager: PICK_CELL");
                        if (IsMyTurn())
                        {
                            StartCoroutine(DelayBOTAttack(1));
                        }
                        break;
                    }

                default: break;
            }
        }
    }

    private void NotifyChangeState(GameState newState)
    {
        GameManager.Instance.OnRequestChangeState(newState);
    }

    public void ChangeState(GameState newState)
    {
        NotifyChangeState(newState);
    }

    public void OnRequestChangeState(GameState newState)
    {
        ChangeLocalState(newState);
    }

    public void Ready(string serializedShips)
    {
        GameState newState = CurrentState.Copy();
        newState.GuestPlayerShips = serializedShips;
        ChangeState(newState);
    }

    public void Attack(TargetModel target)
    {
        GameState newState = CurrentState.Copy();
        newState.CurrentState = State.FIRE;
        newState.Target = JsonUtility.ToJson(target);
        ChangeState(newState);
    }

    IEnumerator DelayBOTAttack(float wait)
    {
        yield return new WaitForSeconds(wait);
        InGameController controller = GameObject.FindGameObjectWithTag(Tags.SCENE_CONTROLLER).GetComponent<InGameController>();
        controller.BOTAttack();
    }
}
