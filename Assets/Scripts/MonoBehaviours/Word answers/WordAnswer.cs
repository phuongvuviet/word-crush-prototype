using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordAnswer : MonoBehaviour
{
    [SerializeField] WordAnswerCell cellPrefab;
    string answerStr = "";
    List<WordAnswerCell> cells = new List<WordAnswerCell>();


    public void SetAnswer(string word)
    {
        answerStr = word;
        for (int i = 0; i < word.Length; i++)
        {
            WordAnswerCell cellInstance = Instantiate(cellPrefab, transform);
            cellInstance.SetLetter(word[i]);
            cells.Add(cellInstance);
        }
    }
    public void Show()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Show();
        }
    }
    public string GetAnswer()
    {
        return answerStr;
    }
}
