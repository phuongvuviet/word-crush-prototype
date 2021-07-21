using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace WCross
{
    public class LetterBoardViewModel : MonoBehaviour
    {
        [SerializeField] LetterTileViewModel letterTilePrefab;
        [SerializeField] RectTransform letterTileGrid;
        [SerializeField] float letterPadding = 2f;
        float screenBoardWidth, screenBoardHeight; 
        float tileSize;
        LetterTileViewModel[,] letterTiles = null;

        private void Awake()
        {
            RectTransform rectTrans = GetComponent<RectTransform>();
            screenBoardWidth = rectTrans.rect.width; 
            screenBoardHeight = rectTrans.rect.height;
        }
        public void Init(LetterModel[,] letterBoard) 
        {
            GenerateBoard(letterBoard);
        }
        void GenerateBoard(LetterModel[,] letterBoard) 
        {
            int width = letterBoard.GetLength(1);
            int height = letterBoard.GetLength(0);
            float tileSize = 0;
            letterTileGrid.anchoredPosition = Vector2.zero;
            if (screenBoardHeight / height <= screenBoardWidth / width) {
                tileSize = screenBoardHeight / height;
                letterTileGrid.anchoredPosition += new Vector2(1, 0) * (screenBoardWidth - width * tileSize) / 2f;
            } else {
                tileSize = screenBoardWidth / width;
                letterTileGrid.anchoredPosition += new Vector2(0, 1) * (screenBoardHeight - height * tileSize) / 2f;
            }
            CleanUpLetterTileViewModels();
            letterTiles = new LetterTileViewModel[height, width];
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    if (letterBoard[i, j] != null) {
                        LetterTileViewModel letterTileInstance = Instantiate(letterTilePrefab, letterTileGrid);
                        letterTileInstance.Init(letterBoard[i, j].Letter, j, i, tileSize);
                        letterTiles[i, j] = letterTileInstance;
                    }
                }
            }
        }

        public List<LetterTileViewModel> GetLetterTileList(WordModel wordModel)
        {
            List<LetterTileViewModel> resLetterTiles = new List<LetterTileViewModel>();
            Vector2Int curPos = wordModel.StartPosition;
            for (int i = 0; i < wordModel.Word.Length; i++)
            {
                resLetterTiles.Add(letterTiles[curPos.x, curPos.y]);
                curPos += wordModel.Direction;
            }
            return resLetterTiles;
        }
        public void ShowWord(WordModel wordModel, Action callback)
        {
            Vector2Int curPos = wordModel.StartPosition;
            Vector2Int direction = wordModel.Direction;
            int wordLen = wordModel.Word.Length;
            Debug.Log(wordModel);
            for (int i = 0; i < wordLen; i++)
            {
                if (letterTiles[curPos.x, curPos.y] == null) 
                {
                    Debug.LogError("Letter tile is null - pos: " + curPos.x + " - " + curPos.y + " " + letterTiles.GetLength(0) + " " + letterTiles.GetLength(1));
                }
                letterTiles[curPos.x, curPos.y].ShowLetter();
                curPos += direction;
            }
            callback?.Invoke();
        }

        public void ShowHintedLetter(Vector2Int pos)
        {
            letterTiles[pos.x, pos.y].ShowLetter();
        }
        void CleanUpLetterTileViewModels() {
            if (letterTiles != null) {
                for (int i = 0; i < letterTiles.GetLength(0); i++) {
                    for (int j = 0; j < letterTiles.GetLength(1); j++) {
                        if (letterTiles[i, j] != null)
                        {
                            Destroy(letterTiles[i, j].gameObject);
                            letterTiles[i, j] = null;
                        }
                    }
                }
            }
            letterTiles = null;
        }
    }
}
