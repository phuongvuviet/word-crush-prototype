using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  WCross
{
    public class BoardGenerator : MonoBehaviour
    {
        const int MAX_WIDTH = 10;
        const int MAX_HEIGHT = 10;
        public DataModel levelData;
        List<BoardWord> boardWords = new List<BoardWord>();
        List<string> allWords; 
        char[,] letterBoard = new char[MAX_HEIGHT, MAX_WIDTH];

        int boardGenerationCnt = 0;
        private void Start() {
            GenerateBoard();
        }

        public void GenerateBoard() {
            allWords = new List<string>(); 
            foreach (var word in levelData.Words) {
                allWords.Add(word);
            }
            Utilities.Shuffle(allWords);
            for (int i = 0; i < MAX_HEIGHT; i++) {
                for (int j = 0; j < MAX_WIDTH; j++) {
                    letterBoard[i, j] = ' ';
                }
            }
            boardWords.Clear();
            int cnt = 0;
            while (allWords.Count > 0) {
                CreateBoardWord(ChooseRandomBoardWord());
                cnt++;
                // Debug.Log("cnt: " + cnt++ + " allword: " + allWords.Count);
                if (cnt > 50) {
                    // Debug.LogError("Cann't create word");
                    // string remainWords = "";
                    // for (int i = 0; i < allWords.Count; i++) {
                    //     remainWords += allWords[i] + "--";
                    // }
                    // Debug.LogError(remainWords);
                    boardGenerationCnt ++;
                    // Debug.LogError("Board generation cnt: " + boardGenerationCnt);
                    if (boardGenerationCnt > 1000) {
                        Debug.LogError("Can can not not generate board");
                        return;
                    } else {
                        GenerateBoard();
                    }
                    return;
                } 
            }
            Debug.Log("Cnt: " + boardGenerationCnt);
            Print();
        }
        BoardWord ChooseRandomBoardWord() {
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
                boardWord.word = GetRandomWord();
                canCreate = true;
                int rd = UnityEngine.Random.Range(1, 1000000) % 2; 
                Vector2Int insertedPos = new Vector2Int(MAX_HEIGHT / 2, MAX_WIDTH / 2);
                if (rd == 0) {
                    boardWord.direction = new Vector2Int(0, 1);
                    insertedPos.y -= boardWord.word.Length / 2;
                } else {
                    boardWord.direction = new Vector2Int(-1, 0);
                    insertedPos.x += boardWord.word.Length / 2;
                }
                boardWord.startPosition = insertedPos; 
            } else {
                // Debug.Log("Pre board word not null");
                for (int i = 0; i < preBoardWord.word.Length; i++) {
                    string nextWord = GetWordContainingLetter(preBoardWord.word[i]);
                    // Debug.Log("Next word: " + nextWord);
                    if (nextWord != null) {
                        Vector2Int insertPosition = preBoardWord.startPosition + preBoardWord.direction * i;
                        // Debug.Log("Word: " + nextWord + " - Insert position: " + insertPosition);
                        int insertPointInCurWord = nextWord.IndexOf(preBoardWord.word[i]); 
                        if (ValidateInsertPosition(insertPosition, insertPointInCurWord, preBoardWord.direction, nextWord)) {
                            // Debug.LogError("Validate position");
                            if (preBoardWord.direction == new Vector2Int(0, 1)) { // right
                                boardWord.startPosition = new Vector2Int(insertPosition.x + insertPointInCurWord, insertPosition.y); 
                                boardWord.direction = new Vector2Int(-1, 0);
                            } else { // down
                                boardWord.startPosition = new Vector2Int(insertPosition.x, insertPosition.y - insertPointInCurWord);
                                boardWord.direction = new Vector2Int(0, 1);
                            }
                            boardWord.word = nextWord;
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
                allWords.Remove(boardWord.word);
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
                for (int i = 0; i < curWordLength; i++) { 
                    if (startWordPos.x < 0 || startWordPos.x >= MAX_HEIGHT || startWordPos.y < 0 || startWordPos.y >= MAX_WIDTH) {
                        return false;
                    }
                    if (letterBoard[startWordPos.x, startWordPos.y] == ' ') {
                        if ((startWordPos.y + 1 < MAX_WIDTH && letterBoard[startWordPos.x, startWordPos.y + 1] != ' ') 
                        || (startWordPos.y - 1 >= 0 && letterBoard[startWordPos.x, startWordPos.y - 1] != ' ')) {
                            return false;
                        } 
                    } else {
                        if (letterBoard[startWordPos.x, startWordPos.y] != word[i]) {
                            return false;
                        }
                    }
                    if (i == 0) {
                        if (startWordPos.x + 1 < MAX_HEIGHT && letterBoard[startWordPos.x + 1, startWordPos.y] != ' ') {
                            return false;
                        }
                    }
                    if (i == curWordLength - 1) {
                        if (startWordPos.x - 1 >= 0 && letterBoard[startWordPos.x - 1, startWordPos.y] != ' ') {
                            return false;
                        }
                    }
                    startWordPos += new Vector2Int(-1, 0);
                }
            } else { // else if preWord is vertical -> insert horizontal
                // insert horizontal
                Vector2Int startWordPos = new Vector2Int(crossedPosition.x, crossedPosition.y - insertPointInCurWord);
                for (int i = 0; i < curWordLength; i++) {
                    if (startWordPos.x < 0 || startWordPos.x >= MAX_HEIGHT || startWordPos.y < 0 || startWordPos.y >= MAX_WIDTH) {
                        return false;
                    }
                    if (letterBoard[startWordPos.x, startWordPos.y] == ' ') {
                        if (( startWordPos.x - 1 >= 0 && letterBoard[startWordPos.x - 1, startWordPos.y] != ' ') 
                            || (startWordPos.x + 1 < MAX_HEIGHT && letterBoard[startWordPos.x + 1, startWordPos.y] != ' ')) {
                            return false;
                        } 
                    } else {
                        if (letterBoard[startWordPos.x, startWordPos.y] != word[i]) {
                            return false;
                        }
                    }
                    if (i == 0) {
                        if (startWordPos.y - 1 >= 0 && letterBoard[startWordPos.x, startWordPos.y - 1] != ' ') {
                            return false;
                        }
                    }
                    if (i == curWordLength - 1) {
                        if (startWordPos.y + 1 < MAX_WIDTH && letterBoard[startWordPos.x, startWordPos.y + 1] != ' ') {
                            return false;
                        }
                    }
                    startWordPos += new Vector2Int(0, 1);
                }
            }
            return true;
        }
        void AddWordToLetterBoard(BoardWord boardWord) {
            // Debug.Log("add word: " + boardWord);
            Vector2Int startPos = boardWord.startPosition;
            string word = boardWord.word;
            for (int i = 0; i < word.Length; i++) {
                letterBoard[startPos.x, startPos.y] = word[i]; 
                startPos += boardWord.direction;
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
                    if (letterBoard[i, j] == ' ') {
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
        public string word;
        public Vector2Int startPosition;
        public Vector2Int direction;

        public override string ToString(){
            return word + " - " + startPosition + " - " + direction;
        }
    }
}
