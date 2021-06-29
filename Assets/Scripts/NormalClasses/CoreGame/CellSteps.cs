using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSteps 
{
    public List<Vector2Int> CellsToDeletes = new List<Vector2Int>();
    public List<List<MoveInfo>> Steps = new List<List<MoveInfo>>();
    public List<MoveInfo> HorizontallyCollapsedSteps = new List<MoveInfo>();
}
public struct MoveInfo{
    public Vector2Int FromPosition;
    public Vector2Int ToPosition;
    public MoveInfo(Vector2Int fromPos, Vector2Int toPos) {
        FromPosition = fromPos;
        ToPosition = toPos; 
    }
    public override string ToString() {
        return $"{FromPosition.x}:{FromPosition.y} -> {ToPosition.x}:{ToPosition.y}";
    }
}

