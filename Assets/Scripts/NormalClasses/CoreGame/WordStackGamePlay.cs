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
        } else if (sessionData.LevelData == null) {
            Debug.Log("sessiondata/leveldata is null");
        }
        if (!Prefs.HasSessionData) {
            sessionData.BoardData = new BoardData(boardGenerator.GenerateBoard(sessionData.LevelData.Words));
            Prefs.HasSessionData = true;
        }
        if (sessionData.LevelData == null) {
            Debug.Log("Level data is null in load game data");
        }
        charBoard = sessionData.BoardData.GetCharBoard();
        boardLogic = new BoardLogicController(charBoard);
        if (sessionData.HintWord != null) {
            boardLogic.SetHintWordInfo(sessionData.HintWord);
        }
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
        Debug.Log("Save game session");
        sessionData.BoardData = new BoardData(charBoard);
        sessionData.HintWord = boardLogic.GetHintWordInfo();
        gameDataLoader.SaveGameData(sessionData);
    }
    public string GetWord(Vector2Int fromPos, Vector2Int toPos) {
        return boardLogic.GetWord(fromPos, toPos);
    }
    public bool VerityInputPositions(Vector2Int fromPos, Vector2Int toPos) {
        return boardLogic.VerityInputPositions(fromPos, toPos);
    }
    public List<Vector2Int> GetAllPositionsInRange(Vector2Int fromPos, Vector2Int toPos) {
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
        if (sessionData == null) Debug.Log("session data is null");
        else if (sessionData.LevelData == null) Debug.Log("Level data is null");
        return sessionData.LevelData.Words;
    }
    public List<string> GetSolvedWords() {
        return sessionData.SolvedWords;
    }
    List<string> GetRemainingWords() {
        // Debug.LogError("1");
        List<string> remainingWords = new List<string>();
        List<string> allWords = GetTargetWords();
        for (int i = 0; i < allWords.Count; i++) {
            if (!sessionData.SolvedWords.Contains(allWords[i])) {
                remainingWords.Add(allWords[i]);
            }
        }
        return remainingWords;
    }
    public GameSessionData GetGameSessionData() {
        return sessionData;
    }
    public Vector2Int GetNextHintPosition() {
        List<string> remainingWords = GetRemainingWords();
        return boardLogic.GetNextHintPosition(remainingWords);
    }
    // public bool HasHintWord() {
    //     return !boardLogic.IsHintWordCompleted(GetRemainingWords());
    // }
    public bool IsHintWordCompleted() {
        return boardLogic.IsHintWordCompleted(GetRemainingWords());
    }
    public List<Vector2Int> GetHintWordEndPositions() {
        HintWordInfo hintWord = boardLogic.GetHintWordInfo();
        return new List<Vector2Int>() {
            hintWord.GetStartPosition(),
            hintWord.GetEndPosition()
        };
    }
    public void SetHintWord(HintWordInfo hintWord) {
        boardLogic.SetHintWordInfo(hintWord);
    }
    public HintWordInfo GetHintWordInfo() {
        return boardLogic.GetHintWordInfo();
    } 
    public string GetHintWord() {
        if (GetHintWordInfo() != null) {
            return GetHintWordInfo().Word;
        }
        return "";
    }
    public HintWordInfo FindHintWord(List<string> remainingWords) {
        return boardLogic.FindHintWord(remainingWords);
    }
    public void UpdateHintWordInfo() {
        boardLogic.UpdateHintWordInfo();
    }
}
