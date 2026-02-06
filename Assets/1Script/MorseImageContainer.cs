using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MorseImageContainer : MonoBehaviour
{
    //Todo 두개로 분리할까 ?

    MorseImage[] morseImages;
    MorseInputImage[] morseInputImages;




    public ResetUIOn resetUIOn;

    public CanvasGroup[] PopupUI;

    public Graphic[] PopupUI_OffGraphics;



    public QuestionSelectTextContainer questionTextContainer;
    Queue<MorseType> _morseInput = new Queue<MorseType>();

    bool isGuideMode = false;

    Arduino_MorseKey arduino_MorseKey;

    public SequenceScript SequenceScript;



    Coroutine _morseIndexCheckCoroutine = null;

    public Graphic[] CheckGraphics;


    string Ctext = "";

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
            arduino_MorseKey.OnReset += Reset;

            arduino_MorseKey.onMorseTransmitEnd += GetMorseTransmitData;
        }
        _currentIndex = 0;

        _morseInput.Clear();

        arduino_MorseKey.StartMorseCheck();


    }

    public void Reset()
    {
        Debug.Log("Reset");
        if (isGuideMode)
        {
            ;
        }
        else
        {
            foreach (MorseInputImage mi in morseInputImages)
            {
                mi.Reset();
            }

            foreach (CanvasGroup popupUI in PopupUI)
            {
                FadeManager.Instance.SetAlphaZero(popupUI);
            }

            // FadeManager.Instance.SetAlphaZero(CheckGraphics);

            resetUIOn.Reset();
            Ctext = "";

            questionTextContainer.Reset();
            _currentIndex = 0;
        }
    }


    IEnumerator MorseIndexCheckCoroutine(MorseType morseType)
    {
        isAnswer = false;

        if (isGuideMode)
        {
            while (morseImages[_currentIndex].IsCheck == false)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
        }
        else
        {

            while (morseInputImages[_currentIndex].isFilled == false)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
            }
        }

        _currentIndex++;
        if (isGuideMode)
        {
            if (_currentIndex == morseImages.Length)
            {
                Debug.Log("트리거");
                SequenceScript.TriggerFroceOn();
                arduino_MorseKey.StopMorseCheck();
            }
        }
        else
        {
            if (_currentIndex < morseInputImages.Length && _morseInput.Count > 0)
            {
                StartCoroutine(InputDequeue());
            }
            else if (_currentIndex == morseInputImages.Length)
            {
                yield return CoroutineReturnManager.GetWaitForSeconds(0.5f);
                if (Ctext != "")
                {
                    Debug.Log(Ctext + " 정답확인");
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
                    yield return CoroutineReturnManager.WaitForFixedUpdate;
                    isAnswer = true;
                    Debug.Log("트리거");


                }
                else
                {
                    arduino_MorseKey.Reset();
                }

            }
        }
        _morseIndexCheckCoroutine = null;


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isAnswer && Ctext != "")
            {
                SequenceScript.TriggerFroceOn();
                arduino_MorseKey.StopMorseCheck();
            }

        }
    }

    public void GetMorseTransmitData(string morseData)
    {
        if (questionTextContainer == null)
        {
            // return "";
            return;
        }
        Ctext = "";
        Debug.Log("디버그 값: " + questionTextContainer.GetOptionCount());

        if (questionTextContainer.GetOptionCount() == 5)
        {
            switch (morseData)
            {
                case "0010":
                    Ctext = questionTextContainer.Select(0);
                    break;
                case "0100":
                    Ctext = questionTextContainer.Select(1);

                    break;
                case "0110":
                    Ctext = questionTextContainer.Select(2);
                    break;
                case "0101":
                    Ctext = questionTextContainer.Select(3);

                    break;
                case "0001":
                    Ctext = questionTextContainer.Select(4);
                    break;
                default:
                    Debug.LogError("MorseImageContainer GetMorseTransmitData 잘못된 데이터 받음 " + morseData);


                    break;
            }
        }
        else if (questionTextContainer.GetOptionCount() == 15)
        {
            switch (morseData)
            {
                case "0100":
                    Ctext = questionTextContainer.Select(0);
                    break;
                case "0000":
                    Ctext = questionTextContainer.Select(1);

                    break;
                case "1100":
                    Ctext = questionTextContainer.Select(2);
                    break;
                case "1000":
                    Ctext = questionTextContainer.Select(3);

                    break;
                case "1101":
                    Ctext = questionTextContainer.Select(4);
                    break;
                case "1110":
                    Ctext = questionTextContainer.Select(5);
                    break;
                case "0111":
                    Ctext = questionTextContainer.Select(6);

                    break;
                case "0011":
                    Ctext = questionTextContainer.Select(7);
                    break;
                case "1011":
                    Ctext = questionTextContainer.Select(8);

                    break;
                case "1010":
                    Ctext = questionTextContainer.Select(9);
                    break;
                case "0010":
                    Ctext = questionTextContainer.Select(10);
                    break;
                case "0001":
                    Ctext = questionTextContainer.Select(11);

                    break;
                case "0101":
                    Ctext = questionTextContainer.Select(12);
                    break;
                case "0110":
                    Ctext = questionTextContainer.Select(13);

                    break;
                case "1111":
                    Ctext = questionTextContainer.Select(14);
                    break;
                default:
                    Debug.LogError("MorseImageContainer GetMorseTransmitData 잘못된 데이터 받음 " + morseData);


                    break;
            }
        }



        //return Ctext;

        Debug.Log("답 :  " + Ctext);

    }

    void Start()
    {
        morseImages = GetComponentsInChildren<MorseImage>();

        questionTextContainer = transform.parent.GetComponentInChildren<QuestionSelectTextContainer>();
        if (morseImages.Length > 0)
        {
            isGuideMode = true;
        }
        else
        {
            morseInputImages = GetComponentsInChildren<MorseInputImage>();
            if (morseInputImages.Length == 0)
            {
                Debug.LogError("MorseImageContainer에 MorseImage나 MorseInputImage가 없음");
            }
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
        if (isGuideMode)
        {
            if (_currentIndex >= morseImages.Length)
            {
                return;
            }
            if (morseImages[_currentIndex].CurrentMorseType == morseType)
            {
                morseImages[_currentIndex].StartColoring();
                if (_morseIndexCheckCoroutine == null)
                    _morseIndexCheckCoroutine = StartCoroutine(MorseIndexCheckCoroutine(morseType));
            }

        }
        else
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



}

