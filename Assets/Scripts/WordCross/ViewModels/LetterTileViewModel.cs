using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LetterTileViewModel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI letterTxt;
    [SerializeField] private float padding = 2f;
    Vector2Int pos;
    char letter;
    RectTransform rectTransform;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Init(char letterParam, int posX, int posY, float size) {
        letterTxt.gameObject.SetActive(false);
        SetLetter(letterParam);
        SetPosition(posX, posY, size);
        rectTransform.sizeDelta = new Vector2(size - padding, size - padding);
    }
    public void ShowLetter()
    {
        letterTxt.gameObject.SetActive(true);
    } 
    void SetLetter(char letterPram) {
        letterTxt.text = letterPram.ToString();
        letter = letterPram;
    }

    public char GetLetter()
    {
        return letter;
    }
    void SetPosition(int x, int y, float size) {
        pos = new Vector2Int(x, y);
        rectTransform.anchoredPosition = size * (new Vector2(x, y) + Vector2.one * .5f);
    }
}
