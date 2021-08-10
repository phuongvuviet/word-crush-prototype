using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class SelectingWordViewModel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _wordTxt;
    private RectTransform _rectTransform;
    string _word = "";

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ChangeSelectingWord("");
    }

    public void Init()
    {
        _rectTransform.sizeDelta = new Vector2(0f, _rectTransform.sizeDelta.y);
        _wordTxt.text = "";
    }

    public void ChangeSelectingWord(string newWord)
    {
        _word = newWord;
        _wordTxt.text = _word;
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _wordTxt.preferredWidth + 30);
    }
}
