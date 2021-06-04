using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataGenerator 
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
    public BoardDataGenerator(LevelData levelDataParam)
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
        int numWords = levelData.GetNumWords();
        List<Vector2Int> boardSizes = GetPossibleBoardSizes();
        Debug.LogError("board size: " + boardSizes.Count);
        int rdBoardSizeIndex = UnityEngine.Random.Range(0, boardSizes.Count);
        boardWidth = boardSizes[rdBoardSizeIndex].x;
        boardHeight = boardSizes[rdBoardSizeIndex].y;
        boardData = new char[boardHeight, boardWidth];
        for (int i = 0; i< boardHeight; i++)
        {
            for (int j = 0; j < boardWidth; j++)
            {
                boardData[i, j] = ' ';
            }
        }
        //Debug.Log("Width: " + boardWidth + " height: " + boardHeight);
        for (int i = 0; i < words.Count; i++)
        {
            int rd = UnityEngine.Random.Range(0, 2);
            if (rd == 0)
            {
                if (!InsertVertical(words[i]))
                {
                    if (!InsertHorizontal(words[i]))
                    {
                        Debug.LogError("In hrere: " + boardWidth + " - " + boardHeight);
                        return GenerateBoard();
                    }
                } 
            } else
            {
                if (!InsertHorizontal(words[i]))
                {
                    if (!InsertVertical(words[i]))
                    {
                        Debug.LogError("In hrere: " + boardWidth + " - " + boardHeight);
                        return GenerateBoard();
                    }
                } 
            }
        }
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
        for (int i = 0; i < boardWidth; i++)
        {
            if (boardData[0, i] != ' ') ans++;
        }
        return ans;
    }
    public int GetMaxCellHeight()
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
        System.Random rd = new System.Random();
        int wordLen = word.Length;

        //int rdValue = rd.Next(0, 2);
        // 0 move from bottom to top
        // 1 move top to bottom
        int rdValue = 0;
        if (rdValue == 0)
        {
            int cnt = 30;
            int randomColIndex;
            do
            {
                randomColIndex = rd.Next(0, boardWidth);
                cnt--;
                if (cnt == 0) 
                {
                    Debug.Log("Can not insert vertical");
                    Debug.Log(Show());
                    return false;
                }  
            } while (GetNumEmptyCellsInCol(randomColIndex) < wordLen);
            int numFilledCells = boardHeight - GetNumEmptyCellsInCol(randomColIndex);
            for (int i = numFilledCells - 1; i >= 0; i--)
            {
                boardData[i + wordLen, randomColIndex] = boardData[i, randomColIndex];
            }
            for (int i = 0; i < wordLen; i++)
            {
                boardData[i, randomColIndex] = word[i];
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
}

