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
    WordStackGamePlay gamePlay;
    bool isAnimEnded = false; 
    bool hasWon = false;

    public bool IsAnimEnded{
        get => isAnimEnded;
        set => isAnimEnded = value;
    }
    

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
    }
    private void Start()
	{
        hasWon = false;
		gamePlay = new WordStackGamePlay();
		InitUI();
	}

	private void InitUI()
	{
        isAnimEnded = false;

		char[,] charBoard = gamePlay.GetCharBoard();
        boardUIController.Initialize(charBoard);

		wordPreviewer.ResetText();
        answersDisplayer.SetWordAnswers(gamePlay.GetTargetWords());
        answersDisplayer.ShowAnswer(gamePlay.GetSolvedWords());
		levelTxt.text = "LEVEL: " + Prefs.CurrentLevel;

        GameSessionData sessionData = gamePlay.GetGameSessionData();
        gamePlay.SetHintWord(sessionData.HintWord);
	}
    public void ShowHintedCells() {
        isAnimEnded = true;
        GameSessionData sessionData = gamePlay.GetGameSessionData();
        if (sessionData.HintWord.HasWordInfo()) {
            // Debug.Log("Start pos: " + sessionData.HintWord.GetStartPosition());
            // Debug.Log("Cur pos: " + sessionData.HintWord.GetCurrentPosition());
            // Debug.Log("--------------------");
            // foreach (var p in pos) {
            //     Debug.Log(p);
            // }
            // Debug.Log("--------------------");
            var pos = gamePlay.GetAllPositionsInRange(sessionData.HintWord.GetStartPosition(), sessionData.HintWord.GetCurrentPosition()); 
            boardUIController.SetHintedCells(
                gamePlay.GetAllPositionsInRange(sessionData.HintWord.GetStartPosition(), sessionData.HintWord.GetCurrentPosition())
            );
        }
    }

	public void SetInputCellPosition(Vector2Int pos)
    {
        if (!isAnimEnded) return;
        if (fromPosition == Vector2Int.one * -1)
        {
            fromPosition = toPosition = lastValidPosition = pos;
            boardUIController.SetCellState(fromPosition, BoardCell.BoardCellState.ACTIVE);
        } else
        {
            if (gamePlay.VerityInputPositions(fromPosition, pos)) {
                if (Utilities.IsInside(pos, fromPosition, lastValidPosition)) {
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(lastValidPosition, Utilities.GetPreLastPosition(lastValidPosition, pos)), BoardCell.BoardCellState.NORMAL);
                } else {
                    boardUIController.SetCellsState(
                        gamePlay.GetAllPositionsInRange(Utilities.GetNextBeginPosition(lastValidPosition, pos), pos), BoardCell.BoardCellState.NORMAL);
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
                gamePlay.ResetHintWord();
            } 
            // else if (gamePlay. && gamePlay.FindHintWord(new List<string>(){gamePlay.GetHintWord()}) != null) {
            //     gamePlay.UpdateHintWordInfo();
            // }
            if (gamePlay.GetHintWordInfo() == null) {
                Debug.LogError("Hint word info is null in game controller");
            }
            if (gamePlay.HasSolvedAllWords()) {
                hasWon = true;
                Prefs.HasSessionData = false;
                Prefs.CurrentLevel++;
            }
            List<Vector2Int> positionsInBoard = gamePlay.GetAllPositionsInRange(fromPosition, toPosition);
            CellSteps steps = gamePlay.RemoveCellsInRangeAndCollapsBoard(fromPosition, toPosition);
            //Debug.Log("-- from: " + fromPosition + " to: " + toPosition + " posInBoard: " + positionsInBoard.Count);
            floatingWord.MoveWord(curWord, boardUIController.GetCellSize(), new Vector2(50, 50), 
                boardUIController.GetCellWorldPosition(positionsInBoard), answersDisplayer.GetLetterPositions(curWord), () => {
                answersDisplayer.ShowAnswer(curWord);
                if (gamePlay.HasSolvedAllWords()) {
                    StartCoroutine(DelayActivateWinDialog());
                }
            });
            boardUIController.RemoveCellsAndCollapseBoard(
                steps, 
                () => {
                    isAnimEnded = true;
                    Debug.LogError("Anim ended true");
                    if (!hasWon) {
                        if (!gamePlay.CheckIfBoardValid()) {
                            Debug.Log("Board is invaliddddddddddddddddddddddddddd");
                            ShuffleBoard();
                        }
                    }
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

    IEnumerator DelayActivateWinDialog() {
        yield return new WaitForSeconds(.5f);
        winDialog.SetActive(true);
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
        isAnimEnded = true;
        gamePlay.ResetHintWord();
        List<MoveInfo> moveInfos = gamePlay.ShuffleBoard();
        boardUIController.ShuffleBoard(moveInfos, gamePlay.GetBoardWidth(), gamePlay.GetBoardHeight());
    }

    public void Hint() {
        if(hasWon || !isAnimEnded) return;
        Debug.LogError("Hint");
        Vector2Int hintPosition = gamePlay.GetNextHintPosition();
        boardUIController.SetHintedCell(hintPosition);
        if (gamePlay.IsHintWordCompleted()) {
            List<Vector2Int> hintWordPostions = gamePlay.GetHintWordEndPositions();
            fromPosition = hintWordPostions[0];
            toPosition = hintWordPostions[1];
            StartCoroutine(DelayAndCheckWord());
        }
    }
    IEnumerator DelayAndCheckWord() {
        yield return new WaitForSeconds(.2f);
        CheckWord();
    }
    #endregion
}
