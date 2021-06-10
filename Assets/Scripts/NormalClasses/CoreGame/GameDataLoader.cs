using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataLoader  
{
    const string levelDataPath = "LevelData/Level";
    public LevelData LoadCurrentLevelData() {
        int currentLevel = Prefs.CurrentLevel;
        Debug.Log("Current level: " + currentLevel);
        LevelData levelData = Resources.Load<LevelData>(levelDataPath + currentLevel);
        if (levelData == null) {
            Debug.Log("Fail to load level data: " + currentLevel);
        }
        return levelData;
    }
    public void SaveGameData(char[,] charBoard, List<string> allWords, List<string> solvedWords) {
        GameData gameData = new GameData(charBoard, allWords, solvedWords);
        string gameDataJson = JsonUtility.ToJson(gameData);
        Debug.Log("game data json: " + gameDataJson);
        Prefs.GameData = gameDataJson; 
    }
    public GameData LoadGameData() {
        string gameDataJson = Prefs.GameData;
        GameData gameData = null;
        if (gameDataJson != "") {
            gameData = JsonUtility.FromJson<GameData>(gameDataJson);
            if (gameData == null) {
                Debug.Log("Can not convert game data jsoin to game data");
            }
        } 
        return gameData;
    } 
}
