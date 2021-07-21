using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WCross
{
    public class WCBoardController 
    {
        LetterModel[,] letterBoard;
        private Action<string> _onHintedWordFound;
        public WCBoardController(LetterModel[,] letterBoard, Action<string> onHintedWordFound)
        {
            this.letterBoard = letterBoard;
            _onHintedWordFound = onHintedWordFound;
        }

        public Vector2Int HintRandomLetter()
        {
            List<Vector2Int> allUnOpenLetterPosition = GetAllUnOpenLetter();
            Utilities.Shuffle(allUnOpenLetterPosition);
            Debug.Log("All un open letter: " + allUnOpenLetterPosition.Count);
            HintLetter(allUnOpenLetterPosition[0]);
            CheckIfHasNewWordsFound(allUnOpenLetterPosition[0]);
            return allUnOpenLetterPosition[0];
        }

        void CheckIfHasNewWordsFound(Vector2Int pos)
        {
            int height = letterBoard.GetLength(0);
            int width = letterBoard.GetLength(1);
            string horizontalWord = "";
            string verticalWord = "";
            Vector2Int iterPos = pos;
            Vector2Int rightDir = new Vector2Int(0, 1);
            Vector2Int leftDir = new Vector2Int(0, -1);
            Vector2Int upDir = new Vector2Int(1, 0);
            Vector2Int downDir = new Vector2Int(-1, 0);
            while (iterPos.y >= 0 && letterBoard[iterPos.x, iterPos.y] != null && HasOpenLetter(iterPos))
            {
                horizontalWord = letterBoard[iterPos.x, iterPos.y].Letter + horizontalWord;
                iterPos += leftDir;
            }
            iterPos = pos + rightDir; 
            while (iterPos.y < width && letterBoard[iterPos.x, iterPos.y] != null && HasOpenLetter(iterPos))
            {
                LetterModel letterModel = letterBoard[iterPos.x, iterPos.y]; 
                horizontalWord += letterBoard[iterPos.x, iterPos.y].Letter;
                iterPos += rightDir;
            }
            iterPos = pos;
            while (iterPos.x < height && letterBoard[iterPos.x, iterPos.y] != null && HasOpenLetter(iterPos))
            {
                verticalWord = letterBoard[iterPos.x, iterPos.y].Letter + verticalWord;
                iterPos += upDir;
            }
            iterPos = pos + downDir; 
            while (iterPos.x >= 0 && letterBoard[iterPos.x, iterPos.y] != null && HasOpenLetter((iterPos)))
            {
                verticalWord += letterBoard[iterPos.x, iterPos.y].Letter;
                iterPos += downDir;
            }

            _onHintedWordFound(horizontalWord);
            _onHintedWordFound(verticalWord);
            Debug.Log("hori word: " + horizontalWord + " - verti word: " + verticalWord);
        }

        bool HasOpenLetter(Vector2Int pos)
        {
            if (letterBoard[pos.x, pos.y] != null 
                && (letterBoard[pos.x, pos.y].IsHinted || letterBoard[pos.x, pos.y].IsOpen))
            {
                return true;
            }
            return false;
        }

        void HintLetter(Vector2Int pos)
        {
            letterBoard[pos.x, pos.y].IsHinted = true;
        }

        List<Vector2Int> GetAllUnOpenLetter()
        {
            int height = letterBoard.GetLength(0);
            int width = letterBoard.GetLength(1);
            LetterModel startLetter = null;
            for (int i = 0; i < letterBoard.GetLength(0); i++)
            {
                if (startLetter != null) break;
                for (int j = 0; j < letterBoard.GetLength(1); j++)
                {
                    if (letterBoard[i, j] != null)
                    {
                        startLetter = letterBoard[i, j];
                        break;
                    }
                }
            }
            List<Vector2Int> allUnOpenLetterPos = new List<Vector2Int>();
            if (startLetter == null)
            {
                Debug.LogError("All letter is nul in board. Noooooo");
                return allUnOpenLetterPos;
            } else if (!startLetter.IsHinted && !startLetter.IsOpen)
            {
                allUnOpenLetterPos.Add(startLetter.Position);
            }
            bool[,] visited = new bool[height, width];
            Debug.Log("Height: " + height + " width: " + width + " " + startLetter.Position);
            Queue<LetterModel> Q = new Queue<LetterModel>();
            Q.Enqueue(startLetter);
            visited[startLetter.Position.x, startLetter.Position.y] = true;
            Vector2Int[] directions = 
            {
                new Vector2Int(-1, 0), 
                new Vector2Int(1, 0), 
                new Vector2Int(0, -1), 
                new Vector2Int(0, 1), 
            };
            while (Q.Count > 0)
            {
                LetterModel curLetterModel = Q.Dequeue();
                Vector2Int curPos = curLetterModel.Position;
                for (int i = 0; i < 4; i++)
                {
                    int newX = curPos.x + directions[i].x;
                    int newY = curPos.y + directions[i].y;
                    if (newX >= 0 && newX < height && newY >= 0 && newY < width && letterBoard[newX, newY] != null && !visited[newX, newY])
                    {
                        if (!letterBoard[newX, newY].IsOpen && !letterBoard[newX, newY].IsHinted)
                        {
                            allUnOpenLetterPos.Add(new Vector2Int(newX, newY));
                        }
                        visited[newX, newY] = true;
                        Q.Enqueue(letterBoard[newX, newY]);
                    }
                }
            }

            return allUnOpenLetterPos;
        }
        
        public WordModel FindWordPosition(string word)
        {
            Debug.Log("word: " + word);
            int wordLen = word.Length;
            int boardHeight = letterBoard.GetLength(0); 
            int boardWidth = letterBoard.GetLength(1);
            List<string> horizontalWordLines = GetHorizontalWordLines();
            string log = "horizontal\n";
            for (int i = 0; i < horizontalWordLines.Count; i++)
            {
                log += "###" + horizontalWordLines[i] + "###\n";
            }
            log += "--------------------\n";
            WordModel resultWordModel = new WordModel();
            resultWordModel.Word = word;
            bool hasRes = false;
            for (int i = 0; i < horizontalWordLines.Count; i++)
            {
                int wordIndex = horizontalWordLines[i].IndexOf(word);
                if (wordIndex != -1)
                {
                    Debug.Log("hori word index not -1");
                    bool checkLeftRight = true;
                    if (wordIndex - 1 >= 0 && letterBoard[i, wordIndex - 1] != null)
                    {
                        checkLeftRight = false;
                    }
                    if (checkLeftRight)
                    {
                        if (wordIndex + wordLen < boardWidth && letterBoard[i, wordIndex + wordLen] != null)
                        {
                            checkLeftRight = false;
                        }
                    }
                    if (checkLeftRight)
                    {
                        resultWordModel.Direction = new Vector2Int(0, 1);
                        resultWordModel.StartPosition = new Vector2Int(i, wordIndex);
                        hasRes = true;
                        break;
                    }
                }
            }
            if (!hasRes)
            {
                log += "vertical word\n";
                List<string> verticalWordLines = GetVerticalWordLines();
                for (int i = 0; i < verticalWordLines.Count; i++)
                {
                    log += "###" + verticalWordLines[i] + "###\n";
                }
                log += "---------------------\n";
                for (int i = 0; i < verticalWordLines.Count; i++)
                {
                    // word index start from top to bottom
                    int wordIndex = verticalWordLines[i].IndexOf(word);
                    if (wordIndex != -1)
                    {
                        Debug.Log("vertical word index not -1");
                        // convert to start from bottom to top
                        int actualIndex = boardHeight - 1 - (wordIndex + wordLen - 1);
                        bool checkTopBottom = true;
                        if (actualIndex - 1 >= 0 && letterBoard[actualIndex - 1, i] != null)
                        {
                            checkTopBottom = false;
                        }
                        if (checkTopBottom)
                        {
                            if (actualIndex + wordLen < boardHeight && letterBoard[actualIndex + wordLen, i] != null)
                            {
                                checkTopBottom = false;
                            }
                        }
                        if (checkTopBottom)
                        {
                            resultWordModel.Direction = new Vector2Int(-1, 0);
                            resultWordModel.StartPosition = new Vector2Int(actualIndex + wordLen - 1,i);
                            hasRes = true;
                            break;
                        }
                    }
                }
            }
            Debug.Log(log);
            if (!hasRes) Debug.LogError("Impossibleeeeeeeeeeeeeeeeeeee");
            OpenWord(resultWordModel);
            return resultWordModel;
        }

        void OpenWord(WordModel wordModel)
        {
            Vector2Int pos = wordModel.StartPosition;
            int wordLen = wordModel.Word.Length;
            for (int i = 0; i < wordLen; i++)
            {
                letterBoard[pos.x, pos.y].IsOpen = true;
                pos += wordModel.Direction;
            }
        }

        List<string> GetHorizontalWordLines()
        {
            List<string> res = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            int boardWidth = letterBoard.GetLength(1);
            int boardHeight = letterBoard.GetLength(0);
            for (int i = 0; i < boardHeight; i++)
            {
                stringBuilder.Clear();
                for (int j = 0; j < boardWidth; j++)
                {
                    if (letterBoard[i, j] == null)
                    {
                        stringBuilder.Append(' ');
                    }
                    else
                    {
                        stringBuilder.Append(letterBoard[i, j].Letter);
                    }
                }
                res.Add(stringBuilder.ToString());
            }
            return res;
        }

        List<string> GetVerticalWordLines()
        {
            List<string> res = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            int boardWidth = letterBoard.GetLength(1);
            int boardHeight = letterBoard.GetLength(0);
            for (int j = 0; j < boardWidth; j++)
            {
                stringBuilder.Clear();
                for (int i = boardHeight - 1; i >= 0; i--)
                {
                    if (letterBoard[i, j] == null)
                    { 
                        stringBuilder.Append(' ');
                    }
                    else
                    {
                        stringBuilder.Append(letterBoard[i, j].Letter);
                    }
                }
                res.Add(stringBuilder.ToString());
            }
            return res;
        } 
    }
}