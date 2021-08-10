using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace WCross{
    public class GameViewModel : MonoBehaviour
    {
        [SerializeField] LetterBoardViewModel _boardViewModel;
        [SerializeField] WheelViewModel _wheelViewMode;
        [SerializeField] GameObject _winDialog;
        [SerializeField] FloatingWordViewModel _floatingWrodViewMode;

        private WCGameController _gameController;
        private SaveLoadProgressController _saveLoadContorller;
        private GameState _curState = GameState.NORMAL;

        enum GameState
        {
            NORMAL,
            WIN
        }

        private void Start() {
            _saveLoadContorller = new SaveLoadProgressController();
            StartGame();
        }

        public void StartGame()
        {
            LevelDataModel levelDataModel = _saveLoadContorller.LoadLevelData(_saveLoadContorller.CurrentLevel);
            _gameController = new WCGameController(levelDataModel, OnWordFound, OnLevelCompleted, ShowHintedLetter);
            _boardViewModel.Init(_gameController.GetBoardGenerator().LetterBoard);
            _wheelViewMode.Init(levelDataModel.Letters, OnWordSelected);
            _curState = GameState.NORMAL;
        }

        void OnWordSelected(string word)
        {
            _gameController.ValidateWord(word);
        }

        void OnWordFound(WordModel wordModel)
        {
            _floatingWrodViewMode.MoveWord(_boardViewModel.GetLetterTileList(wordModel), () =>
            {
                if (_curState == GameState.WIN)
                {
                    _boardViewModel.ShowWord(wordModel, ShowWinScreen);
                }
                else
                {
                    _boardViewModel.ShowWord(wordModel, null);
                } 
            });
        }

        void ShowWord()
        {
            
        }

        void ShowWinScreen()
        {
            _winDialog.SetActive(true);
        }

        void OnLevelCompleted()
        {
            _saveLoadContorller.CurrentLevel++;
            _curState = GameState.WIN;
        }

        void ShowHintedLetter(Vector2Int pos)
        {
            _boardViewModel.ShowHintedLetter(pos);
        }

        public void BtnHintClicked()
        {
            _gameController.Hint();
        }
    }
}