using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross
{
    public class BoardGenerator 
    {
        const int MAX_WIDTH = 14;
        const int MAX_HEIGHT = 8;
        LevelDataModel _levelData;
        List<BoardWord> boardWords = new List<BoardWord>();
        List<string> allWords; 
        int boardGenerationCnt = 0;
        LetterModel[,] letterBoard = new LetterModel[MAX_HEIGHT, MAX_WIDTH];

        public LetterModel[,] LetterBoard => letterBoard;

        public BoardGenerator(LevelDataModel levelDataModel) {
            _levelData = levelDataModel;
            GenerateBoard();
        }

        public void GenerateBoard() {
            allWords = new List<string>(); 
            foreach (var word in _levelData.Words) {
                allWords.Add(word);
            }
            Utilities.Shuffle(allWords);
            for (int i = 0; i < MAX_HEIGHT; i++) {
                for (int j = 0; j < MAX_WIDTH; j++) {
                    letterBoard[i, j] = null;
                }
            }
            boardWords.Clear();
            int cnt = 0;
            while (allWords.Count > 0) {
                CreateBoardWord(ChooseRandomBoardWord());
                cnt++;
                // Debug.Log("cnt: " + cnt++ + " allword: " + allWords.Count);
                if (cnt > 50) {
                    boardGenerationCnt ++;
                    if (boardGenerationCnt > 1000) {
                        Debug.LogError("Can can not not generate board");
                        return;
                    } else {
                        GenerateBoard();
                    }
                    return;
                } 
            }
            int[] boundingPositions = GetBoundingBoxPositions(letterBoard); 
            int left = boundingPositions[0], right = boundingPositions[1], top = boundingPositions[2], bottom = boundingPositions[3]; 
            int newWidth = Mathf.Abs(boundingPositions[0] - boundingPositions[1]) + 1;
            int newHeight = Mathf.Abs(boundingPositions[2] - boundingPositions[3]) + 1;
            LetterModel[,] tmpLetterBoard = new LetterModel[newHeight, newWidth]; 
            for (int i = bottom; i <= top; i++) {
                for (int j = left; j <= right; j++) {
                    tmpLetterBoard[i - bottom, j - left] = letterBoard[i, j];
                    if (tmpLetterBoard[i - bottom, j - left] != null) {
                        tmpLetterBoard[i - bottom, j - left].Position = new Vector2Int(i - bottom, j - left);
                    }
                }
            }
            letterBoard = tmpLetterBoard;
        }

        /// <summary>
        /// Get 4 positions which create a smallest box containg all characters
        /// </summary>
        /// <returns></returns>
        int[] GetBoundingBoxPositions(LetterModel[,] board) {
            int height = board.GetLength(0);
            int width  = board.GetLength(1);
            int left = 0, right = board.GetLength(1) - 1, top = board.GetLength(0) - 1, bottom = 0;
            bool found = false;
            for (int i = 0; i < width; i++) {
                if (found) break;
                for (int j = 0; j < height; j++) {
                    if (board[j, i] != null) {
                        left = i;
                        found = true;
                        break;
                    }
                }
            }
            found = false;
            for (int i = width - 1; i >= 0; i--) {
                if (found) break;
                for (int j = 0; j < height; j++) {
                    if (board[j, i] != null) {
                        right = i;
                        found = true;
                        break;
                    }
                }
            }
            found = false;
            for (int i = 0; i < height; i++) {
                if (found) break;
                for (int j = 0; j < width; j++) {
                    if (board[i, j] != null) {
                        bottom = i;
                        found = true;
                        break;
                    }
                }
            }
            found = false;
            for (int i = height - 1; i >= 0; i--) {
                if (found) break;
                for (int j = 0; j < width; j++) {
                    if (board[i, j] != null) {
                        top = i;
                        found = true;
                        break;
                    }
                }
            }
            return new int[]{left, right, top, bottom};
        }

        private BoardWord ChooseRandomBoardWord() {
            if (boardWords.Count == 0) return null;
            return boardWords[UnityEngine.Random.Range(0, boardWords.Count)];
        }
        string GetRandomWord() {
            if (allWords.Count == 0) {
                Debug.LogError("Cann't get word in empty allwords");
                return null;
            } else {
                return allWords[UnityEngine.Random.Range(0, allWords.Count)]; 
            }
        }
        bool CreateBoardWord(BoardWord preBoardWord) {
            BoardWord boardWord = new BoardWord();
            bool canCreate = false;
            if (preBoardWord == null) {
                boardWord.Word = GetRandomWord();
                canCreate = true;
                int rd = UnityEngine.Random.Range(1, 1000000) % 2; 
                Vector2Int insertedPos = new Vector2Int(MAX_HEIGHT / 2, MAX_WIDTH / 2);
                if (rd == 0) {
                    boardWord.Direction = new Vector2Int(0, 1);
                    insertedPos.y -= boardWord.Word.Length / 2;
                } else {
                    boardWord.Direction = new Vector2Int(-1, 0);
                    insertedPos.x += boardWord.Word.Length / 2;
                }
                boardWord.StartPosition = insertedPos; 
            } else {
                for (int i = 0; i < preBoardWord.Word.Length; i++) {
                    string nextWord = GetWordContainingLetter(preBoardWord.Word[i]);
                    if (nextWord != null) {
                        Vector2Int insertPosition = preBoardWord.StartPosition + preBoardWord.Direction * i;
                        int insertPointInCurWord = nextWord.IndexOf(preBoardWord.Word[i]); 
                        if (ValidateInsertPosition(insertPosition, insertPointInCurWord, preBoardWord.Direction, nextWord)) {
                            if (preBoardWord.Direction == new Vector2Int(0, 1)) { // right
                                boardWord.StartPosition = new Vector2Int(insertPosition.x + insertPointInCurWord, insertPosition.y); 
                                boardWord.Direction = new Vector2Int(-1, 0);
                            } else { // down
                                boardWord.StartPosition = new Vector2Int(insertPosition.x, insertPosition.y - insertPointInCurWord);
                                boardWord.Direction = new Vector2Int(0, 1);
                            }
                            boardWord.Word = nextWord;
                            canCreate = true;
                        } else {
                            // Debug.LogError("Can not validate position");
                        }
                    }
                }
            }
            if (canCreate) {
                boardWords.Add(boardWord);
                AddWordToLetterBoard(boardWord);
                allWords.Remove(boardWord.Word);
            }
            return canCreate;
        }
        bool IsPositionValid(Vector2Int position) {
            if (position.x < 0 || position.x >= MAX_HEIGHT || position.y < 0 || position.y >= MAX_WIDTH) return false;
            return true;
        }
        // Check if insert position is valid
        bool ValidateInsertPosition(Vector2Int crossedPosition, int insertPointInCurWord, Vector2Int preBoardWordDirection, string word) {
            int curWordLength = word.Length;
            // if preWord is horizontal -> insert vertically
            if (preBoardWordDirection == new Vector2Int(0, 1)) {
                // insert vertical 
                Vector2Int startWordPos = new Vector2Int(crossedPosition.x + insertPointInCurWord, crossedPosition.y);
                int sameLetterCnt = 0;
                for (int i = 0; i < curWordLength; i++) { 
                    if (startWordPos.x < 0 || startWordPos.x >= MAX_HEIGHT || startWordPos.y < 0 || startWordPos.y >= MAX_WIDTH) {
                        return false;
                    }
                    if (letterBoard[startWordPos.x, startWordPos.y] == null)
                    {
                        sameLetterCnt = 0;
                        if ((startWordPos.y + 1 < MAX_WIDTH && letterBoard[startWordPos.x, startWordPos.y + 1] != null) 
                        || (startWordPos.y - 1 >= 0 && letterBoard[startWordPos.x, startWordPos.y - 1] != null)) {
                            return false;
                        } 
                    } else
                    {
                        if (letterBoard[startWordPos.x, startWordPos.y].Letter != word[i])
                        {
                            return false;
                        }
                        else
                        {
                            sameLetterCnt++;
                            if (sameLetterCnt >= 2) return false;
                        }
                    }
                    if (i == 0) {
                        if (startWordPos.x + 1 < MAX_HEIGHT && letterBoard[startWordPos.x + 1, startWordPos.y] != null) {
                            return false;
                        }
                    }
                    if (i == curWordLength - 1) {
                        if (startWordPos.x - 1 >= 0 && letterBoard[startWordPos.x - 1, startWordPos.y] != null) {
                            return false;
                        }
                    }
                    startWordPos += new Vector2Int(-1, 0);
                }
            } else { // else if preWord is vertical -> insert horizontal
                // insert horizontal
                int sameLetterCnt = 0;
                Vector2Int startWordPos = new Vector2Int(crossedPosition.x, crossedPosition.y - insertPointInCurWord);
                for (int i = 0; i < curWordLength; i++) {
                    if (startWordPos.x < 0 || startWordPos.x >= MAX_HEIGHT || startWordPos.y < 0 || startWordPos.y >= MAX_WIDTH) {
                        return false;
                    }
                    if (letterBoard[startWordPos.x, startWordPos.y] == null)
                    {
                        sameLetterCnt = 0;
                        if (( startWordPos.x - 1 >= 0 && letterBoard[startWordPos.x - 1, startWordPos.y] != null) 
                            || (startWordPos.x + 1 < MAX_HEIGHT && letterBoard[startWordPos.x + 1, startWordPos.y] != null)) {
                            return false;
                        } 
                    } else {
                        if (letterBoard[startWordPos.x, startWordPos.y].Letter != word[i]) {
                            return false;
                        }
                        else
                        {
                            sameLetterCnt++;
                            if (sameLetterCnt >= 2) return false;
                        }
                    }
                    if (i == 0) {
                        if (startWordPos.y - 1 >= 0 && letterBoard[startWordPos.x, startWordPos.y - 1] != null) {
                            return false;
                        }
                    }
                    if (i == curWordLength - 1) {
                        if (startWordPos.y + 1 < MAX_WIDTH && letterBoard[startWordPos.x, startWordPos.y + 1] != null) {
                            return false;
                        }
                    }
                    startWordPos += new Vector2Int(0, 1);
                }
                if (sameLetterCnt == word.Length) return false;
            }
            return true;
        }
        void AddWordToLetterBoard(BoardWord boardWord) {
            // Debug.Log("add word: " + boardWord);
            Vector2Int startPos = boardWord.StartPosition;
            string word = boardWord.Word;
            for (int i = 0; i < word.Length; i++) {
                LetterModel curLetterModel = new LetterModel();
                curLetterModel.Letter = word[i];
                curLetterModel.Position = startPos;
                letterBoard[startPos.x, startPos.y] = curLetterModel; 
                startPos += boardWord.Direction;
            }
        }
        string GetWordContainingLetter(char letter) {
            string res = null;
            for (int i = 0; i < allWords.Count; i++) {
                if (allWords[i].IndexOf(letter) != -1) {
                    res = allWords[i];
                    break;
                }
            }
            return res;
        }
        void Print() {
            string str = "";
            for (int i = letterBoard.GetLength(0) - 1; i >= 0; i--) {
                for (int j = 0; j < letterBoard.GetLength(1); j++) {
                    if (letterBoard[i, j] == null) {
                        str += "-";
                    } else {
                        str += letterBoard[i, j];
                    }
                }
                str += "\n";
            }
            Debug.Log(str);
            Debug.Log("Board word count: " + boardWords.Count);
        }
    }

    public class BoardWord{
        public string Word;
        public Vector2Int StartPosition;
        public Vector2Int Direction;

        public override string ToString(){
            return Word + " - " + StartPosition + " - " + Direction;
        }
    }
}
