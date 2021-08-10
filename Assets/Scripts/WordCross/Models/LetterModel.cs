using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WCross
{
    [System.Serializable]
    public class LetterModel 
    {
        public char Letter;
        public Vector2Int Position;
        public bool IsOpen = false;
        public bool IsHinted = false;
    }
}
