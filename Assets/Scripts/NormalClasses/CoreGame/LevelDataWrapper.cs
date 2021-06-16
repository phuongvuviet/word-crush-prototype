using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataWrapper 
{
    // LevelData levelData;
    int maxWordLength = 0;
    int minWordLength = 100;
    int wordsArea = 0;
    List<string> words;
    public LevelDataWrapper(LevelData levelData)
    {
        words = levelData.Words;
    }
    public LevelDataWrapper(List<string> words) {
        this.words = words;
    }

    public int GetMaxWordLength()
    {
        if (maxWordLength == 0)
        {
            for (int i = 0; i < words.Count; i++)
            {
                maxWordLength = Mathf.Max(maxWordLength, words[i].Length);
            }
        } 
        return maxWordLength;
    }
    public int GetMinWordLength()
    {
        if (minWordLength == 100)
        {
            for (int i = 0; i < words.Count; i++)
            {
                minWordLength = Mathf.Min(minWordLength, words[i].Length);
            }
        } 
        return minWordLength;
    }

    public int GetWordsArea()
    {
        if (wordsArea == 0)
        {
            for (int i = 0; i < words.Count; i++)
            {
                wordsArea += words[i].Length;
            }
        }
        return wordsArea;
    }
    public int Count()
    {
        return words.Count;
    }
    public List<string> GetWords() {
        return words;
    }
}
