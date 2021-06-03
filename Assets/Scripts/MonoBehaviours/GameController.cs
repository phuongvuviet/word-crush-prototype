using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] LevelData data;
    [SerializeField] BoardController boardController;
    [SerializeField] WordPreviewer wordPreviewer;
    [SerializeField] WordAnswersDisplayer answersDisplayer; 

    public static GameController Instance;

    Vector2Int startPosition = Vector2Int.one * -1, endPosition = Vector2Int.one * -1;
    List<string> answers;
    string curAns = "";
    bool isCurAnsWrong = false;

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Instance = this;
    }

    private void Start()
    {
        boardController.Init(data);
        wordPreviewer.ResetText();
        answers = data.GetWords();
        answersDisplayer.SetWordAnswers(data.GetWords());

    }
    public void SetWordPosition(Vector2Int pos)
    {
        if (startPosition == Vector2Int.one * -1)
        {
            startPosition = pos;
            wordPreviewer.SetWord(boardController.GetLetter(pos.x, pos.y).ToString());
            boardController.ChangeCellsColor(startPosition, startPosition);
        } else
        {
            SetEndPostion(pos);
        }
    }

    public void SetEndPostion(Vector2Int pos)
    {
        boardController.ChangeCellsColor(startPosition, endPosition, false);
        endPosition = pos;
        boardController.ChangeCellsColor(startPosition, endPosition, true);
        wordPreviewer.SetWord(boardController.GetWord(startPosition, endPosition));
    }
    
    public bool HasStartPosition()
    {
        return startPosition != Vector2Int.one * -1;
    }
    public void CheckWord()
    {
        if (endPosition == Vector2Int.one * -1) endPosition = startPosition;

        string curAns = boardController.GetWord(startPosition, endPosition);
        if (answers.Contains(curAns))
        {
            answersDisplayer.ShowAnswer(curAns);
            boardController.RemoveCells(startPosition, endPosition);
        } else
        {
            boardController.ChangeCellsColor(startPosition, endPosition, false);
        }
        startPosition = Vector2Int.one * -1;
        endPosition = Vector2Int.one * -1;
        wordPreviewer.ResetText();
    }
}
