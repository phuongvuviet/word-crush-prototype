using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BoardCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler 
{
    [SerializeField] Text letterText;
    [SerializeField] Color activeColor, normalColor, hintColor;
    [SerializeField] Image bgImage;

    float cellSize;
    float cellMargin;
    Vector2Int positionInBoard = Vector2Int.one * -1;
    char letter = ' ';
    bool isPointerDown = false;
    RectTransform rectTransform;
    BoardCellState curState = BoardCellState.NORMAL;
    bool isHinted = false;

    public bool IsHinted{
        get{
            return isHinted;
        }
        set {
            isHinted = value;
            bgImage.color = hintColor;
        }
    }

    public enum BoardCellState{
        NORMAL,
        ACTIVE
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
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
        if (positionInBoard != Vector2Int.one * -1) {
            rectTransform.DOAnchorPos(new Vector2(pos.y, pos.x) * cellSize + Vector2.one * cellSize / 2f, .2f);
        }
        positionInBoard = pos;
    }
    public void SetPositionInBoardWhenStartGame(Vector2Int pos) {
        positionInBoard = pos;
        rectTransform.DOAnchorPos(new Vector2(pos.y, pos.x) * cellSize + Vector2.one * cellSize / 2f, .2f);
    }
    public Vector2Int GetPositionInBoard()
    {
        return positionInBoard;
    }
    public void SetCellSizeAndMargin(float size, float margin)
    {
        cellSize = size;
        cellMargin = margin;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize - cellMargin);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize - cellMargin);
    }
    public void SetState(BoardCellState state, bool useAnim = true) {
        switch (state)
        {   
            case BoardCellState.ACTIVE:
                // Debug.Log("Active");
                if (curState == BoardCellState.NORMAL) {
                    bgImage.color = activeColor;
                    if (useAnim) {
                        transform.SetAsLastSibling();
                        transform.DOScale(1.05f, .2f).OnComplete(() => {
                            transform.DOScale(1f, .1f);
                        });
                        // transform.DOScale(.9f, .1f).OnComplete(() => {
                        // });
                    }
                } 
                break;
            case BoardCellState.NORMAL:
                // Debug.Log("Normal");
                if (IsHinted) {
                    bgImage.color = hintColor;
                } else {
                    bgImage.color = normalColor;
                }
                break;
        }
        this.curState = state;
    }
    public BoardCellState GetState() {
        return curState;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.HasStartPosition())
        {
            // transform.DOScale(1.1f, .2f).OnComplete(() => {
            //     transform.DOScale(1f, .15f);
            // });
            GameController.Instance.SetInputCellPosition(positionInBoard);
        } 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.Instance.HasStartPosition())
        {
            // transform.DOScale(1.2f, .2f).OnComplete(() => {
            //     transform.DOScale(1f, .15f);
            // });
            GameController.Instance.SetInputCellPosition(positionInBoard);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameController.Instance.CheckWord();
    }
}
