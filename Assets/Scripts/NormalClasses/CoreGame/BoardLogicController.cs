using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BoardLogicController 
{
    char[,] board;
    int numCols, numRows;
    HintWordInfo hintWordInfo = null;
    public BoardLogicController(char[,] boardParam)
    {
        SetCharBoard(boardParam);
    }
    public void SetCharBoard(char[,] board) {
        this.board = board;
        numCols = board.GetLength(1);
        numRows = board.GetLength(0);
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
                    res += board[fromPos.x, i];
                }
            } else
            {
                for (int i = fromPos.y; i <= toPos.y; i++)
                {
                    res += board[fromPos.x, i];
                }
            }
        } else if (fromPos.y == toPos.y)
        {
            if (fromPos.x > toPos.x)
            {
                for (int i = fromPos.x; i >= toPos.x; i--)
                {
                    res += board[i, fromPos.y];
                }
            } else
            {
                for (int i = fromPos.x; i <= toPos.x; i++)
                {
                    res += board[i, fromPos.y];
                }
            }
        }
        return res;
    }
    public List<Vector2Int> GetVerticalAndHorizontalCellsFromCell(Vector2Int cellPos)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        for (int col = 0; col < numCols; col++)
        {
            if (board[cellPos.x, col] != ' ')
            {
                cells.Add(new Vector2Int(cellPos.x, col));
            }
            else break;
        }
        for (int row = 0; row < numRows; row++)
        {
            if (board[row, cellPos.y] != ' ')
            {
                cells.Add(new Vector2Int(row, cellPos.y));
            }
            else break;
        }
        return cells;
    }
    public List<Vector2Int> GetAllPositionsInRange(Vector2Int startPosition, Vector2Int endPosition)
    {
        List<Vector2Int> positions = new List<Vector2Int>(); 
        if (!CheckValidInputPositions(startPosition, endPosition)) {
            positions.Add(startPosition);
            return positions;
        }
        if (startPosition.x == endPosition.x)
        {
            if (startPosition.y > endPosition.y) {
                for (int i = startPosition.y; i >= endPosition.y; i--) {
                    if (board[startPosition.x, i] == ' ') break;
                    positions.Add(new Vector2Int(startPosition.x, i));
                }
            } else {
                for (int i = startPosition.y; i <= endPosition.y; i++) {
                    if (board[startPosition.x, i] == ' ') break;
                    positions.Add(new Vector2Int(startPosition.x, i));
                }
            } 
        } else if (startPosition.y == endPosition.y)
        {
            if (startPosition.x > endPosition.x) {
                for (int i = startPosition.x; i >= endPosition.x; i--) {
                    if (board[i, startPosition.y] == ' ') break;
                    positions.Add(new Vector2Int(i, startPosition.y));
                }
            } else {
                for (int i = startPosition.x; i <= endPosition.x; i++) {
                    if (board[i, startPosition.y] == ' ') break;
                    positions.Add(new Vector2Int(i, startPosition.y));
                }
            } 
        } 
        return positions;
    }

    //public void ResetValidWordColor(S)
    public bool CheckValidInputPositions(Vector2Int startPosition, Vector2Int endPosition)
    {
        if (startPosition == endPosition) return true;
        if (startPosition.x != endPosition.x && startPosition.y != endPosition.y)
        {
            return false;
        } else if (startPosition.x == endPosition.x)
        {
            for (int i = Mathf.Min(startPosition.y, endPosition.y); i <= Mathf.Max(startPosition.y, endPosition.y); i++)
            {
                if (board[startPosition.x, i] == ' ') return false;
            }
        } else
        {
            for (int i = Mathf.Min(startPosition.x, endPosition.x); i <= Mathf.Max(startPosition.x, endPosition.x); i++)
            {
                if (board[i, startPosition.y] == ' ') return false;
            }
        }
        return true;
    }
    public CellSteps RemoveCellsInRangeAndCollapsBoard(Vector2Int fromPosition, Vector2Int toPosition)
    {
        CellSteps cellSteps = new CellSteps();
        if (fromPosition.x == toPosition.x)
        {
            int i = fromPosition.y; 
            if (toPosition.y > i) toPosition.y++;
            else toPosition.y--;
            int cnt = 0;
            while (i != toPosition.y) {
                cellSteps.CellsToDeletes.Add(new Vector2Int(fromPosition.x, i));
                board[fromPosition.x, i] = ' ';
                bool hasCell = false;
                for (int j = fromPosition.x; j < numRows - 1; j++)
                {
                    if (board[j + 1, i] == ' ')
                    {
                        break;
                    } else if (!hasCell) {
                        hasCell = true;
                        cellSteps.Steps.Add(new List<MoveInfo>());
                    } 
                    board[j, i] = board[j + 1, i];
                    board[j + 1, i] = ' ';
                    cellSteps.Steps[cellSteps.Steps.Count - 1].Add(new MoveInfo(new Vector2Int(j + 1, i), new Vector2Int(j, i)));
                }
                if (fromPosition.y > toPosition.y) i--;
                else i++;
                cnt++;
                if (cnt > 100) {
                    Debug.LogError("Inside infinite looppppppppppppppppppppp");
                }
            }
        } else
        {
            int lowerX = Mathf.Min(fromPosition.x, toPosition.x);
            int upperX = Mathf.Max(fromPosition.x, toPosition.x);
            int commonY = fromPosition.y;
            int gap = upperX - lowerX + 1;
            cellSteps.Steps.Add(new List<MoveInfo>());
            for (int i = lowerX; i < board.GetLength(0); i++)
            {
                if (i <= upperX) {
                    cellSteps.CellsToDeletes.Add(new Vector2Int(i, commonY));
                    board[i, commonY] = ' ';
                }
                if (i + gap < board.GetLength(0) && board[i + gap, commonY] != ' ')
                {
                    cellSteps.Steps[cellSteps.Steps.Count - 1].Add(new MoveInfo(new Vector2Int(i + gap, commonY), new Vector2Int(i, commonY)));
                    board[i, commonY] = board[i + gap, commonY];
                    board[i + gap, commonY] = ' ';
                } 
            }
        }
        // Debug.LogError(Show());
        return cellSteps;
    }

    public Vector2Int GetNextHintPosition(List<string> remainingWords) {
        if (hintWordInfo == null) {
            hintWordInfo = FindHintWord(remainingWords);
        }
        if (hintWordInfo == null) return Vector2Int.one * -1;
        return hintWordInfo.GetNextCharPosition();
    } 

    public bool IsHintWordCompleted(List<string> remainingWords) {
        if (hintWordInfo == null) {
            hintWordInfo = FindHintWord(remainingWords);
            // Debug.Log("hint info is null");
        }
        if (hintWordInfo == null) {
            // Debug.Log("Hint word info is null");
            return true;
        } else {
            // Debug.Log("Hint info not null");
            // Debug.Log(hintWordInfo);
        }
        return hintWordInfo.IsCompleted();
    }
    public HintWordInfo GetHintWordInfo() {
        return hintWordInfo;
    }
    public void SetHintWordInfo(HintWordInfo hintWordInfo) {
        this.hintWordInfo = hintWordInfo;
    }

    HintWordInfo FindHintWord(List<string> remainingWords) {
        // Debug.Log("remaing word: " + remainingWords.Count);
        // foreach (var w in remainingWords) {
        //     Debug.Log(w);
        // }
        List<string> reversedRemainingWords = new List<string>();
        for (int i = 0; i < remainingWords.Count; i++) {
            reversedRemainingWords.Add(remainingWords[i].Reversed());
        }
        List<string> rowWords = new List<string>();
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                stringBuilder.Append(board[i, j]);
            }
            rowWords.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        List<string> colWords = new List<string>();
        for (int j = 0; j < numCols; j++) {
            for (int i = 0; i < numRows; i++) {
                stringBuilder.Append(board[i, j]);
            }
            colWords.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        HintWordInfo res = new HintWordInfo();
        bool hasRes = false;
        for (int i = 0; i < rowWords.Count; i++) {
            if (hasRes) break;
            for (int j = 0; j < remainingWords.Count; j++) {
                int index = rowWords[i].IndexOf(remainingWords[j]);
                if (index != -1) {
                    // Debug.LogError("row word: " + rowWords[i] + " remaining: " + remainingWords[j]);
                    res.Position = new Vector2Int(i, index);
                    res.Word = remainingWords[j];
                    res.Direction = new Vector2Int(0, 1);//Vector2Int.right;
                    hasRes = true;
                    break;
                } else {
                    index = rowWords[i].IndexOf(reversedRemainingWords[j]);
                    if (index != -1) {
                        res.Position = new Vector2Int(i, index + reversedRemainingWords[j].Length - 1);
                        res.Word = reversedRemainingWords[j].Reversed();
                        res.Direction = new Vector2Int(0, -1); //Vector2Int.left;
                        hasRes = true;
                        break;
                    }
                }
            }
        }
        if (!hasRes) {
            for (int i = 0; i < colWords.Count; i++) {
                if (hasRes) break;
                for (int j = 0; j < remainingWords.Count; j++) {
                    int index = colWords[i].IndexOf(remainingWords[j]);
                    if (index != -1) {
                        res.Position = new Vector2Int(index, i);
                        res.Word = remainingWords[j];
                        res.Direction = new Vector2Int(1, 0);//Vector2Int.up;
                        hasRes = true;
                        break;
                    } else {
                        index = colWords[i].IndexOf(reversedRemainingWords[j]);
                        if (index != -1) {
                            res.Position = new Vector2Int(index + reversedRemainingWords[j].Length - 1, i);
                            res.Word = reversedRemainingWords[j].Reversed();
                            res.Direction = new Vector2Int(-1, 0);
                            hasRes = true;
                            break;
                        }
                    }
                }
            }
        }
        // Debug.Log("row worddddddddddddddddd: " + rowWords.Count);
        // foreach (var word in rowWords) {
        //     Debug.Log(word);
        // }
        // Debug.Log("-----------------");
        // Debug.Log("col worddddddddddddddddd: " + colWords.Count);
        // foreach (var word in colWords) {
        //     Debug.Log(word);
        // }
        if (hasRes) return res;
        Debug.Log("Can not find hint wordddddddddddddddddddddddd");
        return null;
    }
    public string Show()
    {
        string ans = "----------------------------\n";
        for (int i = board.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == ' ')
                {
                    ans += '-';
                }
                else
                {
                    ans += board[i, j];
                }
            }
            ans += "\n";
        }
        ans += "----------------------------\n";

        return ans;
    }
}