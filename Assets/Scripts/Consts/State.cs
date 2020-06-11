using System;

public enum State
{
    NULL = 0,

    // Global States
    MAIN_MENU = 101,
    SETTINGS = 102,
    SELECTING_GAME_MODE = 103,
    FINDING_NEARBY = 104,
    NEARBY_LIST = 105,
    MATCHING = 106,
    PRE_GAME = 107,
    IN_GAME = 108,
    POST_GAME = 109,

    BEGIN_BATTLE = 200,

    // InGame States
    NEW_TURN = 201,
    PICK_CELL = 202,
    FIRE = 203,
    CHECK_WIN = 204,
    CHANGE_TURN = 205,
    END_GAME = 206,

    // PreGame States
    ARRANGE_SHIPS = 301,
    WAITING_OP = 302,
}
