using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MorseImageContainer : MonoBehaviour
{


    protected MorseInputImage[] morseInputImages;

    public ResetUIOn resetUIOn;

    public CanvasGroup[] PopupUI;

    public Graphic[] PopupUI_OffGraphics;

    public QuestionSelectTextContainer questionTextContainer;
    protected Queue<MorseType> _morseInput = new Queue<MorseType>();


    protected Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;

    protected Coroutine _morseIndexCheckCoroutine = null;

    protected float _popupDelay = 1.2f;



    //Todo 저거 좀 이벤트로 쪼개기

    protected bool isAnswer = false;



    protected int _currentIndex = 0;




    void OnEnable()
    {
        _morseIndexCheckCoroutine = null;
        if (arduino_MorseKey != null)
            arduino_MorseKey.StopMorseCheck();
    }

    public virtual void CheckStart()
    {


        isAnswer = false;
        if (resetUIOn != null)
            arduino_MorseKey.ResetUIOn = resetUIOn;
        if (arduino_MorseKey == null)
        {
            return;
        }
        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.AddOnMorseInput(ColoringMorseImage);
            arduino_MorseKey.OnMorseTransmitEnd += SetAnswerTrue;
            arduino_MorseKey.OnReset += Reset;
            arduino_MorseKey.OnReset += resetUIOn.Reset;
            arduino_MorseKey.OnReset += questionTextContainer.Reset;


            arduino_MorseKey.OnMorseTransmitEnd += questionTextContainer.SetTextColor;
        }
        _currentIndex = 0;

        _morseInput.Clear();

        arduino_MorseKey.StartMorseCheck();

    }

    virtual public void Reset()
    {
        Debug.Log("Reset");

        foreach (MorseInputImage mi in morseInputImages)
        {
            mi.Reset();
        }

        foreach (CanvasGroup popupUI in PopupUI)
        {
            FadeManager.Instance.TargetFade(popupUI, 0f, FadeManager.Instance.FadeDuration);
        }


        isAnswer = false;


        _currentIndex = 0;
    }


    virtual protected IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
    {
        isAnswer = false;

        while (morseInputImages[_currentIndex].IsFilled == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        _currentIndex++;

        if (_currentIndex == 4)
        {
            arduino_MorseKey.IsColoringDone = true;
            Debug.Log("Coloring Done");
        }

        if (_currentIndex < morseInputImages.Length && _morseInput.Count > 0)
        {
            StartCoroutine(InputDequeue());
        }

        _morseIndexCheckCoroutine = null;

    }

    IEnumerator SetAnswerTrueCoroutine(float delay)
    {
        if (delay <= 0f)
        {
            yield break;
        }
        yield return CoroutineReturnManager.GetWaitForSeconds(delay);

        if (PopupUI_OffGraphics != null)
        {
            foreach (Graphic cg in PopupUI_OffGraphics)
            {
                FadeManager.Instance.SetAlphaZero(cg);
            }
        }

        foreach (CanvasGroup popupUI in PopupUI)
        {
            FadeManager.Instance.TargetFade(popupUI, 1f, FadeManager.Instance.FadeDuration);
        }

        yield return CoroutineReturnManager.GetWaitForSeconds(FadeManager.Instance.FadeDuration);
        isAnswer = true;
    }

    public void SetAnswerTrue()
    {
        StartCoroutine(SetAnswerTrueCoroutine(_popupDelay));
    }



    public void SelectAnswer()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        if (isAnswer)
        {
            string currentData = MorseTranslator.CurrentData;
            //TODO 선택한 정보 보내는  api + tcp로 상대 기기로 정보 보내기
            if (PageController.Instance.CurrentPage == 5)
            {
                UserDataManager.Instance.GetPlayer().PassCode = currentData;

                string sendMessage = "P" + currentData;
                NetworkManager.Instance.SendData(sendMessage);

            }
            else
            {
                NetworkManager.Instance.SendData(currentData);
            }


            UserDataManager.Instance.GetPlayer().AnswerData.Enqueue(currentData);
            Debug.Log($"AnswerData 수  : {UserDataManager.Instance.GetPlayer().AnswerData.Count}");
            Debug.Log($"PartnerAnswerData 수  : {UserDataManager.Instance.GetPlayer().PartnerAnswerData.Count}");

            SequenceScript.TriggerFroceOn();
            arduino_MorseKey.StopMorseCheck();
            isAnswer = false;
        }
    }


    protected virtual void Start()
    {

        questionTextContainer = transform.parent.GetComponentInChildren<QuestionSelectTextContainer>();

        FindAnyObjectByType<Arduino_SelectButton>()._onButtonPressed += SelectAnswer;

        morseInputImages = GetComponentsInChildren<MorseInputImage>();
        if (morseInputImages.Length == 0)
        {
            Debug.LogError("MorseImageContainer에 MorseImage나 MorseInputImage가 없음");
        }

        arduino_MorseKey = GetComponentInParent<Arduino_MorseKey>();
    }
    void OnDisable()
    {

        if (arduino_MorseKey != null)
        {
            arduino_MorseKey.StopMorseCheck();
        }

    }

    protected IEnumerator InputDequeue()
    {
        if (_morseInput.Count > 0)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;

            ColoringMorseImage(_morseInput.Dequeue());
        }

    }

    virtual public void ColoringMorseImage(MorseType morseType)
    {
        if (_currentIndex >= morseInputImages.Length)
        {
            return;
        }


        //TODO 급하게 막았는데 구조 좀 생각해서 수정 
        //TODO Guide모드에서 틀린 입력 들어왔을 때 인덱스 떄문에 구조 고민해야함

        if (_morseIndexCheckCoroutine == null)
        {

            morseInputImages[_currentIndex].StartColoring(morseType);
            _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));

        }
        else
        {
            //Debug.Log($"코루틴 돌리는중 추가입력 {morseType} 큐에 추가");
            _morseInput.Enqueue(morseType);
        }

    }

    // virtual public void ColoringMorseImage2(MorseType morseType)
    // {

    //     if (_currentIndex >= morseInputImages.Length)
    //     {
    //         return;
    //     }


    //     if (_morseIndexCheckCoroutine == null)
    //     {

    //         morseInputImages[_currentIndex].StartColoring(morseType);
    //         _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));

    //     }
    //     else
    //     {
    //         //Debug.Log($"코루틴 돌리는중 추가입력 {morseType} 큐에 추가");
    //         _morseInput.Enqueue(morseType);
    //     }

    // }



}

