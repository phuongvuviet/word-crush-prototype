using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public BoardData BoardData; 
    public List<string> SolvedWord;
    public List<string> AllWords;
    public GameData(char[,] board, List<string> allWords, List<string> solvedWord) {
        BoardData = new BoardData(board);
        SolvedWord = solvedWord;
        AllWords = allWords;
    }
}
