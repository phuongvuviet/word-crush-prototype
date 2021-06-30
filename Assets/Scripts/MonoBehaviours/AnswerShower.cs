using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class AnswerShower : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI answerTxt; 

    string word;
    public void SetWord(string word) {
        this.word = word;
        answerTxt.text = word;
    }
    public string GetWord() {
        return word;
    }
    public void Show() {
        transform.DOScale(1.2f, .1f).OnComplete(() => {
            transform.DOScale(1.0f, .1f);
        });
    }
}
