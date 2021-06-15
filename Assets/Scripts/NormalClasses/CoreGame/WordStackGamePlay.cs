using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordStackGamePlay 
{
    BoardDataGenerator boardGenerator;
    BoardLogicController boardLogic;
    GameDataLoader gameDataLoader;

    public WordStackGamePlay(List<string> words, char[,] charBoard) {
        boardGenerator = new BoardDataGenerator(words);
        boardLogic = new BoardLogicController(charBoard, words);
        gameDataLoader = new GameDataLoader();
    }
}
