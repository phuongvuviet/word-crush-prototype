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

    Vector2Int positionInBoard;
    char letter = ' ';
    bool isPointerDown = false;

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
        positionInBoard = pos;
    }
    public Vector2Int GetPositionInBoard()
    {
        return positionInBoard;
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
