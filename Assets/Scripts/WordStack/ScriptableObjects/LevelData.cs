using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level")]
public class LevelData : ScriptableObject 
{
    public List<string> Words = new List<string>();
    public string Subject;
}

[System.Serializable]
public class BoardRowData
{
    public char[] row = null;
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
    char[,] charBoard = null;
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
    public BoardData(char[,] charBoard) {
        this.charBoard = charBoard;
        ROWS = charBoard.GetLength(0);
        COLS = charBoard.GetLength(1);
        DataInside = new List<BoardRowData>();
        for (int i = 0; i < ROWS; i++)
        {
            DataInside.Add(new BoardRowData(COLS));
            for (int j = 0; j < COLS; j++) {
                SetLetter(i, j, charBoard[i, j]);
            }
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
    public char[,] GetCharBoard() {
        if (charBoard == null) {
            charBoard = new char[ROWS, COLS];
            for (int i = 0; i < ROWS; i++) {
                for (int j = 0; j < COLS; j++) {
                    charBoard[i, j] = GetLetter(i, j);
                }
            }
        }
        return charBoard;
    }
}

