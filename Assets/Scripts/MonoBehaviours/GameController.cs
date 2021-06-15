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
    [SerializeField] TextMeshProUGUI questionTxt;

    public static GameController Instance;

    Vector2Int fromPosition = Vector2Int.one * -1, toPosition = Vector2Int.one * -1;
    string curAns = "";
    HintWordInfo hintWordInfo = null;
    WordStackGamePlay gamePlay;
    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
    }
    private void Start()
	{
		gamePlay = new WordStackGamePlay();
		InitUI();
	}

	private void InitUI()
	{
		char[,] charBoard = gamePlay.GetCharBoard();
        boardUIController.Initialize(charBoard);
		wordPreviewer.ResetText();
        answersDisplayer.SetWordAnswers(gamePlay.GetTargetWords());
        answersDisplayer.ShowAnswer(gamePlay.GetSolvedWords());
		levelTxt.text = "LEVEL: " + Prefs.CurrentLevel;
	}

	public void SetInputCellPosition(Vector2Int pos)
    {
        if (fromPosition == Vector2Int.one * -1)
        {
            fromPosition = toPosition = pos;
            boardUIController.SetCellState(fromPosition, BoardCell.BoardCellState.ACTIVE);
        } else
        {
            if (gamePlay.CheckValidInputPositions(fromPosition, toPosition)) {
                boardUIController.SetCellsState(
                    gamePlay.GetAllPositionInRange(fromPosition, toPosition), BoardCell.BoardCellState.NORMAL);
            }
            toPosition = pos;
            if (gamePlay.CheckValidInputPositions(fromPosition, toPosition)) {
                boardUIController.SetCellsState(
                    gamePlay.GetAllPositionInRange(fromPosition, toPosition), BoardCell.BoardCellState.ACTIVE);
            } 
        }
        boardUIController.SetCellState(fromPosition, BoardCell.BoardCellState.ACTIVE);
        if (gamePlay.CheckValidInputPositions(fromPosition, toPosition)) {
            wordPreviewer.SetWord(gamePlay.GetWord(fromPosition, toPosition));
        } else {
            wordPreviewer.SetWord(gamePlay.GetWord(fromPosition, fromPosition));
        }
    }
    
    public bool HasStartPosition()
    {
        return fromPosition != Vector2Int.one * -1;
    }

    public void CheckWord()
    {
        string curWord = gamePlay.GetWord(fromPosition, toPosition);
        if (gamePlay.CheckWord(curWord)) {
            answersDisplayer.ShowAnswer(curWord);
            boardUIController.RemoveCellsAndUpdateBoard(
                gamePlay.RemoveCellsInRangeAndCollapsBoard(fromPosition, toPosition));
            if (gamePlay.HasSolvedAllWords()) {
                Prefs.HasSessionData = false;
                Prefs.CurrentLevel++;
                winDialog.SetActive(true);
            }
            gamePlay.ResetHintWord();
        }
        else
        {
            // Debug.Log("Change cell state: " + fromPosition + " - " + toPosition);
            boardUIController.SetCellsState(
                gamePlay.GetAllPositionInRange(fromPosition, toPosition), BoardCell.BoardCellState.NORMAL);
        }
        fromPosition = Vector2Int.one * -1;
        toPosition = Vector2Int.one * -1;
        wordPreviewer.ResetText();
    }
	public void LoadCurrentLevel()
	{
        gamePlay.LoadGameData();
        InitUI();
	}

    private void OnApplicationQuit() {
        Debug.Log("Has session data: " + Prefs.HasSessionData);
        if (Prefs.HasSessionData) {
            gamePlay.SaveGameSession();
        }
    }
    #region  Button Event
    public void ShuffleBoard()
    {
		char[,] charBoard = gamePlay.ShuffleBoard();
        boardUIController.Initialize(charBoard);
        gamePlay.ResetHintWord();
    }
    public void Hint() {
        if (gamePlay.HasHintWord()) {
            Vector2Int hintPosition = gamePlay.GetNextHintPosition();
            boardUIController.SetHintedCell(hintPosition);
            // boardUIController.SetCellState(hintPosition, BoardCell.BoardCellState.);
            if (gamePlay.IsHintWordCompleted()) {
                List<Vector2Int> hintWordPostions = gamePlay.GetHintWordPositions();
                fromPosition = hintWordPostions[0];
                toPosition = hintWordPostions[1];
                CheckWord();
            }
        } else {
            Debug.LogError("Can not find hint word");
            // List<Vector2Int> hintWordPostions = gamePlay.GetHintWordPositions();
            // fromPosition = hintWordPostions[0];
            // toPosition = hintWordPostions[1];
            // CheckWord();
            // boardUIController.RemoveCellsAndUpdateBoard(
            //     gamePlay.RemoveCellsInRangeAndCollapsBoard(hintWordPostions[0], hintWordPostions[1]));
            // if (gamePlay.HasSolvedAllWords()) {
            //     Prefs.HasSessionData = false;
            //     Prefs.CurrentLevel++;
            //     winDialog.SetActive(true);
            // }
        }
    }
    #endregion
}
