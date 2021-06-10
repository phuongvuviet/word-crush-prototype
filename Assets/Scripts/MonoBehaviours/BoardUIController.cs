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
    BoardDataGenerator boardDataGenerator;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        boardScreenWidth = rectTrans.rect.width;
        boardScreenHeight = rectTrans.rect.height;
    }
    // private void Start() {
    // }

    public void Initialize(List<string> words)
    {
        // data = levelData;
        boardDataGenerator = new BoardDataGenerator(words);
        charBoard = boardDataGenerator.GenerateBoard();
        cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetBoardHeight(), boardScreenWidth / boardDataGenerator.GetBoardWidth());
        if (cellSize * boardDataGenerator.GetBoardWidth() < boardScreenWidth) {
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * boardDataGenerator.GetBoardWidth())) / 2.0f, cellParent.anchoredPosition.y); 
        }
        uiBoard = new BoardCell[charBoard.GetLength(0), charBoard.GetLength(1)];
        boardController = new BoardLogicController(charBoard);
        GenerateBoard(charBoard);
    }
    public void Initialize(char[,] board) {
        cellSize = Mathf.Min(boardScreenHeight / board.GetLength(0), boardScreenWidth / board.GetLength(1));
        charBoard = board;
        if (cellSize * board.GetLength(1) < boardScreenWidth) {
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * board.GetLength(1))) / 2.0f, cellParent.anchoredPosition.y); 
        }
        uiBoard = new BoardCell[charBoard.GetLength(0), charBoard.GetLength(1)];
        boardController = new BoardLogicController(charBoard);
        GenerateBoard(charBoard);
    }
    public void ShuffleBoard(List<string> words)
    {
        ClearUIBoard();
        boardDataGenerator = new BoardDataGenerator(words);
        charBoard = boardDataGenerator.GenerateBoard();
        cellSize = Mathf.Min(boardScreenHeight / boardDataGenerator.GetBoardHeight(), boardScreenWidth / boardDataGenerator.GetBoardWidth());
        if (cellSize * boardDataGenerator.GetBoardWidth() < boardScreenWidth) {
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * boardDataGenerator.GetBoardWidth())) / 2.0f, cellParent.anchoredPosition.y); 
        } else {
            cellParent.anchoredPosition = new Vector2(0f, cellParent.anchoredPosition.y); 
        }

        uiBoard = new BoardCell[charBoard.GetLength(0), charBoard.GetLength(1)];
        boardController = new BoardLogicController(charBoard);
        GenerateBoard(charBoard);
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

    public void GenerateBoard(char[,] charBoard)
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
                } else {
                    uiBoard[i, j] = null;
                }
            }
        }
    }

    public void ChangeCellsColor(Vector2Int startPosition, Vector2Int endPosition, bool activeColor = true)
    {
        List<Vector2Int> cellPositionsToChange = boardController.GetAdjCellPositions(startPosition, endPosition); 
        if (!activeColor)
        {
            cellPositionsToChange.AddRange(boardController.GetVerticalAndHorizontalCellsFromCell(startPosition));
        }
        for (int i = 0; i < cellPositionsToChange.Count; i++)
        {
            Vector2Int pos = cellPositionsToChange[i];
            if (uiBoard[pos.x, pos.y]) {
                uiBoard[pos.x, pos.y].ChangeColor(activeColor);
            } else {
                // Debug.Log("ui board " + pos.x + " " + pos.y + " is null");
            }
        }
    }

    public void RemoveCellsAndUpdateBoard(Vector2Int fromPosition, Vector2Int toPosition)
    {
        CellSteps cellSteps = boardController.RemoveCellsAndCollapseRows(fromPosition, toPosition);
        for (int i = 0; i < cellSteps.CellsToDeletes.Count; i++) {
            Vector2Int cellPos = cellSteps.CellsToDeletes[i];
            // Debug.Log(cellPos.x + " " + cellPos.y);
            Destroy(uiBoard[cellPos.x, cellPos.y].gameObject);
            uiBoard[cellPos.x, cellPos.y] = null;
        }
        for (int i = 0; i < cellSteps.Steps.Count; i++) {
            Vector2Int lastPos = new Vector2Int(-1, -1);
            for (int j = 0; j < cellSteps.Steps[i].Count; j++) {
                MoveInfo moveInfo = cellSteps.Steps[i][j];
                uiBoard[moveInfo.ToPosition.x, moveInfo.ToPosition.y] = uiBoard[moveInfo.FromPosition.x, moveInfo.FromPosition.y];//.SetPositionInBoard(moveInfo.ToPosition);
                uiBoard[moveInfo.FromPosition.x, moveInfo.FromPosition.y] = null;
                uiBoard[moveInfo.ToPosition.x, moveInfo.ToPosition.y].SetPositionInBoard(moveInfo.ToPosition);
            }
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
    public char[,] GetBoardData() {
        return charBoard;
    }
}
