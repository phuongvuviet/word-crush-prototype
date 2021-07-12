using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HintWordInfo 
{
    public string Word;
    public Vector2Int Position;
    public Vector2Int Direction;
    public int CurrentIndex;

    public HintWordInfo() {
        Reset();
    }
    public HintWordInfo(string word, Vector2Int position, Vector2Int direction) {
        Word = word;
        Position = position;
        Direction = direction;
        CurrentIndex = 0;
    }
    public Vector2Int GetNextCharPosition() {
        // Debug.Log("Word: " + Word + " Pos: " + Position + " dir: " + Direction + " index: " + CurrentIndex);
        Vector2Int res = Position + Direction * CurrentIndex;
        CurrentIndex++;
        return res;
    }
    public Vector2Int GetStartPosition() {
        return Position;
    }
    public Vector2Int GetEndPosition() {
        return Position + Direction * (Word.Length - 1);
    }
    public Vector2Int GetCurrentPosition() {
        if (CurrentIndex > 0) {
            Debug.Log("current index > 0");
            return Position + Direction * (CurrentIndex - 1);
        } else {
            Debug.Log("current index < 0");
            return Position + Direction * CurrentIndex;
        }
    }
    public bool IsCompleted() {
        return CurrentIndex == Word.Length;
    }
    public bool HasWordInfo() {
        return Word != "";
    }
    public void Reset() {
        CurrentIndex = 0;
        Word = "";
        Position = new Vector2Int(-1, -1);
    }
    public override string ToString() {
        return $"Word: {Word} - Pos: {Position} - Direction: {Direction} - Index: {CurrentIndex}";
    }
}
