using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTileViewModel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI letterTxt;
    Vector2Int pos;
    char letter;
    RectTransform rectTransform;
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Init(char letter, int posX, int posY, float size) {
        SetLetter(letter);
        SetPosition(posX, posY, size);
        rectTransform.sizeDelta = new Vector2(size, size);
    }
    public void SetLetter(char letter) {
        letterTxt.text = letter.ToString();
        this.letter = letter;
    } 
    public void SetPosition(int x, int y, float size) {
        pos = new Vector2Int(x, y);
        rectTransform.anchoredPosition = size * (new Vector2(x, y) + Vector2.one * .5f);
    }
}
