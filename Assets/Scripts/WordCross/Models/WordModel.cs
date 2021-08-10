using System;
using UnityEngine;

namespace WCross
{
    //[System.Serializable]
    public class WordModel 
    {
        public string Word;
        public Vector2Int StartPosition;
        public Vector2Int Direction;
        public override string ToString()
        {
            return $"W: {Word} - Pos: {StartPosition} - Dir: {Direction}";
        }
    }
}