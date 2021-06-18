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

    Vector2Int fromPosition = Vector2Int.one * -1, toPosition = Vector2Int.one * -1, lastValidPosition = Vector2Int.one * -1;
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

        GameSessionData sessionData = gamePlay.GetGameSessionData();
        if (sessionData.HintWord != null && sessionData.HintWord.GetStartPosition() != Vector2Int.one * -1) {
            // Debug.Log("Inside");
            gamePlay.SetHintWord(sessionData.HintWord);
            boardUIController.SetHintedCells(
                gamePlay.GetAllPositionsInRange(sessionData.HintWord.GetStartPosition(), sessionData.HintWord.GetCurrentPosition()));
        }
	}

	public void SetInputCellPosition(Vector2Int pos)
    {
        if (fromPosition == Vector2Int.one * -1)
        {
            fromPosition = toPosition = lastValidPosition = pos;
            boardUIController.SetCellState(fromPosition, BoardCell.BoardCellState.ACTIVE);
        } else
        {
            if (gamePlay.VerityInputPositions(fromPosition, pos)) {
                if (Utility.IsInside(pos, fromPosition, lastValidPosition)) {
                    // Debug.Log("Inside");
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(lastValidPosition, Utility.GetPreLastPosition(lastValidPosition, pos)), BoardCell.BoardCellState.NORMAL);
                } else {
                    // Debug.Log("Nottt Inside");
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(Utility.GetNextPosition(lastValidPosition, pos), pos), BoardCell.BoardCellState.NORMAL);
                }
                // Debug.Log("1");
                // Vector2Int nextLastValidPosition = Utility.GetNextPosition(lastValidPosition, pos);
                boardUIController.SetCellsState(
                    gamePlay.GetAllPositionsInRange(fromPosition, pos), BoardCell.BoardCellState.ACTIVE);
                lastValidPosition = pos;
                toPosition = pos;
            } else {
                // Debug.Log("2");
                boardUIController.SetCellsState(
                    gamePlay.GetAllPositionsInRange(fromPosition, lastValidPosition), BoardCell.BoardCellState.NORMAL);
                toPosition = pos;
            }
        }
        toPosition = pos;
        boardUIController.SetCellState(fromPosition, BoardCell.BoardCellState.ACTIVE, false);
        if (gamePlay.VerityInputPositions(fromPosition, toPosition)) {
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
            boardUIController.RemoveCellsAndCollapseBoard(
                gamePlay.RemoveCellsInRangeAndCollapsBoard(fromPosition, toPosition));
            if (gamePlay.HasSolvedAllWords()) {
                Prefs.HasSessionData = false;
                Prefs.CurrentLevel++;
                winDialog.SetActive(true);
            }
            gamePlay.SetHintWord(null);
        }
        else
        {
            // Debug.Log("Change cell state: " + fromPosition + " - " + toPosition);
            boardUIController.SetCellsState(
                gamePlay.GetAllPositionsInRange(fromPosition, toPosition), BoardCell.BoardCellState.NORMAL);
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
        // Debug.Log("Has session data: " + Prefs.HasSessionData);
        if (Prefs.HasSessionData) {
            gamePlay.SaveGameSession();
        }
    }
    #region  Button Event
    public void ShuffleBoard()
    {
		char[,] charBoard = gamePlay.ShuffleBoard();
        boardUIController.Initialize(charBoard);
        gamePlay.SetHintWord(null);
    }
    public void Hint() {
        if (!gamePlay.IsHintWordCompleted()) {
            Vector2Int hintPosition = gamePlay.GetNextHintPosition();
            boardUIController.SetHintedCell(hintPosition);
            // boardUIController.SetCellState(hintPosition, BoardCell.BoardCellState.);
            if (gamePlay.IsHintWordCompleted()) {
                List<Vector2Int> hintWordPostions = gamePlay.GetHintWordEndPositions();
                fromPosition = hintWordPostions[0];
                toPosition = hintWordPostions[1];
                CheckWord();
            }
        } else {
            Debug.LogError("Can not find hint word");
        }
    }
    #endregion
}
