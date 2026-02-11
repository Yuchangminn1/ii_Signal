using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestionInfo
{
    string _question;
    public string Question
    {
        get { return _question; }
        set { _question = value; }
    }
    string[] _selection;
    public string[] Selection
    {
        get { return _selection; }
        set { _selection = value; }
    }
}

public class QuestionScript : MonoBehaviour, IQuestionTarget
{

    List<QuestionInfo> questionInfos = new List<QuestionInfo>(16);

    QuestionTutorialPopup tutorialPopup;

    int currentIndex = 0;
    MorseImageContainer morseImageContainer;

    public Text QuestionText;


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

        tutorialPopup = GetComponentInChildren<QuestionTutorialPopup>();
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



        currentIndex = 0;
        QuestionText.text = questionInfos[currentIndex].Question;
        questionTextContainer.Reset();
        questionTextContainer.SetSelectedOption(questionInfos[currentIndex].Selection);
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
        if (currentIndex >= questionInfos.Count - 1)
        {
            yield return delayWait;
            endTrigger?.TriggerOn();
            yield break;
        }
        if (currentIndex == 0)
        {
            for (int i = 0; i < tutorialPopup.GetTextCount(); i++)
            {
                tutorialPopup.SetText(i);
                yield return CoroutineReturnManager.GetWaitForSeconds(2.5f);
            }

        }

        currentIndex++;

        FadeManager.Instance.SetAlphaZero(QuestionText);
        FadeManager.Instance.SetAlphaZero(questionTextContainer.GetCanvasGroup());

        morseImageContainer.Reset();
        FadeManager.Instance.TargetFade(_resetContainerCanvasGroup, 0f, FadeManager.Instance.FadeDuration);

        yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);

        ResetContainer?.SetActive(false);

        yield return delayWait;
        ResetContainer?.SetActive(true);

        QuestionText.text = questionInfos[currentIndex].Question;
        questionTextContainer.SetSelectedOption(questionInfos[currentIndex].Selection);
        FadeManager.Instance.TargetFade(_resetContainerCanvasGroup, 1f, FadeManager.Instance.FadeDuration);

        yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);

        pageBase.ResetValue();

        _nextQuestionCoroutine = null;
    }

    // IQuestionTarget 구현
    public void Initialize(List<QuestionInfo> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log($"{i} : {items[i].Question}");
        }
        questionInfos = items;

        Debug.Log("로드된 질문 수: " + items.Count);
    }

    public List<QuestionInfo> Data()
    {
        return questionInfos;
    }

    // 헬퍼들: 로드 데이터가 있으면 우선 사용, 없으면 기존 배열로 폴백




}
