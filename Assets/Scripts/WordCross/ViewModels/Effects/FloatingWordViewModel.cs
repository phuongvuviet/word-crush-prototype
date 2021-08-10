using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingWordViewModel : MonoBehaviour
{
    [SerializeField] FloatingLetterViewModel _floatingLetterPrefab;
    [SerializeField] float letterSize = 50f;


    public void MoveWord(List<LetterTileViewModel> destinationTiles, Action onComplete)
    {
        StartCoroutine(COMoveWord(destinationTiles, onComplete));
    }

    IEnumerator COMoveWord(List<LetterTileViewModel> destinationTiles, Action onComplete)
    {
        for (int i = 0; i < destinationTiles.Count; i++)
        {
            LetterTileViewModel curDestinationTile = destinationTiles[i];
            FloatingLetterViewModel floatingLetter = Instantiate(_floatingLetterPrefab, transform);
            floatingLetter.SetLetterText(destinationTiles[i].GetLetter());
            floatingLetter.SetRectSize(Vector2.one * letterSize);
            floatingLetter.SetAnchoredPosition(Vector2.zero);
            if (i == destinationTiles.Count - 1)
            {
                floatingLetter.MoveTo(curDestinationTile.transform.position, () =>
                {
                    curDestinationTile.ShowLetter();
                    onComplete?.Invoke();
                });
            }
            else
            {
                floatingLetter.MoveTo(curDestinationTile.transform.position, null);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
