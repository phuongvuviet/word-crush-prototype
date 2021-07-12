using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WinDialog : MonoBehaviour
{
    [SerializeField] CanvasGroup chapterRewardCanvasGroup, themeBonusCanvasGroup;
    [SerializeField] Image chapterRewardProgressImg, themeBonusProgressImg;  
    [SerializeField] Button levelBtn;
    [SerializeField] TextMeshProUGUI chapterProgressTxt, themeProgressTxt, subjectTxt;

    private void OnEnable() {
        // Debug.LogError("Current level: " + Prefs.CurrentLevel);
        int preProgressValue = (Prefs.CurrentLevel - 2) % 10;
        chapterProgressTxt.text = $"{preProgressValue}/10"; 
        themeProgressTxt.text = $"{preProgressValue}/10"; 
        chapterRewardProgressImg.fillAmount = (preProgressValue / 10f); 
        themeBonusProgressImg.fillAmount = (preProgressValue / 10f); 
        HideCanvasGroup(chapterRewardCanvasGroup);
        HideCanvasGroup(themeBonusCanvasGroup);
        levelBtn.gameObject.SetActive(false);
        subjectTxt.gameObject.SetActive(false);
        StartCoroutine(COShowUI());
    }
    IEnumerator COShowUI() {
        int curProgressValue = (Prefs.CurrentLevel - 1) % 10; 
        if (curProgressValue == 0) curProgressValue = 10;

        yield return new WaitForSeconds(.5f);
        subjectTxt.gameObject.SetActive(true);
        ShowCanvasGroup(chapterRewardCanvasGroup);
        // yield return new WaitForSeconds(.2f);
        yield return chapterRewardProgressImg.DOFillAmount(curProgressValue / 10f, .5f)
        .OnComplete(() => {
            chapterProgressTxt.text = $"{curProgressValue}/10"; 
        }).WaitForCompletion();
        yield return new WaitForSeconds(.5f);
        ShowCanvasGroup(themeBonusCanvasGroup);
        // yield return new WaitForSeconds(.2f);
        yield return themeBonusProgressImg.DOFillAmount(curProgressValue / 10f, .5f)
        .OnComplete(() => {
            themeProgressTxt.text = $"{curProgressValue}/10"; 
        }).WaitForCompletion();
        yield return new WaitForSeconds(.5f);
        levelBtn.gameObject.SetActive(true);
        levelBtn.transform.DOScale(1.2f, .2f)
        .OnComplete(() => {
            levelBtn.transform.DOScale(1f, .2f);
        });
    }
    public void LoadNextLevel() {
        gameObject.SetActive(false);
        GameController.Instance.LoadCurrentLevel();
    }
    public void ShowCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }
    public void HideCanvasGroup(CanvasGroup canvasGroup) {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
    }
}
