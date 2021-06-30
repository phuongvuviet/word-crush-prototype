using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BoardCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler 
{
    [SerializeField] TextMeshProUGUI letterText;
    [SerializeField] Color activeColor, normalColor, hintColor;
    [SerializeField] Image bgImage;

    float cellSize;
    float cellMargin;
    Vector2Int positionInBoard = Vector2Int.one * -1;
    char letter = ' ';
    RectTransform rectTransform;
    BoardCellState curState = BoardCellState.NORMAL;
    bool isHinted = false;

    public bool IsHinted{
        get{
            return isHinted;
        }
        set {
            isHinted = value;
            if (isHinted) {
                bgImage.color = hintColor;
            } else {
                bgImage.color = normalColor;
            }
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
    public void SetPositionInBoard(Vector2Int pos, float time = .2f)
    {
        if (positionInBoard != Vector2Int.one * -1) {
            rectTransform.DOAnchorPos(new Vector2(pos.y, pos.x) * cellSize + Vector2.one * cellSize / 2f, time);
        }
        positionInBoard = pos;
    }
    public void SetPositionInBoardWhenStartGame(Vector2Int pos) {
        positionInBoard = pos;
        rectTransform.DOAnchorPos(new Vector2(pos.y, pos.x) * cellSize + Vector2.one * cellSize / 2f, .15f);
    }
    public Vector2Int GetPositionInBoard()
    {
        return positionInBoard;
    }
    public void SetCellSizeAndMargin(float size, float margin)
    {
        cellSize = size;
        cellMargin = margin;

        // rectTransform.DOSizeDelta(new Vector2(cellSize - cellMargin, cellSize - cellMargin), .1f);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize - cellMargin);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize - cellMargin);
    }
    public void SetState(BoardCellState state, bool useAnim = true) {
        switch (state)
        {   
            case BoardCellState.ACTIVE:
                if (curState == BoardCellState.NORMAL) {
                    bgImage.color = activeColor;
                    if (useAnim) {
                        transform.SetAsLastSibling();
                        transform.DOScale(1.05f, .2f).OnComplete(() => {
                            transform.DOScale(1f, .1f);
                        });
                    }
                } 
                break;
            case BoardCellState.NORMAL:
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
            GameController.Instance.SetInputCellPosition(positionInBoard);
        } 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.Instance.HasStartPosition())
        {
            GameController.Instance.SetInputCellPosition(positionInBoard);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameController.Instance.CheckWord();
    }
}
