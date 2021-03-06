﻿using System;
using UnityEngine;

public class Constants
{
    public const int MAP_SIZE = 10;
    public const int CELL_SIZE = 10;
    public const int CELL_ELEVATION = 6;
    public const float LEFT_OFFSET = -100.0f;
    public const float BOTTOM_OFFSET = -55.0f;
    public const string CELL_COLOR_NORMAL = "#FFFFFFFF";
    public const string CELL_COLOR_VALID = "#62FF00";
    public const string CELL_COLOR_INVALID = "#E64100AA";
    public const string CELL_COLOR_SELECTED = "#7AC637AA";
    public const string CELL_COLOR_FIRING = "#525252AA";
    public const string CELL_COLOR_GREYED = "#525252AA";
    public const string SHIP_COLOR_RED = "#B04A31FF";
    public const string SHIP_COLOR_BLUE = "#6BA0BEFF";
    public const int SHIP_AMOUNT = 10;
    public const int MAX_HP = 20;
    public const int EMPTY_CELL_ID = -1;

    public const string WFDR_NAMESPACE = "arship01";

    public const int SCENE_INDEX_MENU = 0;
    public const int SCENE_INDEX_PREGAME = 1;
    public const int SCENE_INDEX_INGAME = 2;
    public const int SCENE_INDEX_INGAME_AR = 3;

    public const int ARRANGE_SHIP_TIME = 60;
}
