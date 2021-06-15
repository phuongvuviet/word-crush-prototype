using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordStackGamePlay 
{
    BoardDataGenerator boardGenerator;
    BoardLogicController boardLogic;
    GameDataLoader gameDataLoader;
    GameSessionData sessionData;
    char[,] charBoard;

    public WordStackGamePlay() {
        gameDataLoader = new GameDataLoader();
        boardGenerator = new BoardDataGenerator();
        LoadGameData();
    }
    public void LoadGameData() {
        sessionData = gameDataLoader.LoadGameData();
        if (sessionData == null) {
            Debug.Log("Session data is null");
        }
        if (!Prefs.HasSessionData) {
            sessionData.BoardData = new BoardData(boardGenerator.GenerateBoard(sessionData.LevelData.Words));
            Prefs.HasSessionData = true;
        }
        charBoard = sessionData.BoardData.GetCharBoard();
        boardLogic = new BoardLogicController(charBoard);
    }
    public char[,] ShuffleBoard() {
        List<string> remaingWords = GetRemainingWords();
        charBoard = boardGenerator.GenerateBoard(remaingWords);
        boardLogic.SetCharBoard(charBoard);
        return charBoard;
    }
    public char[,] GetCharBoard() {
        if (sessionData == null) {
            LoadGameData();
        } 
        return sessionData.BoardData.GetCharBoard();
    }
    public void SaveGameSession() {
        sessionData.BoardData = new BoardData(charBoard);
        gameDataLoader.SaveGameData(sessionData);
    }
    public string GetWord(Vector2Int fromPos, Vector2Int toPos) {
        return boardLogic.GetWord(fromPos, toPos);
    }
    public bool CheckValidInputPositions(Vector2Int fromPos, Vector2Int toPos) {
        // Debug.LogError(fromPos + " - " + toPos + " isValid: " + boardLogic.CheckValidInputPositions(fromPos, toPos));
        // Debug.Log(boardLogic.Show());
        return boardLogic.CheckValidInputPositions(fromPos, toPos);
    }
    public List<Vector2Int> GetAllPositionInRange(Vector2Int fromPos, Vector2Int toPos) {
        return boardLogic.GetAllPositionsInRange(fromPos, toPos); 
    }
    public CellSteps RemoveCellsInRangeAndCollapsBoard(Vector2Int fromPos, Vector2Int toPos) {
        return boardLogic.RemoveCellsInRangeAndCollapsBoard(fromPos, toPos);
    }
    public bool CheckWord(string word) {
        if (sessionData.SolvedWords.Contains(word)) {
            return false;
        }
        if (sessionData.LevelData.Words.Contains(word)) {
            sessionData.SolvedWords.Add(word);
            return true;
        }
        return false;
    }
    public bool HasSolvedAllWords() {
        return sessionData.SolvedWords.Count == sessionData.LevelData.Words.Count;
    }
    public List<string> GetTargetWords() {
        return sessionData.LevelData.Words;
    }
    public List<string> GetSolvedWords() {
        return sessionData.SolvedWords;
    }
    List<string> GetRemainingWords() {
        List<string> remainingWords = new List<string>();
        List<string> allWords = GetTargetWords();
        for (int i = 0; i < allWords.Count; i++) {
            if (!sessionData.SolvedWords.Contains(allWords[i])) {
                remainingWords.Add(allWords[i]);
            }
        }
        return remainingWords;
    }
    public Vector2Int GetNextHintPosition() {
        List<string> remainingWords = GetRemainingWords();
        return boardLogic.GetNextHintPosition(remainingWords);
    }
    public bool HasHintWord() {
        return !boardLogic.IsHintWordCompleted(GetRemainingWords());
    }
    public bool IsHintWordCompleted() {
        return boardLogic.IsHintWordCompleted(GetRemainingWords());
    }
    public List<Vector2Int> GetHintWordPositions() {
        HintWordInfo hintWord = boardLogic.GetHintWordInfo();
        return new List<Vector2Int>() {
            hintWord.GetStartPosition(),
            hintWord.GetEndPosition()
        };
    }
    public void ResetHintWord() {
        boardLogic.SetHintWordInfo(null);
    }
}
