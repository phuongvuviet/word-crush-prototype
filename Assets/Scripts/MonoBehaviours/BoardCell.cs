using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler 
{
    [SerializeField] Text letterText;
    [SerializeField] Color activeColor, normalColor, hintColor;
    [SerializeField] Image bgImage;

    float cellSize;
    float cellMargin;
    [SerializeField] Vector2Int positionInBoard;
    [SerializeField] char letter = ' ';
    bool isPointerDown = false;
    BoardCellState curState = BoardCellState.NORMAL;

    public enum BoardCellState{
        NORMAL,
        ACTIVE,
        HINT
    }

    public void UpdateAnchoredPosition()
    {
        //Debug.Log(positionInBoard.x + "-" + positionInBoard.y);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(positionInBoard.y, positionInBoard.x) * cellSize;
    }

    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
        this.letter = letter;
    }
    public char GetLetter()
    {
        return letter;
    }
    public void SetPositionInBoard(Vector2Int pos)
    {
        //Debug.Log($"Update {positionInBoard.x}-{positionInBoard.y} -> {pos.x}-{pos.y}");
        positionInBoard = pos;
    }
    public Vector2Int GetPositionInBoard()
    {
        return positionInBoard;
    }
    public void SetCellSizeAndMargin(float size, float margin)
    {
        cellSize = size;
        cellMargin = margin;
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize - cellMargin);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize - cellMargin);
    }
    public void SetState(BoardCellState state) {
        // this.curState = state;
        switch (state)
        {   
            case BoardCellState.ACTIVE:
                bgImage.color = activeColor;
                break;
            case BoardCellState.NORMAL:
                if (curState == BoardCellState.HINT) {
                    bgImage.color = hintColor;
                } else {
                    bgImage.color = normalColor;
                }
                break;
            case BoardCellState.HINT:
                bgImage.color = hintColor;
                break;
        }
        this.curState = state;
    }
    public BoardCellState GetState() {
        return curState;
    }
    public void ChangeColor(bool useActiveColor)
    {
        if (useActiveColor)
        {
            bgImage.color = activeColor;
        } else
        {
            bgImage.color = normalColor;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.HasStartPosition())
        {
            GameController.Instance.SetWordPosition(positionInBoard);
        } 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.Instance.HasStartPosition())
        {
            GameController.Instance.SetWordPosition(positionInBoard);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameController.Instance.CheckWord();
    }
}
