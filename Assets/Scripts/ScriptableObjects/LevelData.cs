using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level")]
public class LevelData : ScriptableObject 
{
    public List<string> Words = new List<string>();
    public BoardData serializedBoardData;
    //[SerializeField] char[,] boardData; 

    public bool ContainLetter(int x, int y)
    {
        return GetLetter(x, y) != ' '; 
    }
    public char GetLetter(int x, int y)
    {
        return serializedBoardData.GetLetter(x, y); //boardData[x, y] != ' ';
    }
    public int NumCols()
    {
        return serializedBoardData.COLS; 
    }
    public int NumRows()
    {
        return serializedBoardData.ROWS;
    }
    //public char[,] GetBoardData()
    //{
    //    return boardData;
    //}
    public void SetBoardData(char[,] boardData)
    {
        //boardData = boardDataParam;
        Debug.LogError("Set board dataaaaaa");
        serializedBoardData = new BoardData(boardData.GetLength(0), boardData.GetLength(1));
        for (int i = 0; i < serializedBoardData.ROWS; i++)
        {
            for (int j = 0; j < serializedBoardData.COLS; j++)
            {
                serializedBoardData.SetLetter(i, j, boardData[i,j]);
            }
        }

    } 
    public List<string> GetWords()
    {
        return Words;
    }
    public int GetNumWords()
    {
        return Words.Count;
    }
    public int GetMaxWordLength()
    {
        int maxLength = 0;
        for (int i = 0; i < Words.Count; i++)
        {
            maxLength = Mathf.Max(maxLength, Words[i].Length);
        }
        return maxLength;
    }
    public int GetMinWordLength()
    {
        int minLength = 100;
        for (int i = 0; i < Words.Count; i++)
        {
            minLength = Mathf.Max(minLength, Words[i].Length);
        }
        return minLength;
    }
}

[System.Serializable]
public class BoardRowData
{
    public char[] row;
    public BoardRowData(int num)
    {
        row = new char[num];
    }
    public void SetLetter(int pos, char letter)
    {
        row[pos] = letter;
    }
    public char GetLetter(int pos)
    {
        return row[pos];
    }
}
[System.Serializable]
public class BoardData
{
    public int ROWS, COLS;
    public List<BoardRowData> DataInside;
    public BoardData(int rows, int cols)
    {
        ROWS = rows;
        COLS = cols;
        DataInside = new List<BoardRowData>();
        for (int i = 0; i < rows; i++)
        {
            DataInside.Add(new BoardRowData(cols));
        }
    }
    public void SetLetter(int x, int y, char letter)
    {
        DataInside[x].SetLetter(y, letter);
    }
    public char GetLetter(int x, int y)
    {
        return DataInside[x].GetLetter(y);
    }
}

