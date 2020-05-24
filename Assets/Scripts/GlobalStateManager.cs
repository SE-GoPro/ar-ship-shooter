using System;
using UnityEngine;
using System.Runtime.CompilerServices;

public class GlobalStateManager
{
    public GlobalStateManager()
    {
    }

    private GlobalState CurrentState;

    public GlobalState GetCurrentState => CurrentState;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public GlobalState ChangeState(GlobalState nextState)
    {
        switch(nextState)
        {
            case GlobalState.SETTINGS:
                if (CurrentState == GlobalState.MAIN_MENU) {
                    // Show settings
                    break;
                }
                return CurrentState;

            case GlobalState.SELECTING_GAME_MODE:
                if (CurrentState == GlobalState.MAIN_MENU)
                {
                    // Show main menu
                    break;
                }
                return CurrentState;

            case GlobalState.FINDING_NEARBY:
                if (CurrentState == GlobalState.SELECTING_GAME_MODE)
                {
                    // Find nearby devices that is openning this game
                    break;
                }
                return CurrentState;

            case GlobalState.NEARBY_LIST:
                if (CurrentState == GlobalState.FINDING_NEARBY)
                {
                    // Show selecting modal
                    break;
                }
                return CurrentState;

            case GlobalState.MATCHING:
                if (CurrentState == GlobalState.NEARBY_LIST)
                {
                    // Match 2 player, set up connection
                    break;
                }
                if (CurrentState == GlobalState.POST_GAME)
                {
                    // rematch
                    break;
                }
                return CurrentState;

            case GlobalState.PRE_GAME:
                if (CurrentState == GlobalState.MATCHING)
                {
                    // Use PreGameStateManager
                    break;
                }
                return CurrentState;

            case GlobalState.IN_GAME:
                if (CurrentState == GlobalState.PRE_GAME)
                {
                    // Use InGameStateManager
                    break;
                }
                return CurrentState;

            case GlobalState.POST_GAME:
                if (CurrentState == GlobalState.IN_GAME)
                {
                    // Show result
                    break;
                }
                return CurrentState;

            case GlobalState.MAIN_MENU:
                if (
                    CurrentState == GlobalState.SETTINGS
                    || CurrentState == GlobalState.POST_GAME
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
}
