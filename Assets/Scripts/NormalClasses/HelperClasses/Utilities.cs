using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities 
{
    public static Vector2Int GetNextBeginPosition(Vector2Int fromPos, Vector2Int toPos) {
        if (fromPos == toPos) return fromPos;
        Vector2Int direction = GetDirection(fromPos, toPos); 
        Vector2Int res = fromPos + direction; 
        // Debug.Log("from: " + fromPos + " to: " + toPos + " res: " + res);
        return res;
    }
    public static Vector2Int GetPreLastPosition(Vector2Int fromPos, Vector2Int toPos) {
        if (fromPos == toPos) return fromPos;
        Vector2Int direction = GetDirection(fromPos, toPos); 
        Vector2Int res = toPos - direction; 
        // Debug.Log("from: " + fromPos + " to: " + toPos + " res: " + res);
        return res;
    }
    public static bool IsInside(Vector2Int vec, Vector2Int fromPos, Vector2Int toPos) {
        if (fromPos == toPos) {
            return vec == fromPos;
        }
        if (fromPos.x == toPos.x) {
            int minY = Mathf.Min(fromPos.y, toPos.y);
            int maxY = Mathf.Max(fromPos.y, toPos.y);
            return vec.y >= minY && vec.y <= maxY; 
        } else if (fromPos.y == toPos.y) {
            int minX = Mathf.Min(fromPos.x, toPos.x);
            int maxX = Mathf.Max(fromPos.x, toPos.x);
            return vec.x >= minX && vec.x <= maxX;
        }
        return false;
    } 
    public static Vector2Int GetDirection(Vector2Int fromPos, Vector2Int toPos) {
        Vector2 direction = toPos - fromPos;
        direction.Normalize();
        return new Vector2Int((int)direction.x, (int)direction.y);
    }
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int rdIndex = UnityEngine.Random.Range(0, i + 1);
            T tmpValue = list[i];
            list[i] = list[rdIndex];
            list[rdIndex] = tmpValue;
        }
    }
}
