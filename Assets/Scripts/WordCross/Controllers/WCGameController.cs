using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross{
    public class WCGameController 
    {
        BoardGenerator _boardDataGenerator;
        WCBoardController _boardController;
        List<string> _foundWords; 
        List<string> _allWords; 
        Action<WordModel> _onNewWordFound;
        Action _onLevelCompleted;
        Action<Vector2Int> _onLetterHinted;
             
        public WCGameController(LevelDataModel levelLevelData, Action<WordModel> onNewWordFound,
            Action onLevelCompleted, Action<Vector2Int> onLetterHinted)
        {
            _boardDataGenerator = new BoardGenerator(levelLevelData);
            _boardController = new WCBoardController(_boardDataGenerator.LetterBoard, ValidateWord);
            _onNewWordFound = onNewWordFound;
            _onLevelCompleted = onLevelCompleted;
            _allWords = levelLevelData.Words;
            _foundWords = new List<string>();
            _onLetterHinted = onLetterHinted;
        }
        public BoardGenerator GetBoardGenerator()
        { 
            return _boardDataGenerator;
        }

        public void ValidateWord(string word)
        {
            if (_allWords.Contains(word) && !_foundWords.Contains(word))
            {    
                _foundWords.Add(word);
                WordModel wordModel = _boardController.FindWordPosition(word);
                if (_foundWords.Count == _allWords.Count)
                {
                    _onLevelCompleted?.Invoke();
                }
                _onNewWordFound?.Invoke(wordModel);
            }
        }

        public void Hint()
        {
            Vector2Int hintPos = _boardController.HintRandomLetter();
            _onLetterHinted?.Invoke(hintPos);
        }
    }
}
