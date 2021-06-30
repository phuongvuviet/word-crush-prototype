using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test1 : MonoBehaviour
{
    public int level = 1;
    public List<Transform> pos;
    public PathType pathType;
    public Transform targetTransfrom;
    Vector3 startPos;

    private void Awake() {
        PlayerPrefs.DeleteAll();
        Prefs.CurrentLevel = level;
    }

    public void Move() {
        transform.position = startPos;
        Vector3[] waypoints = new Vector3[pos.Count];
        for (int i = 0; i < pos.Count; i++) {
            waypoints[i] = pos[i].position;
        }
        transform.DOPath(waypoints, 2, pathType, PathMode.Ignore, 10, Color.red);
    }

    public void Move2() {
        transform.position = startPos;
        float horizontalDistance = Mathf.Abs(transform.position.x - targetTransfrom.position.x);
        Vector2 direction = (targetTransfrom.position - transform.position).normalized;
        Vector2 controlPoint1 = transform.position; 
        Vector2 controlPoint2 = targetTransfrom.position; 
        Debug.Log("Distance: " + horizontalDistance);
        if (transform.position.x < targetTransfrom.position.x) {
            controlPoint1 += Vector2.right * (1.5f * horizontalDistance);
            controlPoint2 += Vector2.right * (.25f * horizontalDistance);
        } else {
            controlPoint1 += Vector2.right * (1.5f * horizontalDistance);
            controlPoint2 += Vector2.left * (.25f * horizontalDistance);
        }
        Vector3[] path = new Vector3[]{targetTransfrom.position, controlPoint1, controlPoint2};
        transform.DOPath(path, 2, pathType, PathMode.Ignore, 10, Color.red).SetEase(Ease.Linear);
    }
}
