using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class SellectingWordViewModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _wordTxt;
    private RectTransform _rectTransform;
    string _word = "";

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Init()
    {
        _rectTransform.sizeDelta = new Vector2(0f, _rectTransform.sizeDelta.y);
        _wordTxt.text = "";
    }

    public void ChangeSelectingWord(string newWord)
    {
        // Debug.Log("Word: " + newWord);
        _word = newWord;
        _wordTxt.text = _word;
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _wordTxt.preferredWidth + 30);
        // Debug.Log("preffered width: " + _wordTxt.preferredWidth);
    }
}
