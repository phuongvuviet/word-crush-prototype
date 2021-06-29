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
        if (!VerifyInputPositions(fromPos, toPos)) return res;
        Debug.Log("width: " + numCols + " height: " + numRows + " from: " + fromPos + " to: " + toPos);
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
        if (!VerifyInputPositions(startPosition, endPosition)) {
            Debug.Log("Can not verity positions");
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
    public bool VerifyInputPositions(Vector2Int startPosition, Vector2Int endPosition)
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
        cellSteps.HorizontallyCollapsedSteps.AddRange(RemoveVerticalGap());
        return cellSteps;
    }

    // Remove empte column between two columns
    List<MoveInfo> RemoveVerticalGap() {
        List<MoveInfo> moveInfos = new List<MoveInfo>();
        int[] gaps = FindColumnGap();
        if (gaps[0] != -1 && gaps[1] != -1) {
            int gapL = gaps[0], gapR = gaps[1];
            int gapValueUnit = gapR - gapL + 1;
            int numLeftCols = 0, numRightCols = 0;
            for (int i = gapL - 1; i >= 0; i--) {
                if (board[0, i] == ' ') break;
                else {
                    numLeftCols++;
                } 
            }
            for (int i = gapR + 1; i < numCols; i++) {
                if (board[0, i] == ' ') break;
                else {
                    numRightCols++;
                } 
            }
            Debug.Log("gap l: " + gapL + " gap r: " + gapR + " gap value: " + gapValueUnit + " numLeft: " + numLeftCols + " numRight: " + numRightCols);
            if (numLeftCols < numRightCols) { // move to right
                moveInfos.AddRange(ShiftColsHorizontal(gapL - numLeftCols, gapL - 1, gapValueUnit, true));
            } else if (numLeftCols > numRightCols) { // move to left
                moveInfos.AddRange(ShiftColsHorizontal(gapR + 1, gapR + numRightCols, gapValueUnit, false));
            } else {  
                int centerCol = (numCols - 1)/ 2;
                int centerGap = (gapL + gapR) / 2;
                if (centerGap <= centerCol) {
                    moveInfos.AddRange(ShiftColsHorizontal(gapL - numLeftCols, gapL - 1, gapValueUnit, true));
                } else {
                    moveInfos.AddRange(ShiftColsHorizontal(gapR + 1, gapR + numRightCols, gapValueUnit, false));
                }
            }
        }
        Debug.Log("remove vertical gap: " + moveInfos.Count);
        return moveInfos;
    }

    // Shift all columns to the direction units
    public List<MoveInfo> ShiftColsHorizontal(int leftCol, int rightCol, int moveUnit, bool moveRight = true) {
        List<MoveInfo> moveInfos = new List<MoveInfo>();
        if (moveRight) {
            for (int j = rightCol + moveUnit; j >= leftCol + moveUnit; j--) {
                for (int i = 0; i < numRows; i++) {
                    if (board[i, j - moveUnit] == ' ') break; 
                    board[i, j] = board[i, j - moveUnit];
                    board[i, j - moveUnit] = ' ';
                    Vector2Int fromPos = new Vector2Int(i, j - moveUnit);
                    Vector2Int toPos = new Vector2Int(i, j);
                    moveInfos.Add(new MoveInfo(fromPos, toPos));
                }
            }
        } else {
            for (int j = leftCol - moveUnit; j <= rightCol - moveUnit; j++) {
                for (int i = 0; i < numRows; i++) {
                    if (board[i, j + moveUnit] == ' ') break; 
                    board[i, j] = board[i, j + moveUnit];
                    board[i, j + moveUnit] = ' ';
                    Vector2Int fromPos = new Vector2Int(i, j + moveUnit);
                    Vector2Int toPos = new Vector2Int(i, j);
                    moveInfos.Add(new MoveInfo(fromPos, toPos));
                }
            }
        }
        return moveInfos;
    }
    int[] FindColumnGap() {
        int[] gaps = new int[2]{-1, -1};
        bool hasChar = false;
        for (int i = 0; i < numCols; i++) {
            if (board[0, i] != ' ') {
                hasChar = true;
            } else {
                if (hasChar) {
                    int l = i;
                    int r = -1;
                    for (i = i + 1; i < numCols; i++) {
                        if (board[0, i] != ' ') {
                            r = i - 1;
                            break;
                        }
                    }
                    if (r != -1) {
                        gaps[0] = l;
                        gaps[1] = r;
                        break;
                    }
                }
            }
        }
        return gaps;
    }
    public bool CheckIfBoardValid(List<string> remainingWord) {
        return FindHintWord(remainingWord) != null;
    }
    public bool CheckIfHintWordValid() {
        if (hintWordInfo.HasWordInfo()) {
            string curHintWord = GetWord(hintWordInfo.GetStartPosition(), hintWordInfo.GetEndPosition());
            if (curHintWord == hintWordInfo.Word) return true;
        }
        return false;
    }

    public Vector2Int GetNextHintPosition(List<string> remainingWords) {
        if (!hintWordInfo.HasWordInfo() || hintWordInfo.IsCompleted()) {
            hintWordInfo = FindHintWord(remainingWords);
        } 
        return hintWordInfo.GetNextCharPosition();
    } 

    public bool IsHintWordCompleted(List<string> remainingWords) {
        if (hintWordInfo == null || hintWordInfo.Position == Vector2Int.one * -1) {
            hintWordInfo = FindHintWord(remainingWords);
        }
        if (hintWordInfo == null) {
            Debug.LogError("Invalid boarddd, need to verifyyyyyyyyyyyyyyyyy");
            return true;
        }
        return hintWordInfo.IsCompleted();
    }
    public bool HasHintWord() {
        return hintWordInfo.HasWordInfo();
    }
    public void UpdateHintWordInfo() {
        Debug.Log("Update hint word before: " + hintWordInfo.Word);
        HintWordInfo updatedWordInfo = FindHintWord(new List<string>() { hintWordInfo.Word });
        Debug.Log("Update hint word after: " + updatedWordInfo.Word);
        hintWordInfo.Position = updatedWordInfo.Position;
    }
    public bool CheckIfBoardHasHintWord() {
        Debug.Log("Chieck hint word before: " + hintWordInfo.Word);
        HintWordInfo updatedWordInfo = FindHintWord(new List<string>() { hintWordInfo.Word });
        if (updatedWordInfo == null) {
            return false;
        }
        return true;
    }
    public HintWordInfo GetHintWordInfo() {
        return hintWordInfo;
    }
    public void SetHintWordInfo(HintWordInfo hintWordInfo) {
        this.hintWordInfo = hintWordInfo;
    }
    public void ResetHintWord() {
        if (hintWordInfo != null) {
            hintWordInfo.Reset();
        }
    }

    public HintWordInfo FindHintWord(List<string> remainingWords) {
        // Debug.Log("Find hint wordddddddddddd");
        // Debug.Log(Show());
        List<string> reversedRemainingWords = new List<string>();
        for (int i = 0; i < remainingWords.Count; i++) {
            reversedRemainingWords.Add(remainingWords[i].Reversed());
        }
        List<string> rowWords = GetRowWords(); 
        List<string> colWords = GetColWords();
        List<int> rowWordIndices = new List<int>();
        List<int> colWordIndices = new List<int>();
        for (int i = 0; i < rowWords.Count; i++) rowWordIndices.Add(i);
        for (int i = 0; i < colWords.Count; i++) colWordIndices.Add(i);
        Utilities.Shuffle(rowWordIndices);
        Utilities.Shuffle(colWordIndices);
        HintWordInfo res = new HintWordInfo();
        bool hasRes = false;
        for (int i = 0; i < rowWordIndices.Count; i++) {
            int rowWordIndex = rowWordIndices[i];
            string curRowWord = rowWords[rowWordIndex];
            if (hasRes) break;
            for (int j = 0; j < remainingWords.Count; j++) {
                int index = curRowWord.IndexOf(remainingWords[j]);
                if (index != -1) {
                    // Debug.LogError("row word: " + rowWords[i] + " remaining: " + remainingWords[j]);
                    res.Position = new Vector2Int(rowWordIndex, index);
                    res.Word = remainingWords[j];
                    res.Direction = new Vector2Int(0, 1);//Vector2Int.right;
                    hasRes = true;
                    break;
                } else {
                    index = curRowWord.IndexOf(reversedRemainingWords[j]);
                    if (index != -1) {
                        res.Position = new Vector2Int(rowWordIndex, index + reversedRemainingWords[j].Length - 1);
                        res.Word = reversedRemainingWords[j].Reversed();
                        res.Direction = new Vector2Int(0, -1); //Vector2Int.left;
                        hasRes = true;
                        break;
                    }
                }
            }
        }
        if (!hasRes) {
            for (int i = 0; i < colWordIndices.Count; i++) {
                int colWordIndex = colWordIndices[i];
                string curColWord = colWords[colWordIndex];
                if (hasRes) break;
                for (int j = 0; j < remainingWords.Count; j++) {
                    int index = curColWord.IndexOf(remainingWords[j]);
                    if (index != -1) {
                        res.Position = new Vector2Int(index, colWordIndex);
                        res.Word = remainingWords[j];
                        res.Direction = new Vector2Int(1, 0);//Vector2Int.up;
                        hasRes = true;
                        break;
                    } else {
                        index = colWords[i].IndexOf(reversedRemainingWords[j]);
                        if (index != -1) {
                            res.Position = new Vector2Int(index + reversedRemainingWords[j].Length - 1, colWordIndex);
                            res.Word = reversedRemainingWords[j].Reversed();
                            res.Direction = new Vector2Int(-1, 0);
                            hasRes = true;
                            break;
                        }
                    }
                }
            }
        }
        // Debug.LogError("Resssss: " + res);
        if (hasRes) return res;
        Debug.Log("Can not find hint wordddddddddddddddddddddddd");
        return null;
    }

    List<string> GetRowWords() {
        StringBuilder stringBuilder = new StringBuilder();
        List<string> rowWords = new List<string>();
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                stringBuilder.Append(board[i, j]);
            }
            rowWords.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        return rowWords;
    }
    List<string> GetColWords() {
        StringBuilder stringBuilder = new StringBuilder();
        List<string> colWords = new List<string>();
        for (int j = 0; j < numCols; j++) {
            for (int i = 0; i < numRows; i++) {
                if (board[i, j] == ' ') break;
                stringBuilder.Append(board[i, j]);
            }
            colWords.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        }
        return colWords;
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