using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingWordController : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] Letter letterPrefab;
    public void MoveWord(string word, Vector2 startSize, Vector2 targetSize, List<Vector2> fromPositions, List<Vector2> toPositions, Action callback) {
        bool hasInvolved = false;
        for (int i = 0; i < word.Length; i++) {
            Letter letterInstance = Instantiate(letterPrefab, transform); 
            letterInstance.SetLetter(word[i]);
            letterInstance.SetSize(startSize);
            letterInstance.transform.position = fromPositions[i];//new Vector3(fromPositions[i].x, fromPositions[i].y, 0); 
            letterInstance.GetComponent<RectTransform>().DOSizeDelta(targetSize, duration);
            letterInstance.transform.DOMove(toPositions[i], duration).OnComplete(() => {
                if (!hasInvolved) {
                    callback?.Invoke();
                    hasInvolved = true;
                }
                Destroy(letterInstance.gameObject);
            });
        }
    }
}
