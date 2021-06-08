using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour
{
    [SerializeField] LevelData data;
    [SerializeField] BoardUIController boardUIController;
    [SerializeField] WordPreviewer wordPreviewer;
    [SerializeField] WordAnswersDisplayer answersDisplayer; 

    public static GamePlayController Instance;

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
        boardUIController.Initialize(data);
        wordPreviewer.ResetText();
        answers = data.GetWords();
        answersDisplayer.SetWordAnswers(data.GetWords());

    }
    public void SetWordPosition(Vector2Int pos)
    {
        if (startPosition == Vector2Int.one * -1)
        {
            startPosition = pos;
            wordPreviewer.SetWord(boardUIController.GetLetter(pos.x, pos.y).ToString());
            boardUIController.ChangeCellsColor(startPosition, startPosition);
        } else
        {
            SetEndPostion(pos);
        }
    }

    public void SetEndPostion(Vector2Int pos)
    {
        boardUIController.ChangeCellsColor(startPosition, endPosition, false);
        endPosition = pos;
        boardUIController.ChangeCellsColor(startPosition, endPosition, true);
        wordPreviewer.SetWord(boardUIController.GetWord(startPosition, endPosition));
    }
    
    public bool HasStartPosition()
    {
        return startPosition != Vector2Int.one * -1;
    }
    public void CheckWord()
    {
        if (endPosition == Vector2Int.one * -1) endPosition = startPosition;

        if (boardUIController == null) Debug.Log("Board ui controller is null");
        string curAns = boardUIController.GetWord(startPosition, endPosition);
        if (curAns == null) Debug.Log("cur ans is null");
        // Debug.Log("answer count: " + answers.Count);
        if (answers.Contains(curAns))
        {
            answersDisplayer.ShowAnswer(curAns);
            boardUIController.RemoveCellsAndUpdateBoard(startPosition, endPosition);
        }
        else
        {
            boardUIController.ChangeCellsColor(startPosition, endPosition, false);
        }
        startPosition = Vector2Int.one * -1;
        endPosition = Vector2Int.one * -1;
        wordPreviewer.ResetText();
    }
    public void ShuffleBoard()
    {
        boardUIController.ShuffleBoard(data);
    }
}
