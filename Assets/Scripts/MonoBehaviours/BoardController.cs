using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardController : MonoBehaviour 
{
    [SerializeField] BoardCell cellPrefab;
    [SerializeField] bool run = true;
    LevelData data;
    BoardCell[,] board;
    float boardWidth, boardHeight;
    RectTransform rectTrans;
    float cellSize;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
    }

    public void Init(LevelData data)
    {
        this.data = data;
        boardWidth = rectTrans.rect.width;
        boardHeight = rectTrans.rect.height;
        cellSize = boardWidth / data.NumCols();
        board = new BoardCell[data.NumRows(), data.NumCols()];
        GenerateBoard();
    }

    void GenerateBoard()
    {
        Debug.Log("Generate board");
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (data.ContainLetter(i, j))
                {
                    BoardCell newCell = Instantiate(cellPrefab, transform);
                    newCell.SetLetter(data.GetLetter(i, j));
                    newCell.SetPositionInBoard(new Vector2Int(i, j));
                    RectTransform newCellRectTrans = newCell.GetComponent<RectTransform>();
                    newCellRectTrans.anchorMin = Vector2.zero;
                    newCellRectTrans.anchorMax = Vector2.zero;
                    newCellRectTrans.pivot = Vector2.zero;
                    newCellRectTrans.anchoredPosition = new Vector2(j * cellSize, i * cellSize);
                    board[i, j] = newCell;
                } 
            }
        }
    }
    public char GetLetter(int x, int y)
    {
        return board[x, y].GetLetter(); 
    }
    public string GetWord(Vector2Int fromPos, Vector2Int toPos)
    {
        string res = "";
        if (fromPos.x == toPos.x)
        {
            if (fromPos.y > toPos.y)
            {
                for (int i = fromPos.y; i >= toPos.y; i--)
                {
                    res += board[fromPos.x, i].GetLetter();
                }
            } else
            {
                for (int i = fromPos.y; i <= toPos.y; i++)
                {
                    res += board[fromPos.x, i].GetLetter();
                }
            }
        } else if (fromPos.y == toPos.y)
        {
            if (fromPos.x > toPos.x)
            {
                for (int i = fromPos.x; i >= toPos.x; i--)
                {
                    res += board[i, fromPos.y].GetLetter();
                }
            } else
            {
                for (int i = fromPos.x; i <= toPos.x; i++)
                {
                    res += board[i, fromPos.y].GetLetter();
                }
            }
        }
        return res;
    }
    // Get cells lying in horizontal or vertical line
    public List<BoardCell> GetCells(int x1, int y1, int x2, int y2)
    {

        List<BoardCell> res = new List<BoardCell>();
        if (x1 == x2)
        {
            for (int i = y1; i <= y2; i++)
            {
                res.Add(board[x1, i]);
            }
        } else
        {
            for (int i = x1; i <= x2; i++)
            {
                res.Add(board[i, y1]);
            }
        }
        return res;
    }

    public void ChangeCellsColor(Vector2Int startPosition, Vector2Int endPosition, bool colorState = true)
    {
        //Debug.LogError("Start: " + startPosition + " end: " + endPosition + " state: " + colorState);
        if (startPosition == endPosition) board[startPosition.x, startPosition.y].ChangeColor(colorState);
        if (startPosition.x == endPosition.x)
        {
            for (int i = Mathf.Min(startPosition.y, endPosition.y); i <= Mathf.Max(startPosition.y, endPosition.y); i++)
            {
                if (board[startPosition.x, i])
                {
                    board[startPosition.x, i].ChangeColor(colorState);
                } else
                {
                    Debug.Log("1 positin is null in board: " + startPosition.x + " - " + i);
                }
            }
        } else if (startPosition.y == endPosition.y)
        {
            for (int i = Mathf.Min(startPosition.x, endPosition.x); i <= Mathf.Max(startPosition.x, endPosition.x); i++)
            {
                if (board[i, startPosition.y])
                {
                    board[i, startPosition.y].ChangeColor(colorState); 
                } else
                {
                    Debug.Log("2 positin is null in board: " + i + " - " + startPosition.y);
                }
            }
        } else if (!colorState)
        {
            for (int i = 0; i < board.GetLength(1); i++)
            {
                if (board[startPosition.x, i])
                {
                    board[startPosition.x, i].ChangeColor(colorState);
                }
                else break;
            }
            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (board[i, startPosition.y])
                {
                    board[i, startPosition.y].ChangeColor(colorState);
                }
                else break;
            }
        }
    }

    public void RemoveCells(Vector2Int startPosition, Vector2Int endPosition)
    {
        if (startPosition.x == endPosition.x)
        {
            for (int i = startPosition.y; i <= endPosition.y; i++)
            {
                BoardCell curCell = board[startPosition.x, i];
                for (int j = startPosition.x; j < board.GetLength(0) - 1; j++)
                {
                    if (board[j + 1, i] == null)
                    {
                        break;
                    }
                    board[j, i] = board[j + 1, i];
                    board[j, i].SetPositionInBoard(new Vector2Int(j, i));
                }
                for (int j = startPosition.x; j < board.GetLength(0) - 1; j++)
                {
                    if (board[j + 1, i] == null)
                    {
                        break;
                    }
                    Vector2Int positionInBoard = board[j, i].GetPositionInBoard();
                    board[j, i].GetComponent<RectTransform>().anchoredPosition = new Vector2(positionInBoard.y * cellSize, positionInBoard.x * cellSize);
                }
                Destroy(curCell.gameObject);
            }
        } else
        {
            int lowerX = Mathf.Min(startPosition.x, endPosition.x);
            int upperX = Mathf.Max(startPosition.x, endPosition.x);
            int commonY = startPosition.y;
            int gap = upperX - lowerX + 1;
            for (int i = lowerX; i <= upperX; i++)
            {
                BoardCell curCell = board[i, commonY];
                //Vector2Int positionInBoard = curCell.GetPositionInBoard();
                if (i + gap < board.GetLength(0) && board[i + gap, commonY] != null)
                {
                    board[i + gap, commonY].GetComponent<RectTransform>().anchoredPosition = 
                        board[i, commonY].GetComponent<RectTransform>().anchoredPosition;
                    board[i, commonY] = board[i + gap, commonY];
                    board[i, commonY].SetPositionInBoard(new Vector2Int(i, commonY));
                } else
                {
                    board[i, commonY] = null;
                }
                Destroy(curCell.gameObject);
            }
        }
    }

    //public void ResetValidWordColor(S)
    public bool IsValidPositions(Vector2Int startPosition, Vector2Int endPosition)
    {
        if (startPosition == endPosition) return true;
        if (startPosition.x != endPosition.x && startPosition.y != endPosition.y)
        {
            return false;
        } else if (startPosition.x == endPosition.x)
        {
            for (int i = startPosition.y; i <= endPosition.y; i++)
            {
                if (board[startPosition.x, i] == null) return false;
            }
        } else
        {
            for (int i = startPosition.x; i <= endPosition.x; i++)
            {
                if (board[i, startPosition.y] == null) return false;
            }
        }
        return true;
    }
}
