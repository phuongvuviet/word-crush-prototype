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
        Debug.Log("Word length: " + word.Length + " fromLen: " + fromPositions.Count + " toLen: " + toPositions.Count);
        for (int i = 0; i < word.Length; i++) {
            Letter letterInstance = Instantiate(letterPrefab, transform); 
            letterInstance.SetLetter(word[i]);
            letterInstance.SetSize(startSize);
            letterInstance.transform.position = fromPositions[i];//new Vector3(fromPositions[i].x, fromPositions[i].y, 0); 
            letterInstance.GetComponent<RectTransform>().DOSizeDelta(targetSize, duration);
            MoveBenzier(letterInstance.transform, toPositions[i], () => {
                    if (!hasInvolved) {
                        Debug.Log("Move letter done");
                        callback?.Invoke();
                        hasInvolved = true;
                    }
                    Destroy(letterInstance.gameObject);
                });
        }
    }
    public void MoveBenzier(Transform transformToMove, Vector2 targetPosition, Action callback = null) {
        // transform.position = startPos;
        float horizontalDistance = Mathf.Abs(transformToMove.position.x - targetPosition.x);
        Vector2 direction = (targetPosition - (Vector2)transformToMove.position).normalized;
        Vector2 controlPoint1 = transformToMove.position; 
        Vector2 controlPoint2 = targetPosition; 
        // Debug.Log("Distance: " + horizontalDistance);
        if (transformToMove.position.x < targetPosition.x) {
            controlPoint1 += Vector2.right * (.75f * horizontalDistance);
            controlPoint2 += Vector2.down * .25f;// * (.25f * horizontalDistance);
        } else {
            controlPoint1 += Vector2.left * (.75f * horizontalDistance);
            controlPoint2 += Vector2.down * .25f;// * (.25f * horizontalDistance);
        }
        Vector3[] path = new Vector3[]{targetPosition, controlPoint1, controlPoint2};
        transformToMove.DOPath(path, .5f, PathType.CubicBezier, PathMode.Ignore, 10, Color.red).SetEase(Ease.Linear)
        .OnComplete(() => {
            callback?.Invoke();
        });
    } 
}
