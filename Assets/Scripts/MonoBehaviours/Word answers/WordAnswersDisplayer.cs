using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordAnswersDisplayer : MonoBehaviour
{
    [SerializeField] WordAnswer answerPrefab;

    List<WordAnswer> answers = new List<WordAnswer>();

    public void SetWordAnswers(List<string> words)
    {
        //Debug.Log("word count: " + words.Count);
        for (int i = 0; i < words.Count; i++)
        {
            WordAnswer answerInstance = Instantiate(answerPrefab, transform);
            answerInstance.SetAnswer(words[i]);
            answers.Add(answerInstance);
        }
    }
    public void ShowAnswer(string answer)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i].GetAnswer() == answer)
            {
                answers[i].Show();
                break;
            }
        }
    }
}
