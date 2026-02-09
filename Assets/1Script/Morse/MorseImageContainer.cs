using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorseImageContainer : MonoBehaviour
{



    MorseInputImage[] morseInputImages;

    public ResetUIOn resetUIOn;

    public CanvasGroup[] PopupUI;

    public Graphic[] PopupUI_OffGraphics;

    public QuestionSelectTextContainer questionTextContainer;
    Queue<MorseType> _morseInput = new Queue<MorseType>();


    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;

    Coroutine _morseIndexCheckCoroutine = null;

    float _popupDelay = 1.2f;


    //Todo 저거 좀 이벤트로 쪼개기

    bool isAnswer = false;



    int _currentIndex = 0;




    void OnEnable()
    {
        _morseIndexCheckCoroutine = null;
        if (arduino_MorseKey != null)
            arduino_MorseKey.StopMorseCheck();
    }

    public void CheckStart()
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

    public void Reset()
    {
        Debug.Log("Reset");

        foreach (MorseInputImage mi in morseInputImages)
        {
            mi.Reset();
        }

        foreach (CanvasGroup popupUI in PopupUI)
        {
            FadeManager.Instance.SetAlphaZero(popupUI);
        }

        // FadeManager.Instance.SetAlphaZero(CheckGraphics);

        isAnswer = false;

        _currentIndex = 0;
    }


    IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
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
            FadeManager.Instance.SetAlphaOne(popupUI);
        }
        isAnswer = true;
    }

    public void SetAnswerTrue()
    {
        StartCoroutine(SetAnswerTrueCoroutine(_popupDelay));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isAnswer)
            {
                PlayerDatas.Instance.GetPlayer().QuestionAnswerData.Enqueue(MorseTranslator.CurrentData);
                SequenceScript.TriggerFroceOn();
                arduino_MorseKey.StopMorseCheck();
            }
        }
    }


    void Start()
    {

        questionTextContainer = transform.parent.GetComponentInChildren<QuestionSelectTextContainer>();

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

    IEnumerator InputDequeue()
    {
        if (_morseInput.Count > 0)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;

            ColoringMorseImage(_morseInput.Dequeue());
        }

    }

    public void ColoringMorseImage(MorseType morseType)
    {

        if (_currentIndex >= morseInputImages.Length)
        {
            return;
        }


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



}

