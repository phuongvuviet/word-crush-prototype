using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] BoardUIController boardUIController;
    [SerializeField] WordPreviewer wordPreviewer;
    [SerializeField] WordAnswersDisplayer answersDisplayer; 
    [SerializeField] GameObject winDialog; 
    [SerializeField] TextMeshProUGUI levelTxt;

    public static GameController Instance;

    GameDataLoader dataLoader;
    LevelData data;
    Vector2Int startPosition = Vector2Int.one * -1, endPosition = Vector2Int.one * -1;
    List<string> targetWords;
    List<string> solvedWords; 
    string curAns = "";
    bool isCurAnsWrong = false;
    HintWordInfo hintWordInfo = null;
    BoardLogicController boardLogic = null;
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
    }

    private void Start()
    { 
        dataLoader = new GameDataLoader();
        GameData gameData = dataLoader.LoadGameData();
        if(gameData != null) {
            targetWords = gameData.AllWords; 
            solvedWords = gameData.SolvedWords;
            boardUIController.Initialize(gameData.BoardData.GetCharBoard(), targetWords);
            // Debug.Log("Solved words: " + targetWords.Count);
            answersDisplayer.SetWordAnswers(targetWords);
            // Debug.Log("Solved words: " + solvedWords.Count + " sovled: " + solvedWords[0]);
            answersDisplayer.ShowAnswer(solvedWords);
        } else {
            LoadCurrentLevel();
        }
        wordPreviewer.ResetText();
        levelTxt.text = "LEVEL: " + Prefs.CurrentLevel;
        boardLogic = new BoardLogicController(boardUIController.GetCharBoard(), targetWords);
    }
    public void LoadCurrentLevel() {
        levelTxt.text = "LEVEL: " + Prefs.CurrentLevel;
        data = dataLoader.LoadCurrentLevelData();
        if (data == null) {
            Prefs.CurrentLevel--;
            data = dataLoader.LoadCurrentLevelData();
        }
        targetWords = data.Words;
        solvedWords = new List<string>();
        boardUIController.Initialize(targetWords);
        answersDisplayer.SetWordAnswers(targetWords);
        wordPreviewer.ResetText();
    }
    public void SetWordPosition(Vector2Int pos)
    {
        if (startPosition == Vector2Int.one * -1)
        {
            startPosition = pos;
            wordPreviewer.SetWord(boardUIController.GetLetter(pos.x, pos.y).ToString());
            boardUIController.ChangeCellsState(startPosition, startPosition, BoardCell.BoardCellState.ACTIVE);
        } else
        {
            SetEndPostion(pos);
        }
    }

    public void SetEndPostion(Vector2Int pos)
    {
        boardUIController.ChangeCellsState(startPosition, endPosition, BoardCell.BoardCellState.NORMAL);
        endPosition = pos;
        boardUIController.ChangeCellsState(startPosition, endPosition, BoardCell.BoardCellState.ACTIVE);
        wordPreviewer.SetWord(boardUIController.GetWord(startPosition, endPosition));
        // Debug.Log("start pos: " + startPosition + " end pos: " + endPosition);
    }
    
    public bool HasStartPosition()
    {
        return startPosition != Vector2Int.one * -1;
    }
    public void CheckWord()
    {
        if (endPosition == Vector2Int.one * -1) endPosition = startPosition;

        string curWord = boardUIController.GetWord(startPosition, endPosition);
        if (solvedWords.Contains(curWord)) {
            Debug.Log(curWord + " is found");
        }
        else if (targetWords.Contains(curWord))
        {
            answersDisplayer.ShowAnswer(curWord);
            boardUIController.RemoveCellsAndUpdateBoard(startPosition, endPosition);
            solvedWords.Add(curWord);
            if (solvedWords.Count == targetWords.Count) {
                Prefs.GameData = "";
                Prefs.CurrentLevel++;
                winDialog.SetActive(true);
            }
        }
        else
        {
            boardUIController.ChangeCellsState(startPosition, endPosition, BoardCell.BoardCellState.NORMAL);
        }
        startPosition = Vector2Int.one * -1;
        endPosition = Vector2Int.one * -1;
        wordPreviewer.ResetText();
    }
    public void ShuffleBoard()
    {
        List<string> remainingWords = new List<string>();
        for (int i = 0; i < targetWords.Count; i++) {
            if (!solvedWords.Contains(targetWords[i])) {
                remainingWords.Add(targetWords[i]);
            }
        }
        boardUIController.ShuffleBoard(remainingWords);
    }
    public void Hint() {
        // if (hintWordInfo == null) {
        //     BoardLogicController boardLogic = new BoardLogicController(boardUIController.GetCharBoard(), targetWords);
        //     hintWordInfo = boardLogic.FindCorrectWordPosition(solvedWords); 
        // } else {
        //     Debug.Log(hintWordInfo.ToString());
        // }

        // BoardLogicController boardLogic = new BoardLogicController(boardUIController.GetCharBoard(), targetWords);
        // hintWordInfo = boardLogic.FindCorrectWordPosition(solvedWords); 
        // Debug.Log(hintWordInfo.ToString());
        if (!boardLogic.IsHintWordCompleted(solvedWords)) {
            Vector2Int hintPosition = boardLogic.GetHintPosition(solvedWords);
            Debug.LogError("Hint position: " + hintPosition);
            if (hintPosition != Vector2Int.one * -1) {
                boardUIController.ChangeCellState(hintPosition, BoardCell.BoardCellState.HINT);
            }
        } else {
            Debug.LogError("Final hint");
        }
    }

    private void OnApplicationQuit() {
        Debug.Log("On application quit");
        SaveGame();
    }
    public void SaveGame() {
        Debug.Log("Save gameeeeeeeeeeeeeeeeeeeeeeeeee");
        char[,] board = boardUIController.GetCharBoard(); 
        GameData gameData = new GameData(board, targetWords, solvedWords);
        dataLoader.SaveGameData(gameData);
    }
}
