using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataLoader  
{
    const string levelDataPath = "LevelData/Level";
    public void SaveGameData(GameSessionData gameData) {
        string gameDataJson = JsonUtility.ToJson(gameData);
        Debug.Log("game data json: " + gameDataJson);
        Prefs.GameData = gameDataJson; 
    }
    public GameSessionData LoadGameData() {
        GameSessionData gameData = null;
        if (Prefs.HasSessionData) {
            string gameDataJson = Prefs.GameData;
            if (gameDataJson != "") {
                gameData = JsonUtility.FromJson<GameSessionData>(gameDataJson);
                if (gameData == null) {
                    Debug.Log("Can not convert game data json to game data");
                }
            } else {
                Debug.Log("[GameDataLaoder] can not load game session data");
            } 
        } else {
            // Debug.LogError("2");
            LevelData curLevelData = LoadCurrentLevelData();
            if (curLevelData == null) {
                Debug.LogError("Level data is null");
            }
            gameData = new GameSessionData(curLevelData, new char[1,1], new List<string>());
        }
        return gameData;
    } 
    LevelData LoadCurrentLevelData() {
        int currentLevel = Prefs.CurrentLevel;
        LevelData levelData = Resources.Load<LevelData>(levelDataPath + currentLevel);
        if (levelData == null) {
            Debug.Log("Fail to load level data: " + currentLevel);
        }
        return levelData;
    }
}
