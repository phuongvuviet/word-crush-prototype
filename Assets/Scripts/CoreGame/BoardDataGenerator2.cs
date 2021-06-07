using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataGenerator2 
{
    const int MAX_WIDTH = 8, MAX_HEIGHT = 8;
    const int MIN_WIDTH = 3, MIN_HEIGHT = 3;
    LevelData levelData;
    LevelDataWrapper levelDataWrapper;
    char[,] boardData;
    List<string> words = null;
    int boardWidth, boardHeight;
    int maxWordLength;

    //int ROWS, COLS;
    //int maxWordLen, minWordLen;
    //int numVerticalWords, numHorizontalWords;
    public BoardDataGenerator2(LevelData levelDataParam)
    {
        levelData = levelDataParam;
        levelDataWrapper = new LevelDataWrapper(levelData);
        words = levelData.GetWords();
        Shuffle(words);
    }
    int cnt = 100;
    public char[,] GenerateBoard()
    {
        cnt--;
        if (cnt == 0)
        {
            Debug.Log("can not generate board");
            return null;
        } 
        // int numWords = levelData.GetNumWords();
        // List<Vector2Int> boardSizes = GetPossibleBoardSizes();
        // Debug.LogError("board size: " + boardSizes.Count);
        // int rdBoardSizeIndex = UnityEngine.Random.Range(0, boardSizes.Count);
        boardWidth = MAX_WIDTH;
        boardHeight = MAX_HEIGHT;
        boardData = new char[boardWidth, boardHeight];
        for (int i = 0; i< boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                boardData[i, j] = ' ';
            }
        }
        for (int i = 0; i < words.Count; i++)
        {
            string curWord = words[i];
            int isReversed = UnityEngine.Random.Range(1, 101);
            if (isReversed > 50) curWord = curWord.ReverseString(); 
            // int rd = UnityEngine.Random.Range(0, 2);
            int rd = 0;
            if (rd == 0)
            {
                InsertVertical(curWord);
                // if (!InsertVertical(words[i]))
                // {
                //     if (!InsertHorizontal(words[i]))
                //     {
                //         Debug.LogError("In hrere: " + boardWidth + " - " + boardHeight);
                //         return GenerateBoard();
                //     }
                // } 
            } else
            {
                if (!InsertHorizontal(curWord))
                {
                    if (!InsertVertical(curWord))
                    {
                        Debug.LogError("In hrere: " + boardWidth + " - " + boardHeight);
                        return GenerateBoard();
                    }
                } 
            }
        }
        Debug.Log("---------------BOARD-------------------------");
        Debug.Log(Show());
        boardData = RemoveEmptyColumnAndRow(boardData);
        boardWidth = boardData.GetLength(1);
        boardHeight = boardData.GetLength(0);
        return boardData;
    }
    public int GetBoardWidth()
    {
        return boardWidth;
    }
    public int GetBoardHeight()
    {
        return boardHeight;
    }
    public int GetMaxCellWidth()
    {
        int ans = 0;
        Debug.Log("board width: " + boardWidth + " actual: " + boardData.GetLength(1));
        for (int i = 0; i < boardWidth; i++)
        {
            if (boardData[0, i] != ' ') ans++;
        }
        return ans;
    }

    public int GetMaxColHeight()
    {
        int ans = 0;
        for (int j = 0; j < boardWidth; j++)
        {
            int cnt = 0;
            for (int i = 0; i < boardHeight; i++)
            {
                if (boardData[i, j] != ' ') cnt++;
                else break;
            }
            ans = Mathf.Max(ans, cnt);
        }
        return ans;
    }

    List<Vector2Int> GetPossibleBoardSizes()
    {
        List<Vector2Int> sizes = new List<Vector2Int>();
        int maxWordLen = levelDataWrapper.GetMaxWordLength();
        //int minWordLen = levelDataWrapper.GetMinWordLength();
        int wordArea = levelDataWrapper.GetWordsArea();
        for (int i = Mathf.Max(maxWordLen, MIN_WIDTH); i <= MAX_WIDTH; i++)
        {
            int height = (int)Mathf.Ceil(wordArea / (float)i);
            if (height < maxWordLen) height = maxWordLen;
            Debug.Log("I: " + i + " height: " + height);
            if (height >= Mathf.Max(maxWordLen, MIN_HEIGHT) && height <= MAX_HEIGHT)
            {
                sizes.Add(new Vector2Int(i, height));
            }
        }
        return sizes;
    }

    public bool InsertHorizontal(string word)
    {
        List<Tuple<int, int, int>> rowWidths = GetMaxWidthEachRow();
        Debug.Log("Row width length: " + rowWidths.Count);
        Shuffle(rowWidths);
        int wordLen = word.Length;
        Tuple<int,int,int> rowWidth = null;
        int maxColumnHeight = GetMaxColumnHeight();
        if (maxColumnHeight == boardHeight)
        {
            Debug.Log("1 can not insert horizontal");
            return false;
        } 
        for (int i = 0; i < rowWidths.Count; i++)
        {
            if (wordLen <= rowWidths[i].Item2)
            {
                rowWidth = rowWidths[i];
                break;
            }
        }
        if (rowWidth == null) 
        {
            Debug.Log("2 Cann't insert horizontal");
            return false;
        } 
        int rowIndex = rowWidth.Item1;
        int width = rowWidth.Item2;
        int startIndex = rowWidth.Item3;
        int positionToInsert = UnityEngine.Random.Range(startIndex, startIndex + width - wordLen + 1);
        Debug.Log("Row index: " + rowIndex + " width: " + width + " startIndex" + startIndex);
        Debug.Log("Position to insert: " + positionToInsert);
        // insert word
        for (int i = positionToInsert; i < positionToInsert + wordLen; i++)
        {
            if (boardData[rowIndex, i] != ' ')
            {
                MoveColUp(rowIndex, i);
            }
            boardData[rowIndex, i] = word[i - positionToInsert];
        }
        return true;
    }
    int GetMaxColumnHeight()
    {
        int res = 0;
        int cnt = 0;
        for (int j = 0; j < boardWidth; j++)
        {
            cnt = 0;
            for (int i = 0; i < boardHeight; i++)
            {
                if (boardData[i, j] != ' ') cnt++;
                else break;
            }
            res = Mathf.Max(res, cnt);
        }
        return res;
    }
    int GetNumFilledCellInColumn(int pos)
    {
        Debug.Log("Pos: " + pos);
        int res = 0; 
        for (int i = 0; i < boardHeight; i++)
        {
            if (boardData[i, pos] != ' ')
            {
                res++;
            }
            else break;
        }
        return res;
    }
    public bool InsertVertical(string word)
    {
        List<Tuple<int, int>> colHeights = GetColHeights();
        int wordLen = word.Length;

        int rdValue = 0;
        if (rdValue == 0)
        {
            int cnt = 30;
            int randomColIndex;
            do
            {
                randomColIndex = UnityEngine.Random.Range(0, 10000) % MAX_WIDTH;
                // Debug.Log("col index: " + randomColIndex + " board width: " + );
                cnt--;
                if (cnt == 0) 
                {
                    Debug.Log("Can not insert vertical");
                    Debug.Log(Show());
                    return false;
                }  
            } while (GetNumEmptyCellsInCol(randomColIndex) < wordLen);
            // fill col with char 
            int numEmptyCells = GetNumEmptyCellsInCol(randomColIndex);
            int numFilledCells = boardHeight - numEmptyCells; 
            int startRowIndex = UnityEngine.Random.Range(0, numFilledCells + 1);
            // move chars up
            for (int i = startRowIndex; i <= numFilledCells - 1; i++)
            {
                boardData[i + wordLen, randomColIndex] = boardData[i, randomColIndex];
            }
            // fill cells
            for (int i = startRowIndex; i < startRowIndex + wordLen; i++)
            {
                boardData[i, randomColIndex] = word[i - startRowIndex];
            }
        } else
        {
        }
        return true;
    }
    public int GetNumEmptyCellsInCol(int col)
    {
        int notEmptyCellCnt = 0;
        for (int i = 0; i < boardHeight; i++)
        {
            if (boardData[i, col] == ' ') break;
            else
            {
                notEmptyCellCnt++;
            }
        }
        return boardHeight - notEmptyCellCnt;
    }

    public void MoveColUp(int xPos, int yPos)
    {
        int peakPos = xPos;
        for (int i = xPos + 1; i < boardHeight; i++)
        {
            if (boardData[i, yPos] == ' ')
            {
                peakPos = i;
                break;
            }
        }
        for (int i = peakPos; i > xPos; i--)
        {
            boardData[i, yPos] = boardData[i - 1, yPos];
        }
        boardData[xPos, yPos] = ' ';
    }
    public void MoveLeft(int xPos, int yPos)
    {
        int leftmostPos = yPos;
        for (int i = yPos; i >= 0; i--)
        {
            if (boardData[xPos, i] == ' ')
            {
                leftmostPos = i + 1;
                break;
            }
        }
        for (int i = leftmostPos - 1; i < yPos; i++)
        {
            boardData[xPos, i] = boardData[xPos, i + 1];
        }
        boardData[xPos, yPos] = ' ';
    }
    public void MoveRight(int xPos, int yPos)
    {
        int rightmosPos = yPos;
        for (int i = yPos; i < boardWidth; i++)
        {
            if (boardData[xPos, i] == ' ')
            {
                rightmosPos = i - 1;
                break;
            }
        }
        for (int i = rightmosPos + 1; i > yPos; i--)
        {
            boardData[xPos, i] = boardData[xPos, i - 1];
        }
        boardData[xPos, yPos] = ' ';
    }

    // tuple : item1 : row th, item2 : width, item3: index in row
    public List<Tuple<int, int, int>> GetMaxWidthEachRow()
    {
        List<Tuple<int, int, int>> widths = new List<Tuple<int, int, int>>();
        widths.Add(Tuple.Create(0, boardWidth, 0));
        for (int i = 0; i < boardHeight - 1; i++)
        {
            int curMaxLen = 0;
            int curLen = 0;
            int curIndex = 0;
            for (int j = 0; j < maxWordLength; j++)
            {
                if (boardData[i, j] == ' ' || j == maxWordLength - 1)
                {
                    if (j == maxWordLength - 1) curLen++;
                    if (curMaxLen < curLen)
                    {
                        curMaxLen = curLen;
                        curIndex = j;
                    }
                    curLen = 0;
                }
                else
                {
                    curLen++;
                }
            }
            widths.Add(Tuple.Create(i + 1, curMaxLen, curIndex));
        }
        return widths;
    }
    public List<Tuple<int, int>> GetColHeights()
    {
        List<Tuple<int, int>> heights = new List<Tuple<int, int>>();
        for (int i = 0; i < boardData.GetLength(1); i++)
        {
            int cnt = 0;
            for (int j = 0; j < boardData.GetLength(0); j++)
            {
                //Console.WriteLine("board data: " + boardData[j, i]);
                if (boardData[j, i] != ' ') cnt++;
                else break;
            }
            heights.Add(Tuple.Create(i, cnt));
        }
        return heights;
    }
    public void ShowRowWidth()
    {
        Console.WriteLine("------------ROW WIDTH-------------");
        var res = GetMaxWidthEachRow();
        for (int i = 0; i < res.Count; i++)
        {
            Console.WriteLine(res[i].Item1 + " - " + res[i].Item2);
        }
        Console.WriteLine("----------------------------------");
    }
    public string Show()
    {
        string ans = "----------------------------\n";
        for (int i = boardData.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < boardData.GetLength(1); j++)
            {
                if (boardData[i, j] == ' ')
                {
                    //Console.Write('-');
                    ans += '-';
                }
                else
                {
                    //Console.Write(boardData[i, j]);
                    ans += boardData[i, j];
                }
            }
            //Console.WriteLine();
            ans += "\n";
        }
        ans += "----------------------------\n";
        //Console.WriteLine("----------------------------");
        return ans;
    }

    public void Shuffle<T>(List<T> list)
    {
        System.Random rd = new System.Random();
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int rdIndex = rd.Next(0, i);
            T tmpValue = list[i];
            list[i] = list[rdIndex];
            list[rdIndex] = tmpValue;
        }
    }
    public LevelData GetLevelData()
    {
        return levelData;
    }
    public void SetLevelData(char[,] board)
    {
        levelData.SetBoardData(boardData);
    }
    public char[,] RemoveEmptyColumnAndRow(char[,] board) {
        int numEmptyColumns = 0; 
        int numEmptyRows = board.GetLength(0) - GetMaxColHeight(); 
        string log = Show();
        Debug.Log("--------------");
        Debug.Log(log);
        Debug.Log("--------------");
        for (int i = 0; i < board.GetLength(1); i++) {
            if (board[0, i] == ' ') numEmptyColumns++;
        }
        Debug.Log("empty row: " + numEmptyRows + " col: " + numEmptyColumns);
        char[,] resBoard = new char[MAX_HEIGHT - numEmptyRows, MAX_WIDTH - numEmptyColumns]; 
        ResetData(resBoard);
        int curJ = 0, curI = 0;
        for (int j = 0; j < board.GetLength(1); j++) {
            curI = 0;
            bool isEmptyCol = true;
            for (int i = 0; i < board.GetLength(0); i++) {
                if (board[i,j] == ' ') break;
                else {
                    resBoard[curI, curJ] = board[i, j];
                    isEmptyCol = false;
                }
                curI++;
            }
            if (!isEmptyCol) {
                curJ++;
            }
        }
        string log2 = "";
        for (int i = 0; i < resBoard.GetLength(0); i++) {
            for (int j = 0; j <resBoard.GetLength(1); j++) {
                log2 += resBoard[i, j] + " ";
            };
            log2 += "\n";
        }
        Debug.Log(log2);
        return resBoard;
    }
    void ResetData(char[,] arr) {
        for (int i = 0; i < arr.GetLength(0); i++) {
            for (int j = 0; j < arr.GetLength(1); j++) {
                arr[i, j] = ' ';
            }
        }
    }
}
