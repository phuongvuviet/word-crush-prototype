using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] RectTransform pos1, pos2;
    [SerializeField] RectTransform rectTrans;

    float timer = .0f;
    float horizontalDistance = 0f, verticalDistance = 0f;
    private void Awake() {
        // rectTrans = GetComponent<RectTransform>();
        rectTrans.anchoredPosition = pos1.anchoredPosition;
        horizontalDistance = Mathf.Abs(pos1.anchoredPosition.x - pos2.anchoredPosition.x);
        verticalDistance = Mathf.Abs(pos1.anchoredPosition.y - pos2.anchoredPosition.y);
    }
    private void FixedUpdate() {
        if (timer >= 1f) return;
        Debug.Log("value: " + curve.Evaluate(timer));
        float curX = Mathf.Lerp(pos1.anchoredPosition.x, pos2.anchoredPosition.x, timer);
        float curY = Mathf.Lerp(pos1.anchoredPosition.y, pos2.anchoredPosition.y, curve.Evaluate(timer));
        rectTrans.anchoredPosition = new Vector2(curX, curY);
        timer += Time.deltaTime / 2.0f;

        // rectTrans.anchoredPosition = Vector3.Lerp(pos1.anchoredPosition, pos2.anchoredPosition, curve.Evaluate(timer));
    }
    // public void Move() {
    //     curve.Evaluate()
    // }
}
