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
    [SerializeField] FloatingWordController floatingWord;
    [SerializeField] GameObject winDialog; 
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI questionTxt;

    public static GameController Instance;

    Vector2Int fromPosition = Vector2Int.one * -1, toPosition = Vector2Int.one * -1, lastValidPosition = Vector2Int.one * -1;
    HintWordInfo hintWordInfo = null;
    WordStackGamePlay gamePlay;
    string curAns = "";
    bool isAnimEnded = false; 
    bool hasWon = false;
    

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
    }
    private void Start()
	{
		gamePlay = new WordStackGamePlay();
		InitUI();
        hasWon = false;
        isAnimEnded = true;
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
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(lastValidPosition, Utility.GetPreLastPosition(lastValidPosition, pos)), BoardCell.BoardCellState.NORMAL);
                } else {
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(Utility.GetNextPosition(lastValidPosition, pos), pos), BoardCell.BoardCellState.NORMAL);
                }
                boardUIController.SetCellsState(
                    gamePlay.GetAllPositionsInRange(fromPosition, pos), BoardCell.BoardCellState.ACTIVE);
                lastValidPosition = pos;
                toPosition = pos;
            } else {
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
            isAnimEnded = false;
            if (gamePlay.GetHintWord() == curWord) {
                gamePlay.SetHintWord(null);
            } else if (gamePlay.GetHintWordInfo() != null && gamePlay.FindHintWord(new List<string>(){gamePlay.GetHintWord()}) != null) {
                gamePlay.UpdateHintWordInfo();
            }
            if (gamePlay.GetHintWordInfo() == null) {
                Debug.LogError("Hint word info is null in game controller");
            }
            if (gamePlay.HasSolvedAllWords()) {
                hasWon = true;
                Prefs.HasSessionData = false;
                Prefs.CurrentLevel++;
            }
            List<Vector2Int> positionsInBoard = gamePlay.GetAllPositionsInRange(fromPosition, toPosition);
            floatingWord.MoveWord(curWord, boardUIController.GetCellSize(), new Vector2(50, 50), 
                boardUIController.GetCellWorldPosition(positionsInBoard), answersDisplayer.GetLetterPositions(curWord), () => {
                answersDisplayer.ShowAnswer(curWord);
                if (gamePlay.HasSolvedAllWords()) {
                    winDialog.SetActive(true);
                }
            });
            boardUIController.RemoveCellsAndCollapseBoard(
                gamePlay.RemoveCellsInRangeAndCollapsBoard(fromPosition, toPosition), 
                () => {
                    isAnimEnded = true;
                });
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
        hasWon = false;
        isAnimEnded = true;
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
        if (hasWon || !isAnimEnded) return;
		char[,] charBoard = gamePlay.ShuffleBoard();
        boardUIController.Initialize(charBoard);
        gamePlay.SetHintWord(null);
    }

    public void Hint() {
        if(hasWon || !isAnimEnded) return;
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
            gamePlay.SetHintWord(null);
            Debug.LogError("Can not find hint word");
        }
    }
    #endregion
}
