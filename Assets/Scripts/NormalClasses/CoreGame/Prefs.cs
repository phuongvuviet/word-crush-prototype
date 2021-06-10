using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Prefs 
{
    public static int CurrentLevel {
        get { return PlayerPrefs.GetInt("current_level", 1); }
        set { PlayerPrefs.SetInt("current_level", value); }
    } 
    public static string GameData{
        get { return PlayerPrefs.GetString("game_data", ""); }
        set { PlayerPrefs.SetString("game_data", value); }
    }
    // public static string CurrentBoardData {
    //     get { return PlayerPrefs.GetString("current_board_data", ""); }
    //     set { PlayerPrefs.SetString("current_board_data", value); }
    // } 
}
