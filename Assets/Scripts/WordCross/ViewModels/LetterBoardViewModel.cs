using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBoardViewModel : MonoBehaviour
{
    [SerializeField] LetterTileViewModel letterTilePrefab;
    [SerializeField] RectTransform letterTileGrid;
    float screenBoardWidth, screenBoardHeight; 
    float tileSize;
    LetterTileViewModel[,] letterTileViewModels = null;

    private void Awake() {
        RectTransform rectTrans = GetComponent<RectTransform>();
        screenBoardWidth = rectTrans.rect.width; 
        screenBoardHeight = rectTrans.rect.height;
    }
    public void Init(char[,] charBoard) {
        GenerateBoard(charBoard);
    }

    void GenerateBoard(char[,] charBoard) {
        int width = charBoard.GetLength(1);
        int height = charBoard.GetLength(0);
        float tileSize = 0;
        if (screenBoardHeight / height <= screenBoardWidth / width) {
            tileSize = screenBoardHeight / height;
            letterTileGrid.anchoredPosition += new Vector2(1, 0) * (screenBoardWidth - width * tileSize) / 2f;
        } else {
            tileSize = screenBoardWidth / width;
            letterTileGrid.anchoredPosition += new Vector2(0, 1) * (screenBoardHeight - height * tileSize) / 2f;
        }
        // float tileSize = ComputeTileSize(height, width);
        Debug.Log("Letter board: " + width + " - " + height + " tile size: " + tileSize);
        CleanUpLetterTileViewModels();
        letterTileViewModels = new LetterTileViewModel[height, width];
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                if (charBoard[i, j] != ' ') {
                    LetterTileViewModel letterTileInstance = Instantiate(letterTilePrefab, letterTileGrid);
                    letterTileInstance.Init(charBoard[i, j], j, i, tileSize);
                    letterTileViewModels[i, j] = letterTileInstance;
                }
            }
        }
    }
    float ComputeTileSize(int boardHeight, int boardWidth) {
        return Mathf.Min(screenBoardHeight / boardHeight, screenBoardWidth / boardWidth);
    }

    void CleanUpLetterTileViewModels() {
        if (letterTileViewModels != null) {
            for (int i = 0; i < letterTileViewModels.GetLength(0); i++) {
                for (int j = 0; j < letterTileViewModels.GetLength(1); j++) {
                    Destroy(letterTileViewModels[i, j].gameObject);
                }
            }
        }
        letterTileViewModels = null;
    }
}
