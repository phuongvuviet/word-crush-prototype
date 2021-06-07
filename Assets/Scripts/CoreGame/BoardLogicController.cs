using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLogicController 
{
    BoardCell[,] board;
    int cols, rows;
    public BoardLogicController(BoardCell[,] boardParam)
    {
        board = boardParam;
        cols = board.GetLength(1);
        rows = board.GetLength(0);
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
    public List<Vector2Int> GetVerticalAndHorizontalCellsFromCell(Vector2Int cellPos)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int col = 0; col < cols; col++)
        {
            if (board[cellPos.x, col])
            {
                cells.Add(new Vector2Int(cellPos.x, col));
            }
            else break;
        }
        for (int row = 0; row < rows; row++)
        {
            if (board[row, cellPos.y])
            {
                cells.Add(new Vector2Int(row, cellPos.y));
            }
            else break;
        }
        return cells;
    }
    public List<Vector2Int> GetCells(Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Vector2Int> cells = new List<Vector2Int>(); 
        if (startPosition.x == endPosition.x)
        {
            for (int i = Mathf.Min(startPosition.y, endPosition.y); i <= Mathf.Max(startPosition.y, endPosition.y); i++)
            {
                cells.Add(new Vector2Int(startPosition.x, i));
            }
        } else if (startPosition.y == endPosition.y)
        {
            for (int i = Mathf.Min(startPosition.x, endPosition.x); i <= Mathf.Max(startPosition.x, endPosition.x); i++)
            {
                cells.Add(new Vector2Int(i, startPosition.y));
            }
        } 
        return cells;
    }

    //public void ResetValidWordColor(S)
    public bool IsValidEndPoints(Vector2Int startPosition, Vector2Int endPosition)
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
    public List<BoardCell> GetCellsToRemoveAndUpdateOtherCellsPosition(Vector2Int startPosition, Vector2Int endPosition)
    {
        // Debug.Log("Remove cell: " + startPosition + " to: " + endPosition);
        // Debug.Log("Col: " + cols + " rows: " + rows);
        List<BoardCell> cellsToRemove = new List<BoardCell>();
        if (startPosition.x == endPosition.x)
        {
            for (int i = Mathf.Min(startPosition.y, endPosition.y); i <= Mathf.Max(startPosition.y, endPosition.y); i++)
            {
                cellsToRemove.Add(board[startPosition.x, i]);
                for (int j = startPosition.x; j < rows - 1; j++)
                {
                    if (board[j + 1, i] == null)
                    {
                        // Debug.Log("remove cell: " + (j + 1) + " " + i);
                        break;
                    }
                    // Debug.Log($"{j + 1}:{i} -> {j}{i}");
                    board[j, i] = board[j + 1, i];
                    board[j, i].SetPositionInBoard(new Vector2Int(j, i));
                    board[j + 1, i] = null;
                }
            }
        } else
        {
            // Debug.LogError("fsdkfljsdlfjsal;jfl;as");
            int lowerX = Mathf.Min(startPosition.x, endPosition.x);
            int upperX = Mathf.Max(startPosition.x, endPosition.x);
            int commonY = startPosition.y;
            int gap = upperX - lowerX + 1;
            for (int i = lowerX; i < board.GetLength(0); i++)
            {
                if (i <= upperX) {
                    cellsToRemove.Add(board[i, commonY]);
                }
                if (i + gap < board.GetLength(0) && board[i + gap, commonY] != null)
                {
                    board[i, commonY] = board[i + gap, commonY];
                    board[i, commonY].SetPositionInBoard(new Vector2Int(i, commonY));
                } else
                {
                    board[i, commonY] = null;
                }
            }
            // Debug.Log("num cells to remove: " + cellsToRemove.Count);
        }
        return cellsToRemove;
    }
}
