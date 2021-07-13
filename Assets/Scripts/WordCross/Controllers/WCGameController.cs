using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCross{
    public class WCGameController 
    {
        BoardGenerator boardDataGenerator;

        public WCGameController(DataModel levelData)
        {
            boardDataGenerator = new BoardGenerator(levelData);
        }
        public BoardGenerator GetBoardGenerator() {
            return boardDataGenerator;
        }
    }
}
