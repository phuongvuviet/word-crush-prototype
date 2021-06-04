using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataWrapper 
{
    LevelData levelData;
    int maxWordLength = 0;
    int minWordLength = 100;
    int wordsArea = 0;
    public LevelDataWrapper(LevelData levelData)
    {
        this.levelData = levelData;
    }

    public int GetMaxWordLength()
    {
        if (maxWordLength == 0)
        {
            for (int i = 0; i < levelData.Words.Count; i++)
            {
                maxWordLength = Mathf.Max(maxWordLength, levelData.Words[i].Length);
            }
        } 
        return maxWordLength;
    }
    public int GetMinWordLength()
    {
        if (minWordLength == 100)
        {
            for (int i = 0; i < levelData.Words.Count; i++)
            {
                minWordLength = Mathf.Min(maxWordLength, levelData.Words[i].Length);
            }
        } 
        return maxWordLength;
    }

    public int GetWordsArea()
    {
        if (wordsArea == 0)
        {
            for (int i = 0; i < levelData.Words.Count; i++)
            {
                wordsArea += levelData.Words[i].Length;
            }
        }
        return wordsArea;
    }
    public int Count()
    {
        return levelData.Words.Count;
    }
}
