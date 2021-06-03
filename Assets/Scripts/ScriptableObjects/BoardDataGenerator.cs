using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BoardDataGenerator : ScriptableObject 
{
    [SerializeField] int maxWidth = 8, maxHeight = 8;
    [SerializeField] LevelData levelData;
    char[,] boardData;

    int ROWS, COLS;
    int maxWordLen, minWordLen;
    int numVerticalWords, numHorizontalWords;
    public void GenerateBoard()
    {
        int numWords = levelData.GetNumWords();
        minWordLen = levelData.GetMinWordLength();
        maxWordLen = levelData.GetMaxWordLength();
        numVerticalWords = numWords / 2;
        numHorizontalWords = numWords - numVerticalWords;
        boardData = new char[maxHeight, maxWordLen];
        ROWS = boardData.GetLength(0);
        COLS = boardData.GetLength(1);

        for (int i = 0; i < maxHeight; i++)
        {
            for (int j = 0; j < maxWordLen; j++)
            {
                boardData[i, j] = ' ';
            }
        }
        List<string> words = levelData.Words;
        Shuffle(words);
        for (int i = 0; i < words.Count; i++)
        {
            int rd = UnityEngine.Random.Range(0, 2);
            if (rd == 0)
            {
                InsertHorizontal(words[i]);
            } else
            {
                InsertVertical(words[i]);
            }
        }
        Debug.LogError(Show());
        levelData.SetBoardData(boardData);
        //return boardData;
    }

    public void InsertHorizontal(string word)
    {
        List<Tuple<int, int>> rowWidths = GetMaxWidthEachRow();
        Shuffle(rowWidths);
        int wordLen = word.Length;
        int rowIndex = -1;
        for (int i = 0; i < rowWidths.Count; i++)
        {
            if (wordLen <= rowWidths[i].Item2)
            {
                rowIndex = rowWidths[i].Item1;
                break;
            }
        }
        Console.WriteLine("row index: " + rowIndex);
        int startIndex = 0;
        int curWidth = 0;
        if (rowIndex > 0)
        {
            for (int i = 0; i < boardData.GetLength(1); i++)
            {
                if (boardData[rowIndex, i] == ' ')
                {
                    if (maxWidth < curWidth)
                    {
                        maxWidth = curWidth;
                        startIndex = i;
                    }
                }
                else
                {
                    curWidth++;
                }
            }
        }
        // insert word
        for (int i = startIndex; i < startIndex + wordLen; i++)
        {
            if (boardData[rowIndex, i] != ' ')
            {
                MoveColUp(rowIndex, i);
            }
            boardData[rowIndex, i] = word[i - startIndex];
        }
    }
    public void InsertVertical(string word)
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
            int randomColIndex;
            do
            {
                randomColIndex = rd.Next(0, COLS);
                //Console.WriteLine("rd index: " + randomColIndex + " num empty: " + GetNumEmptyCellsInCol(randomColIndex));
            } while (GetNumEmptyCellsInCol(randomColIndex) < wordLen);
            int numFilledCells = ROWS - GetNumEmptyCellsInCol(randomColIndex);
            //Console.WriteLine("Num filled cells: " + numFilledCells);
            for (int i = numFilledCells - 1; i >= 0; i--)
            {
                boardData[i + wordLen, randomColIndex] = boardData[i, randomColIndex];
            }
            for (int i = 0; i < wordLen; i++)
            {
                boardData[i, randomColIndex] = word[i];
            }
        }
    }
    public int GetNumEmptyCellsInCol(int col)
    {
        int notEmptyCellCnt = 0;
        for (int i = 0; i < ROWS; i++)
        {
            if (boardData[i, col] == ' ') break;
            else
            {
                notEmptyCellCnt++;
            }
        }
        return ROWS - notEmptyCellCnt;
    }

    public void MoveColUp(int xPos, int yPos)
    {
        int peakPos = xPos;
        for (int i = xPos + 1; i < ROWS; i++)
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
        for (int i = yPos; i < COLS; i++)
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

    public List<Tuple<int, int>> GetMaxWidthEachRow()
    {
        List<Tuple<int, int>> widths = new List<Tuple<int, int>>();
        widths.Add(Tuple.Create(0, boardData.GetLength(1)));
        for (int i = 0; i < boardData.GetLength(0) - 1; i++)
        {
            int curMaxLen = 0;
            int curLen = 0;
            for (int j = 0; j < maxWordLen; j++)
            {
                if (boardData[i, j] == ' ')
                {
                    curMaxLen = Math.Max(curMaxLen, curLen);
                    curLen = 0;
                }
                else
                {
                    curLen++;
                }
            }
            curMaxLen = Math.Max(curMaxLen, curLen);
            widths.Add(Tuple.Create(i + 1, curMaxLen));
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

