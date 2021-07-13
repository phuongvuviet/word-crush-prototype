using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WStack;

public class BoardUIController : MonoBehaviour 
{
    [SerializeField] BoardCell cellPrefab;
    [SerializeField] RectTransform cellParent;
    [SerializeField] float cellMargin = 2f;
    BoardCell[,] uiBoard = null;
    float boardCanvasWidth, boardCanvasHeight;
    RectTransform rectTrans;
    float cellSize;
    float screenTopWorldPosition;

    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        boardCanvasWidth = cellParent.rect.width;
        boardCanvasHeight = cellParent.rect.height;
        screenTopWorldPosition = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    }

    public void Initialize(char[,] board, Action callback) {
        StopAllCoroutines();
        if (uiBoard != null) {
            ClearUIBoard();
        }
        int boardHeight = board.GetLength(0);
        int boardWidth = board.GetLength(1);
        ComputeCellSize(boardWidth, boardHeight);
        uiBoard = new BoardCell[boardHeight, boardWidth];
        StartCoroutine(GenerateBoard(board, callback));
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

    IEnumerator GenerateBoard(char[,] charBoard, Action callback)
    {
        for (int i = 0; i < uiBoard.GetLength(0); i++)
        {
            StartCoroutine(GenerateBoardRow(charBoard, i));
            yield return new WaitForSeconds(.1f);
        }
        callback?.Invoke();
    }

    IEnumerator GenerateBoardRow(char[,] charBoard, int row) {
        for (int j = 0; j < uiBoard.GetLength(1); j++)
        {
            if (charBoard[row, j] != ' ')
            {
                BoardCell newCell = Instantiate(cellPrefab, cellParent);
                newCell.SetLetter(charBoard[row, j]);
                newCell.SetCellSizeAndMargin(cellSize, cellMargin);
                RectTransform newCellRectTrans = newCell.GetComponent<RectTransform>();
                newCellRectTrans.anchoredPosition = new Vector2(j, row) * cellSize + Vector2.one * cellSize / 2f;
                newCell.transform.position = new Vector2(newCell.transform.position.x, screenTopWorldPosition + 1f);
                uiBoard[row, j] = newCell;
                newCell.SetPositionInBoardWhenStartGame(new Vector2Int(row, j));
                yield return new WaitForSeconds(.05f);
            } else {
                uiBoard[row, j] = null;
            }
            // yield return null;
        }
        if (row == uiBoard.GetLength(0) - 1) {
            // GameController.Instance.SetIsAnimEnded(true);
            // GameController.Instance.IsAnimEnded = true;
            // Debug.LogError("Generate board succesful");
            GameController.Instance.ShowHintedCells();
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
        }
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
        // Debug.Log("Set hint cell pos: " + pos);
        uiBoard[pos.x, pos.y].IsHinted = true;
    }
    public void UnhintCells(List<Vector2Int> positions) {
        for (int i = 0; i < positions.Count; i++) {
            uiBoard[positions[i].x, positions[i].y].IsHinted = false;
        }
    }
    public void UnhintCells() {
        for (int i = 0; i < uiBoard.GetLength(0); i++) {
            for (int j = 0; j < uiBoard.GetLength(1); j++) {
                if (uiBoard[i, j] != null) {
                    uiBoard[i, j].IsHinted = false;
                } 
            }
        }
    }

    public void RemoveCellsAndCollapseBoard(CellSteps cellSteps, Action callback = null)
    {
        StartCoroutine(CORemoveCellslAndCollapsedBoard(cellSteps, callback));
    }
    IEnumerator CORemoveCellslAndCollapsedBoard(CellSteps cellSteps, Action callback = null) {
        for (int i = 0; i < cellSteps.CellsToDeletes.Count; i++) {
            Vector2Int cellPos = cellSteps.CellsToDeletes[i];
            Destroy(uiBoard[cellPos.x, cellPos.y].gameObject);
            uiBoard[cellPos.x, cellPos.y] = null;
        }
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < cellSteps.Steps.Count; i++) {
            for (int j = 0; j < cellSteps.Steps[i].Count; j++)
			{
				MoveInfo moveInfo = cellSteps.Steps[i][j];
				UpdateUIBoard(moveInfo);
				yield return null;
			}
			yield return null;
        }
        for (int i = 0; i < cellSteps.HorizontallyCollapsedSteps.Count; i++) {
            UpdateUIBoard(cellSteps.HorizontallyCollapsedSteps[i]);
        }
        callback?.Invoke();
        // Debug.LogError("Remove cells doneeeeeeeeeeeeeeeeeeeeeeeeeeee");
    }

	private void UpdateUIBoard(MoveInfo moveInfo)
	{
		uiBoard[moveInfo.ToPosition.x, moveInfo.ToPosition.y] = uiBoard[moveInfo.FromPosition.x, moveInfo.FromPosition.y];//.SetPositionInBoard(moveInfo.ToPosition);
		uiBoard[moveInfo.FromPosition.x, moveInfo.FromPosition.y] = null;
		uiBoard[moveInfo.ToPosition.x, moveInfo.ToPosition.y].SetPositionInBoard(moveInfo.ToPosition);
	}

    public void ShuffleBoard(List<MoveInfo> moveInfos, int boardWidth, int boardHeight) {
        StartCoroutine(COShuffleBoard(moveInfos, boardWidth, boardHeight));
    }
    IEnumerator COShuffleBoard(List<MoveInfo> moveInfos, int boardWidth, int boardHeight) {
        ComputeCellSize(boardWidth, boardHeight);
        BoardCell[,] newUIBoard = new BoardCell[boardHeight, boardWidth];
        for (int i = 0; i < moveInfos.Count; i++) {
            // Debug.Log("Shuffe : " + moveInfos[i]);
            newUIBoard[moveInfos[i].ToPosition.x, moveInfos[i].ToPosition.y] = uiBoard[moveInfos[i].FromPosition.x, moveInfos[i].FromPosition.y];
            newUIBoard[moveInfos[i].ToPosition.x, moveInfos[i].ToPosition.y].SetCellSizeAndMargin(cellSize, cellMargin);
            newUIBoard[moveInfos[i].ToPosition.x, moveInfos[i].ToPosition.y].SetPositionInBoard(moveInfos[i].ToPosition, .3f);
            yield return null;
        }
        uiBoard = newUIBoard;
        yield return new WaitForSeconds(.2f);
        GameController.Instance.IsAnimEnded = true;
    }

	public List<Vector2> GetCellWorldPositions(List<Vector2Int> positions) {
        List<Vector2> res = new List<Vector2>();
        for (int i = 0; i < positions.Count; i++) {
            res.Add(uiBoard[positions[i].x, positions[i].y].transform.position);
        }
        return res;
    }
    public Vector2 GetCellSize() {
        return new Vector2(cellSize, cellSize);
    }
    void ComputeCellSize(int boardWidth, int boardHeight) {
        cellSize = Mathf.Min(boardCanvasHeight / boardHeight, boardCanvasWidth / boardWidth);
        if (cellSize * boardWidth < boardCanvasWidth) {
            cellParent.anchoredPosition = new Vector2((boardCanvasWidth - (cellSize * boardWidth)) / 2.0f, cellParent.anchoredPosition.y); 
        } else {
            cellParent.anchoredPosition = new Vector2(0f, cellParent.anchoredPosition.y); 
        }
    }
}
