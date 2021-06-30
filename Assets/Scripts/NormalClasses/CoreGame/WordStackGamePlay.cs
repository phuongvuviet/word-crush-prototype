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
    List<string> remainingWords; 

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
        ComputeRemainingWords();
    }
    public List<MoveInfo> ShuffleBoard() {
        // List<string> remaingWords = GetRemainingWords();
        Dictionary<char, List<Vector2Int>> charPositionsBefore, charPositionAfter;
        charPositionsBefore = GetCharPositions();
        char[,] charBoardBefore = charBoard;
        while (IsSame(charBoard, charBoardBefore)) {
            charBoard = boardGenerator.GenerateBoard(remainingWords);
        }
        boardLogic.SetCharBoard(charBoard);
        charPositionAfter = GetCharPositions();
        return GetShuffleMove(charPositionsBefore, charPositionAfter);
    }

    bool IsSame(char[,] board1,char[,] board2) {
        if (board1.GetLength(0) != board2.GetLength(0) || board1.GetLength(1) != board2.GetLength(1)) {
            return false;
        }     
        for (int i = 0; i < board1.GetLength(0); i++) {
            for (int j = 0; j < board1.GetLength(1); j++) {
                if (board1[i, j] != board2[i, j]) return false;
            }
        }
        return true;
    }

    public List<MoveInfo> GetShuffleMove(Dictionary<char, List<Vector2Int>> before, Dictionary<char, List<Vector2Int>> after) {
        List<MoveInfo> moveInfos = new List<MoveInfo>();
        foreach (var item in before) {
            for (int i = 0; i < item.Value.Count; i++) {
                Vector2Int fromPos = item.Value[i];
                Vector2Int toPos = after[item.Key][i];
                // Debug.Log("from: " + fromPos + " to: " + toPos);
                moveInfos.Add(new MoveInfo(fromPos, toPos));
            }
        }
        return moveInfos;
    }

    Dictionary<char, List<Vector2Int>> GetCharPositions() {
        Dictionary<char, List<Vector2Int>> charPositions = new Dictionary<char, List<Vector2Int>>();
        for (int i = 0; i < charBoard.GetLength(0); i++) {
            for (int j = 0; j < charBoard.GetLength(1); j++) {
                if (charBoard[i, j] != ' ') {
                    if (!charPositions.ContainsKey(charBoard[i, j])) {
                        charPositions.Add(charBoard[i, j], new List<Vector2Int>());
                    }
                    charPositions[charBoard[i, j]].Add(new Vector2Int(i, j));
                }
            }
        }
        return charPositions;
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
        return boardLogic.VerifyInputPositions(fromPos, toPos);
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
            if (sessionData.SolvedWords == null) {
                Debug.Log("Solved words is nullllllll");
            }
            sessionData.SolvedWords.Add(word);
            remainingWords.Remove(word);
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
    void ComputeRemainingWords() {
        // List<string> remainingWords = new List<string>();
        remainingWords = new List<string>();
        List<string> allWords = GetTargetWords();
        for (int i = 0; i < allWords.Count; i++) {
            if (!sessionData.SolvedWords.Contains(allWords[i])) {
                remainingWords.Add(allWords[i]);
            }
        }
    }
    // public List<string> GetRemainingWords() {
    //     // Debug.LogError("1");
    //     return remainingWords;
    // }
    public GameSessionData GetGameSessionData() {
        return sessionData;
    }
    public Vector2Int GetNextHintPosition() {
        // List<string> remainingWords = GetRemainingWords();
        return boardLogic.GetNextHintPosition(remainingWords);
    }
    public bool IsHintWordCompleted() {
        return boardLogic.IsHintWordCompleted();
    }
    public List<Vector2Int> GetHintWordEndPositions() {
        HintWordInfo hintWord = boardLogic.GetHintWordInfo();
        return new List<Vector2Int>() {
            hintWord.GetStartPosition(),
            hintWord.GetEndPosition()
        };
    }
    public HintWordInfo GetHintWordInfo() {
        return boardLogic.GetHintWordInfo();
    } 
    public void SetHintWord(HintWordInfo hintWordInfo) {
        boardLogic.SetHintWordInfo(hintWordInfo);
    } 
    public string GetHintWord() {
        if (GetHintWordInfo() != null) {
            return GetHintWordInfo().Word;
        }
        return "";
    }
    public void ResetHintWord() {
        boardLogic.ResetHintWord();
    }
    public HintWordInfo FindHintWord() {
        return boardLogic.FindHintWord(remainingWords);
    }
    public bool CheckIfBoardHasHintWord() {
        return boardLogic.CheckIfBoardHasHintWord();
    }
    public void UpdateHintWordInfo() {
        boardLogic.UpdateHintWordInfo();
    }
    public bool CheckIfBoardValid() {
        return boardLogic.CheckIfBoardValid(remainingWords);
    }
    public int GetBoardWidth() {
        return charBoard.GetLength(1);
    }
    public int GetBoardHeight() {
        return charBoard.GetLength(0);
    }
}
