using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestionScript : MonoBehaviour
{

    TutorialPopup tutorialPopup;

    MorseImageContainer morseImageContainer;

    public NameText QuestionText;


    public SequenceScript endTrigger;

    public PageBase pageBase;

    public CanvasGroup TutorialPopUpObject;


    WaitForSeconds delayWait = new WaitForSeconds(1f);

    Coroutine _nextQuestionCoroutine = null;

    public GameObject ResetContainer;
    CanvasGroup _resetContainerCanvasGroup;


    QuestionSelectTextContainer questionTextContainer;
    public string jsonPath = "Json/QuestionConfig.json";



    void Awake()
    {
        pageBase = GetComponent<PageBase>();
    }

    void Start()
    {
        morseImageContainer = GetComponentInChildren<MorseImageContainer>();
        questionTextContainer = GetComponentInChildren<QuestionSelectTextContainer>();

        tutorialPopup = GetComponentInChildren<TutorialPopup>();
        _resetContainerCanvasGroup = ResetContainer.GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (GameManager.Instance.IsStarted == false)
            return;

        Reset();

        ResetContainer?.SetActive(true);

    }



    public void Reset()
    {

        QuestionManager.Instance.CurrentIndex = 0;
        Debug.Log("첫 질문 설정: " + QuestionManager.Instance.CurrentQuestionText);
        QuestionText.SetText(QuestionManager.Instance.CurrentQuestionText);
    }

    public void NextQuestion()
    {
        if (_nextQuestionCoroutine != null)
        {
            StopCoroutine(_nextQuestionCoroutine);
        }
        _nextQuestionCoroutine = StartCoroutine(NextQuestionCoroutine());
    }

    public IEnumerator NextQuestionCoroutine()
    {
        //마지막 질문 넘어서 다음장으로
        if (QuestionManager.Instance.CurrentIndex >= QuestionManager.Instance.QuestionInfos.Count - 1)
        {
            yield return delayWait;
            endTrigger?.TriggerOn();
            yield break;
        }
        if (QuestionManager.Instance.CurrentIndex == 0)
        {
            for (int i = 0; i < tutorialPopup.GetTextCount(); i++)
            {
                tutorialPopup.SetText(i);
                yield return CoroutineReturnManager.GetWaitForSeconds(3f);
            }

        }

        QuestionManager.Instance.CurrentIndex++;

        FadeManager.Instance.SetAlphaZero(QuestionText.GetTextComponent());
        FadeManager.Instance.SetAlphaZero(questionTextContainer.GetCanvasGroup());

        morseImageContainer.Reset();
        FadeManager.Instance.TargetFade(_resetContainerCanvasGroup, 0f, FadeManager.Instance.FadeDuration);

        yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);

        ResetContainer?.SetActive(false);

        yield return delayWait;
        ResetContainer?.SetActive(true);
        QuestionText.SetText(QuestionManager.Instance.QuestionInfos[QuestionManager.Instance.CurrentIndex].Question);
        FadeManager.Instance.TargetFade(_resetContainerCanvasGroup, 1f, FadeManager.Instance.FadeDuration);

        yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);

        pageBase.ResetValue();

        _nextQuestionCoroutine = null;
    }


    // 헬퍼들: 로드 데이터가 있으면 우선 사용, 없으면 기존 배열로 폴백

}
