using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinDialog : MonoBehaviour
{
    public void LoadNextLevel() {
        gameObject.SetActive(false);
        GamePlayController.Instance.LoadCurrentLevel();
    }
}
