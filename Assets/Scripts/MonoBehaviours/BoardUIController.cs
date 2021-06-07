using System.Collections.Generic;
using UnityEngine;

public class BoardUIController : MonoBehaviour 
{
    [SerializeField] BoardCell cellPrefab;
    [SerializeField] RectTransform cellParent;
    [SerializeField] float cellMargin = 2f;
    LevelData data;
    BoardCell[,] uiBoard;
    char[,] charBoard;
    float boardScreenWidth, boardScreenHeight;
    RectTransform rectTrans;
    float cellSize;
    BoardLogicController boardController;
    BoardDataGenerator2 boardDataGenerator;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        boardScreenWidth = rectTrans.rect.width;
        boardScreenHeight = rectTrans.rect.height;
    }

    private void Start() {
        Debug.Log("Phuong");
    }

    public void Initialize(LevelData levelData)
    {
        data = levelData;

        boardDataGenerator = new BoardDataGenerator2(data);
        charBoard = boardDataGenerator.GenerateBoard();

        // cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetMaxColHeight(), boardScreenWidth / boardDataGenerator.GetMaxCellWidth());
        // Debug.Log($"{boardScreenHeight}/{boardDataGenerator.GetMaxColHeight()}  --- {boardScreenWidth} / {boardDataGenerator.GetMaxCellWidth()}");
        // Debug.LogError("Cell size: " + cellSize);
        cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetBoardHeight(), boardScreenWidth / boardDataGenerator.GetBoardWidth());
        Debug.LogError("Cell size: " + cellSize + "-" + (cellSize * boardDataGenerator.GetBoardWidth()) + "-" + boardScreenWidth);
        if (cellSize * boardDataGenerator.GetBoardWidth() < boardScreenWidth) {
            Debug.LogError("in here");
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * boardDataGenerator.GetBoardWidth())) / 2.0f, cellParent.anchoredPosition.y); 
        }

        uiBoard = new BoardCell[charBoard.GetLength(0), charBoard.GetLength(1)];

        boardController = new BoardLogicController(uiBoard);
        GenerateBoard();
    }
    public void ShuffleBoard(LevelData levelData)
    {
        ClearUIBoard();
        boardDataGenerator = new BoardDataGenerator2(data);
        charBoard = boardDataGenerator.GenerateBoard();
        // Debug.Log($"virtual: {boardDataGenerator.GetMaxCellWidth()}-{boardDataGenerator.GetMaxColHeight()} ---{boardDataGenerator.GetBoardWidth()}-{boardDataGenerator.GetBoardHeight()}");
        // cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetMaxColHeight(), boardScreenWidth / boardDataGenerator.GetMaxCellWidth());
        // Debug.Log($"{boardScreenHeight}/{boardDataGenerator.GetMaxColHeight()}  --- {boardScreenWidth} / {boardDataGenerator.GetMaxCellWidth()}");
        // Debug.LogError("Cell size: " + cellSize);
        cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetBoardHeight(), boardScreenWidth / boardDataGenerator.GetBoardWidth());
        Debug.LogError("Cell size: " + cellSize + "-" + (cellSize * boardDataGenerator.GetBoardWidth()) + "-" + boardScreenWidth);
        if (cellSize * boardDataGenerator.GetBoardWidth() < boardScreenWidth) {
            Debug.LogError("in here");
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * boardDataGenerator.GetBoardWidth())) / 2.0f, cellParent.anchoredPosition.y); 
        } else {
            cellParent.anchoredPosition = new Vector2(0f, cellParent.anchoredPosition.y); 
        }

        uiBoard = new BoardCell[charBoard.GetLength(0), charBoard.GetLength(1)];
        boardController = new BoardLogicController(uiBoard);
        GenerateBoard();
    }

    void ClearUIBoard()
    {
        for (int j = 0; j < uiBoard.GetLength(1); j++)
        {
            for (int i = 0; i < uiBoard.GetLength(0); i++)
            {
                if (uiBoard[i, j])
                {
                    Destroy(uiBoard[i, j].gameObject);
                    uiBoard[i, j] = null;
                }
            }
        }
    }

    void GenerateBoard()
    {
        for (int i = 0; i < uiBoard.GetLength(0); i++)
        {
            for (int j = 0; j < uiBoard.GetLength(1); j++)
            {
                //if (data.ContainLetter(i, j))
                if (charBoard[i, j] != ' ')
                {
                    BoardCell newCell = Instantiate(cellPrefab, cellParent);
                    newCell.SetLetter(charBoard[i, j]);
                    newCell.SetPositionInBoard(new Vector2Int(i, j));
                    newCell.SetCellSizeAndMargin(cellSize, cellMargin);
                    RectTransform newCellRectTrans = newCell.GetComponent<RectTransform>();
                    newCellRectTrans.anchorMin = Vector2.zero;
                    newCellRectTrans.anchorMax = Vector2.zero;
                    newCellRectTrans.pivot = Vector2.zero;
                    newCellRectTrans.anchoredPosition = new Vector2(j * cellSize, i * cellSize);
                    uiBoard[i, j] = newCell;
                } 
            }
        }
    }

    public void ChangeCellsColor(Vector2Int startPosition, Vector2Int endPosition, bool activeColor = true)
    {
        List<Vector2Int> cellPositionsToChange = boardController.GetCells(startPosition, endPosition); 
        if (!activeColor)
        {
            cellPositionsToChange.AddRange(boardController.GetVerticalAndHorizontalCellsFromCell(startPosition));
        }
        for (int i = 0; i < cellPositionsToChange.Count; i++)
        {
            Vector2Int pos = cellPositionsToChange[i];
            uiBoard[pos.x, pos.y].ChangeColor(activeColor);
        }
    }

    public void RemoveCellsAndUpdateBoard(Vector2Int startPosition, Vector2Int endPosition)
    {
        List<BoardCell> cellsToRemove = boardController.GetCellsToRemoveAndUpdateOtherCellsPosition(startPosition, endPosition);
        for (int i = 0; i < cellsToRemove.Count; i++)
        {
            Destroy(cellsToRemove[i].gameObject);
            cellsToRemove[i] = null;
        }
        UpdateCellsPosition();
    }

    private void UpdateCellsPosition()
    {
        for (int j = 0; j < uiBoard.GetLength(1); j++)
        {
            for (int i = 0; i < uiBoard.GetLength(0); i++)
            {
                if (uiBoard[i, j])
                {
                    uiBoard[i, j].UpdateAnchoredPosition();
                }
            }
        }
    }

    public string GetWord(Vector2Int startPosition, Vector2Int endPosition)
    {
        return boardController.GetWord(startPosition, endPosition);
    }

    public char GetLetter(int x, int y)
    {
        return uiBoard[x, y].GetLetter();
    }
}
