using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDataGenerator 
{
    const int MAX_WIDTH = 8, MAX_HEIGHT = 10;
    char[,] boardData;
    List<string> words = null;
    int boardWidth, boardHeight;

    int cnt = 100;
    public char[,] GenerateBoard(List<string> words)
    {
        this.words = words; 
        Shuffle(words);

        cnt--;
        if (cnt == 0)
        {
            Debug.Log("can not generate board");
            return null;
        } 
        boardWidth = MAX_WIDTH;
        boardHeight = MAX_HEIGHT;
        boardData = new char[boardHeight, boardWidth];
        ResetCharBoard(boardData);
        for (int i = 0; i < words.Count; i++)
        {
            string curWord = words[i];
            int isReversed = UnityEngine.Random.Range(0, 2);
            if (isReversed == 1) curWord = curWord.Reversed(); 
            int rd = UnityEngine.Random.Range(0, 2);
            // InsertHorizontal(curWord);
            // InsertVertical(curWord);
            if (rd == 0)
            {
                if (!InsertVertical(curWord))
                {
                    if (!InsertHorizontal(curWord))
                    {
                        Debug.LogError("FAIL TO GENERATE BOARD");
                        return GenerateBoard(words);
                    }
                } 
            } else
            {
                if (!InsertHorizontal(curWord))
                {
                    if (!InsertVertical(curWord))
                    {
                        Debug.LogError("FAIL TO GENERATE BOARD");
                        return GenerateBoard(words);
                    }
                } 
            }
        }
        boardData = RemoveEmptyColumnAndRow(boardData);
        boardWidth = boardData.GetLength(1);
        boardHeight = boardData.GetLength(0);
        return boardData;
    }

    int GetMaxColHeight()
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


    bool InsertHorizontal(string word)
    {
        int wordLen = word.Length;
        int randomRow = 0, randomCol = 0;
        int cnt = 0;
        do {
            randomRow = UnityEngine.Random.Range(0, boardWidth);
            randomCol = UnityEngine.Random.Range(0, MAX_WIDTH - wordLen + 1);
            cnt++;
            if (cnt > 100) {
                Debug.LogError("Can not insert horizontallllllllllllllllllllllllllllllll");
                return false;
            }
        } while (!IsValidRowToInsertWord(word, randomCol));
        List<int> colHeights = GetColHeights();
        bool hasValley = false;
        for (int i = randomCol; i < randomCol + wordLen; i++) {
            if (boardData[randomRow, i] == ' ') {
                hasValley = true;
                break;
            }  
        }
        int curWordIndex = 0;
        if (!hasValley) {
            for (int i = randomCol; i < randomCol + wordLen; i++) {
                if (boardData[randomRow, i] == ' ') {
                    boardData[colHeights[i], i] = word[curWordIndex++];
                } else {
                    MoveColUp(randomRow, i, colHeights[i], 1);
                    boardData[randomRow, i] = word[curWordIndex++];
                }
            }
        } else {
            for (int i = randomCol; i < randomCol + wordLen; i++) {
                boardData[colHeights[i], i] = word[curWordIndex++];
            }
        }
        return true;
    }

    bool IsValidRowToInsertWord(string word, int col) {
        List<int> colHeights = GetColHeights();
        for (int i = col; i < col + word.Length; i++) {
            if (colHeights[i] == MAX_HEIGHT) {
                return false;
            }
        }
        return true;
    }
    bool InsertVertical(string word)
    {
        List<int> colHeights = GetColHeights();
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
                    return false;
                }  
            } while (GetNumEmptyCellsInCol(randomColIndex) < wordLen);
            int curColHeight = GetColHeight(randomColIndex);
            int startRowIndex = UnityEngine.Random.Range(0, curColHeight + 1);
            MoveColUp(startRowIndex, randomColIndex, curColHeight, wordLen);

            for (int i = startRowIndex; i < startRowIndex + wordLen; i++)
            {
                boardData[i, randomColIndex] = word[i - startRowIndex];
            }
        } else
        {
        }
        return true;
    }
    int GetColHeight(int col) {
        int ans = 0;
        for (int i = 0; i < MAX_HEIGHT; i++) {
            if (boardData[i, col] == ' ') return ans;
            else ans++;
        }
        return ans;
    } 
    int GetNumEmptyCellsInCol(int col)
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

    void MoveColUp(int xPos, int yPos, int colHeight, int step = 1)
    {
        for (int i = colHeight - 1; i >= xPos; i--) {
            boardData[i + step, yPos] = boardData[i, yPos];
        }
        boardData[xPos, yPos] = ' ';
    }

    List<int> GetColHeights()
    {
        List<int> heights = new List<int>();
        for (int i = 0; i < boardData.GetLength(1); i++)
        {
            int cnt = 0;
            for (int j = 0; j < boardData.GetLength(0); j++)
            {
                if (boardData[j, i] != ' ') cnt++;
                else break;
            }
            heights.Add(cnt);
        }
        return heights;
    }
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int rdIndex = UnityEngine.Random.Range(0, i + 1);
            T tmpValue = list[i];
            list[i] = list[rdIndex];
            list[rdIndex] = tmpValue;
        }
    }
    char[,] RemoveEmptyColumnAndRow(char[,] board) {
        int numEmptyColumns = 0; 
        int numEmptyRows = board.GetLength(0) - GetMaxColHeight(); 
        for (int i = 0; i < board.GetLength(1); i++) {
            if (board[0, i] == ' ') numEmptyColumns++;
        }
        char[,] resBoard = new char[MAX_HEIGHT - numEmptyRows, MAX_WIDTH - numEmptyColumns]; 
        ResetCharBoard(resBoard);
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
        return resBoard;
    }
    void ResetCharBoard(char[,] arr) {
        for (int i = 0; i < arr.GetLength(0); i++) {
            for (int j = 0; j < arr.GetLength(1); j++) {
                arr[i, j] = ' ';
            }
        }
    }
}

