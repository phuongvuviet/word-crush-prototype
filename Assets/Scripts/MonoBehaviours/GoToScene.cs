using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    [SerializeField] int sceneIndex = 0;

    public void StartLoadingScene() {
        SceneManager.LoadScene(sceneIndex);
    }
}
