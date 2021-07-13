using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross{
    public class GameViewModel : MonoBehaviour
    {
        [SerializeField] LetterBoardViewModel boardViewModel;
        [SerializeField] DataModel levelDataModel;

        private WCGameController _gameController;

        private void Start() {
            _gameController = new WCGameController(levelDataModel);
            boardViewModel.Init(_gameController.GetBoardGenerator().LetterBoard);
        }
    }
}