using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public BoardData BoardData; 
    public List<string> SolvedWords;
    public List<string> AllWords;
    public GameData(char[,] board, List<string> allWords, List<string> solvedWords) {
        BoardData = new BoardData(board);
        SolvedWords = solvedWords;
        AllWords = allWords;
    }
}
