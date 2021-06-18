using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingWordController : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] Letter letterPrefab;
    public void MoveWord(string word, Vector2 startSize, Vector2 targetSize, List<Vector2> fromPositions, List<Vector2> toPositions, Action callback) {
        // for (int i = 0; i < fromPositions.Count; i++) {
        //     Debug.Log("i: " + fromPositions[i]);
        // }
        // Debug.Log("-----------------------------------");
        // Debug.Log("Size: " + startSize);
        for (int i = 0; i < word.Length; i++) {
            Letter letterInstance = Instantiate(letterPrefab, transform); 
            letterInstance.SetLetter(word[i]);
            letterInstance.SetSize(startSize);
            letterInstance.transform.position = fromPositions[i];//new Vector3(fromPositions[i].x, fromPositions[i].y, 0); 
            letterInstance.GetComponent<RectTransform>().DOSizeDelta(targetSize, duration);
            letterInstance.transform.DOMove(toPositions[i], duration).OnComplete(() => {
                callback?.Invoke();
                Destroy(letterInstance.gameObject);
            });

            // Vector3 localPos = letterInstance.transform.localPosition;
            // localPos.z = 0;
            // letterInstance.transform.localPosition = localPos;

            // Debug.Log("Pos: " + letterInstance.transform.position);
            // Debug.Log("Anchored: " + letterInstance.GetComponent<RectTransform>().anchoredPosition);
            // Debug.Log("Local: " + letterInstance.transform.localPosition);
            // Debug.Log("to pos: " + toPositions[i]);
        }
    }
}
