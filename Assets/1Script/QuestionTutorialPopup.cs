using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTutorialPopup : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    Text[] _guideTexts;

    Text _currentText = null;

    void Start()
    {
        _guideTexts = GetComponentsInChildren<Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetText(int index)
    {
        StartCoroutine(FadeCoroutineCC(index));

    }

    IEnumerator FadeCoroutineCC(int index)
    {
        if (_canvasGroup.alpha < 0.9f)
            FadeManager.Instance.TargetFade(_canvasGroup, 1f, FadeManager.Instance.FadeDuration);

        if (_currentText != null)
        {
            FadeManager.Instance.TargetFade(_currentText, 0f, FadeManager.Instance.FadeDuration);
            yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);
        }
        if (_guideTexts.Length <= index)
        {
            yield break;
        }
        _currentText = _guideTexts[index];
        FadeManager.Instance.TargetFade(_currentText, 1f, FadeManager.Instance.FadeDuration);
    }

    public int GetTextCount()
    {
        return _guideTexts.Length;
    }
}
