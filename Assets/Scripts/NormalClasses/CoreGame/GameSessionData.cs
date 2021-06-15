using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSessionData
{
    public BoardData BoardData; 
    public LevelData LevelData;
    public List<string> SolvedWords;
    public GameSessionData(LevelData levelData, char[,] board, List<string> solvedWords) {
        LevelData = levelData;
        BoardData = new BoardData(board);
        SolvedWords = solvedWords;
    }
}
