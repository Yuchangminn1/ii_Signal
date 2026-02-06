using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTutorialPopup : MonoBehaviour
{
    CanvasGroup _canvasGroup;
    Text[] _guideTexts;

    void Start()
    {
        _guideTexts = GetComponentsInChildren<Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetText(int index)
    {
        FadeManager.Instance.SetAlphaOne(_canvasGroup);
        for (int i = 0; i < _guideTexts.Length; i++)
        {
            if (i == index)
                FadeManager.Instance.SetAlphaOne(_guideTexts[i]);
            else
                FadeManager.Instance.SetAlphaZero(_guideTexts[i]);
        }
    }

    public int GetTextCount()
    {
        return _guideTexts.Length;
    }
}
