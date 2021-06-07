using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler 
{
    [SerializeField] Text letterText;
    [SerializeField] Color activeColor, normalColor;
    [SerializeField] Image bgImage;

    float cellSize;
    float cellMargin;
    [SerializeField] Vector2Int positionInBoard;
    char letter = ' ';
    bool isPointerDown = false;

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
        if (GamePlayController.Instance.HasStartPosition())
        {
            GamePlayController.Instance.SetWordPosition(positionInBoard);
        } 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GamePlayController.Instance.HasStartPosition())
        {
            GamePlayController.Instance.SetWordPosition(positionInBoard);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GamePlayController.Instance.CheckWord();
    }
}
