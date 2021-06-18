using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUIController : MonoBehaviour 
{
    [SerializeField] BoardCell cellPrefab;
    [SerializeField] RectTransform cellParent;
    [SerializeField] float cellMargin = 2f;
    BoardCell[,] uiBoard = null;
    float boardScreenWidth, boardScreenHeight;
    RectTransform rectTrans;
    float cellSize;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        boardScreenWidth = rectTrans.rect.width;
        boardScreenHeight = rectTrans.rect.height;
    }
    public void Initialize(char[,] board) {
        if (uiBoard != null) {
            ClearUIBoard();
        }
        int boardHeight = board.GetLength(0);
        int boardWidth = board.GetLength(1);
        cellSize = Mathf.Min(boardScreenHeight / board.GetLength(0), boardScreenWidth / board.GetLength(1));
        if (cellSize * boardWidth < boardScreenWidth) {
            cellParent.anchoredPosition = new Vector2((boardScreenWidth - (cellSize * boardWidth)) / 2.0f, cellParent.anchoredPosition.y); 
        } else {
            cellParent.anchoredPosition = new Vector2(0f, cellParent.anchoredPosition.y); 
        }
        uiBoard = new BoardCell[boardHeight, boardWidth];
        GenerateBoard(board);
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

    void GenerateBoard(char[,] charBoard)
    {
        for (int i = 0; i < uiBoard.GetLength(0); i++)
        {
            for (int j = 0; j < uiBoard.GetLength(1); j++)
            {
                if (charBoard[i, j] != ' ')
                {
                    BoardCell newCell = Instantiate(cellPrefab, cellParent);
                    newCell.SetLetter(charBoard[i, j]);
                    newCell.SetCellSizeAndMargin(cellSize, cellMargin);
                    newCell.SetPositionInBoard(new Vector2Int(i, j));
                    RectTransform newCellRectTrans = newCell.GetComponent<RectTransform>();
                    newCellRectTrans.anchorMin = Vector2.zero;
                    newCellRectTrans.anchorMax = Vector2.zero;
                    uiBoard[i, j] = newCell;
                } else {
                    uiBoard[i, j] = null;
                }
            }
        }
    }

    public void SetCellsState(List<Vector2Int> cellPositions, BoardCell.BoardCellState state)
    {
        for (int i = 0; i < cellPositions.Count; i++)
        {
            Vector2Int pos = cellPositions[i];
            if (uiBoard[pos.x, pos.y]) {
                uiBoard[pos.x, pos.y].SetState(state);
            } else {
                Debug.Log("Change state of null cell : " + pos.x + "-" + pos.y);
            }
            // if (state == BoardCell.BoardCellState.ACTIVE) {
            //     yield return new WaitForSeconds(.02f);
            // }
        }
        // StartCoroutine(COSetCellsState(cellPositions, state));
    }
    public IEnumerator COSetCellsState(List<Vector2Int> cellPositions, BoardCell.BoardCellState state) {
        for (int i = 0; i < cellPositions.Count; i++)
        {
            Vector2Int pos = cellPositions[i];
            if (uiBoard[pos.x, pos.y]) {
                uiBoard[pos.x, pos.y].SetState(state);
            } else {
                Debug.Log("Change state of null cell : " + pos.x + "-" + pos.y);
            }
            if (state == BoardCell.BoardCellState.ACTIVE) {
                yield return new WaitForSeconds(.02f);
            }
        }
        yield return null;
    }
    public void SetCellState(Vector2Int position, BoardCell.BoardCellState state, bool useAnim = true) {
        uiBoard[position.x, position.y].SetState(state, useAnim);
    }
    public void SetHintedCells(List<Vector2Int> positions) {
        for (int i = 0; i < positions.Count; i++) {
            SetHintedCell(positions[i]);
        }
    }
    public void SetHintedCell(Vector2Int pos) {
        uiBoard[pos.x, pos.y].IsHinted = true;
    }

    public void RemoveCellsAndUpdateBoard(CellSteps cellSteps)
    {
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
        // UpdateCellsPosition();
    }

    // private void UpdateCellsPosition()
    // {
    //     for (int j = 0; j < uiBoard.GetLength(1); j++)
    //     {
    //         for (int i = 0; i < uiBoard.GetLength(0); i++)
    //         {
    //             if (uiBoard[i, j])
    //             {
    //                 uiBoard[i, j].UpdateAnchoredPosition();
    //             }
    //         }
    //     }
    // }
}
