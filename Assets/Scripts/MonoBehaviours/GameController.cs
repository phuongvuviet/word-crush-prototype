using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    [SerializeField] BoardUIController boardUIController;
    [SerializeField] WordPreviewer wordPreviewer;
	[SerializeField] WordAnswersDisplayer answersDisplayer; 
    [SerializeField] FloatingWordController floatingWord;
    [SerializeField] WinDialog winDialog; 
    [SerializeField] TextMeshProUGUI levelTxt, subjectTxt;
    [SerializeField] TextMeshProUGUI startSubjectTxt;
    [SerializeField] CanvasGroup boosterButtonsCG;
    [SerializeField] GameObject endingWordsBg; 
    [SerializeField] RectTransform endingAnswerParent; 

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
        StartCoroutine(InitUI());
	}

	private IEnumerator InitUI()
	{
        endingAnswerParent.anchoredPosition = Vector2.zero;
        endingWordsBg.gameObject.SetActive(false);
        GameSessionData sessionData = gamePlay.GetGameSessionData();
        startSubjectTxt.gameObject.SetActive(true);
        boardUIController.gameObject.SetActive(false);
        // answersDisplayer.gameObject.SetActive(false);
        answersDisplayer.HideAnswerWords();
        answersDisplayer.HideEndingWords();
		wordPreviewer.ResetText();
        startSubjectTxt.text = sessionData.LevelData.Subject;
        startSubjectTxt.transform.DOScale(1.2f, .3f).OnComplete(() => {
            startSubjectTxt.transform.DOScale(1.0f, .3f);
        });
		levelTxt.text = "LEVEL: " + Prefs.CurrentLevel;
        subjectTxt.gameObject.SetActive(false); 
        boosterButtonsCG.alpha = 0f;
        boosterButtonsCG.interactable = false;
        yield return new WaitForSeconds(1.5f);
        isAnimEnded = false;
        startSubjectTxt.gameObject.SetActive(false);

		char[,] charBoard = gamePlay.GetCharBoard();
        boardUIController.gameObject.SetActive(true);
        boardUIController.Initialize(charBoard, ShowUIAfterGeneratingBoard);
	}
    void ShowUIAfterGeneratingBoard() {
        StartCoroutine(COShowUIAfterGeneratingBoard());
    }
    IEnumerator COShowUIAfterGeneratingBoard() {
        Debug.Log("Generate board done");
        subjectTxt.gameObject.SetActive(true); 
        subjectTxt.text = gamePlay.GetGameSessionData().LevelData.Subject;
        boosterButtonsCG.alpha = 1f;
        boosterButtonsCG.interactable = true;
        yield return new WaitForSeconds(.1f);
        // answersDisplayer.gameObject.SetActive(true);
        answersDisplayer.ShowAnswerWords();
        answersDisplayer.ShowEndingWords();
        boardUIController.gameObject.SetActive(true);
        answersDisplayer.SetWordAnswers(gamePlay.GetTargetWords());
        answersDisplayer.ShowAnswers(gamePlay.GetSolvedWords());
        answersDisplayer.transform.DOScale(1.1f, .1f).OnComplete(() => {
            answersDisplayer.transform.DOScale(1.0f, .1f);
        });
        gamePlay.SetHintWord(gamePlay.GetGameSessionData().HintWord);
    }

    public void ShowHintedCells() {
        isAnimEnded = true;
        GameSessionData sessionData = gamePlay.GetGameSessionData();
        if (sessionData.HintWord.HasWordInfo()) {
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
                boardUIController.GetCellWorldPositions(positionsInBoard), answersDisplayer.GetLetterPositions(curWord), () => {
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
        wordPreviewer.ResetText();
        answersDisplayer.HideAnswerWords();
        subjectTxt.gameObject.SetActive(false);
        endingWordsBg.gameObject.SetActive(true);
        boosterButtonsCG.alpha = 0f;
        boosterButtonsCG.interactable = false;
        yield return endingAnswerParent.DOAnchorPosY(-200, 1f);
        winDialog.gameObject.SetActive(true);
    }
	public void LoadCurrentLevel()
	{
        gamePlay.LoadGameData();
        StartCoroutine(InitUI());
        hasWon = false;
        isAnimEnded = true;
	}

    private void OnApplicationQuit() {
        // Debug.Log("Has session data: " + Prefs.HasSessionData);
        if (Prefs.HasSessionData) {
            gamePlay.SaveGameSession();
        }
    }
    #region  Booster Button Event
    public void ShuffleBoard()
    {
        if (hasWon || !isAnimEnded) return;
        isAnimEnded = true;
        Debug.Log("Shuffle");
        // gamePlay.ResetHintWord();
        if (!gamePlay.IsHintWordCompleted()) {
            List<Vector2Int> hintEndPositions = gamePlay.GetHintWordEndPositions();
            Debug.Log("Hint end position: " + hintEndPositions[0] + " " + hintEndPositions[1]);
            boardUIController.UnhintCells(gamePlay.GetAllPositionsInRange(hintEndPositions[0], hintEndPositions[1]));
        }  
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
