using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using Unity.VisualScripting;
public class Arduino_MorseKey : MonoBehaviour
{

    //TODO 겟 컴포넌트로 그냥 할당하면 될듯

    public AudioSource MorseDotSound;
    public AudioSource MorseDashSound;





    Queue<bool> _morseQueue = new Queue<bool>();


    const float MaxDotTime = 0.15f;

    const float MaxDashTime = 1f;


    Action<MorseType> onMorseInput;

    public Action OnReset;


    public Action<string> onMorseTransmitEnd;

    bool isInputCheck = false;

    bool isPress = false;


    float startTime = 0f;

    string _morseData = "";

    string _answer;

    bool resetRequest = false;

    int _inputCount = 0;


    public ResetUIOn ResetUIOn;
    public string MorseData
    {
        get { return _morseData; }
    }

    protected virtual void Start()
    {

    }

    void Update()
    {
        if (isInputCheck)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_answer != "")
                {
                    ResetUIOn?.StartResetUIOn();
                }
                startTime = Time.time;
                isPress = true;
            }

            else if (isPress && Input.GetKey(KeyCode.C))
            {
                if (_answer != "")
                {
                    if (Time.time - startTime < 1f)
                        ResetUIOn?.ResetBarUpdate(Time.time - startTime);

                }


            }

            else if (isPress && Input.GetKey(KeyCode.C) == false)
            {
                if (PageController.Instance.CurrentPage == 4 || PageController.Instance.CurrentPage == 5)
                {
                    if (Time.time - startTime >= 1f && _inputCount > 3)
                        resetRequest = true;

                    if (_answer != "")
                    {
                        if (Time.time - startTime < 1f)
                            ResetUIOn?.ResetBarUpdate(0f);
                    }
                }

                if (resetRequest)
                {
                    Reset();
                    return;
                }

                MorseTransmit(Time.time - startTime);

                CheckMorse();

                isPress = false;
            }
        }


    }

    public void AddOnMorseInput(Action<MorseType> morseAction)
    {
        onMorseInput += morseAction;
    }

    public void RemoveOnMorseInput(Action<MorseType> morseAction)
    {
        onMorseInput -= morseAction;
    }
    IEnumerator EndInputCoroutine()
    {

        yield return CoroutineReturnManager.GetWaitForSeconds(MaxDashTime + 0.5f);

        CheckMorse();
    }

    public void MorseTransmit(float pressTime)
    {
        GameManager.Instance.GoToIdleCheck();

        if (_answer != "" || _inputCount >= 4)
        {
            return;
        }



        if (MaxDotTime >= pressTime)
        {
            Debug.Log($"DOT");
            MorseDotSound.PlayOneShot(MorseDotSound.clip, 1f);
            _morseQueue.Enqueue(false);
            onMorseInput?.Invoke(MorseType.Dot);

        }
        else
        {
            Debug.Log($"DASH");
            MorseDashSound.PlayOneShot(MorseDashSound.clip, 1f);

            _morseQueue.Enqueue(true);
            onMorseInput?.Invoke(MorseType.Dash);
        }
        _inputCount++;


    }

    public void Reset()
    {
        OnReset?.Invoke();

        ValueReset();
    }



    private void ValueReset()
    {
        _morseQueue.Clear();
        _answer = "";
        _inputCount = 0;

        _morseData = "";
        isPress = false;
        resetRequest = false;

    }

    public void CheckMorse()
    {
        if (_morseQueue.Count != 4)
        {
            return;
        }

        if (_answer != "")
        {
            return;
        }
        _morseData = "";



        while (_morseQueue.Count > 0)
        {
            bool isDash = _morseQueue.Dequeue();
            if (isDash)
            {
                _morseData += '1';
            }
            else
            {
                _morseData += '0';
            }
            if (_morseData.Length > 4)
            {
                break;
            }
        }
        if (PageController.Instance.CurrentPage == 4)
        {
            switch (_morseData)
            {
                case "0010":
                    _answer = "봄";
                    break;
                case "0100":
                    _answer = "여름";
                    break;
                case "0110":
                    _answer = "가을";
                    break;
                case "0101":
                    _answer = "겨울";
                    break;
                case "0001":
                    _answer = "사계";
                    break;
                default:
                    _answer = "";
                    break;
            }
        }
        else if (PageController.Instance.CurrentPage == 5)
        {
            switch (_morseData)
            {
                case "0100":
                    _answer = "너무 신나";
                    break;
                case "0000":
                    _answer = "행복해";


                    break;
                case "1100":
                    _answer = "그냥 그래";

                    break;
                case "1000":
                    _answer = "평범한 하루";

                    break;
                case "1101":
                    _answer = "오늘도 새로워";
                    break;
                case "1110":
                    _answer = "점점 기대돼";
                    break;
                case "0111":
                    _answer = "어제보단 별로";

                    break;
                case "0011":
                    _answer = "완전 우울해";
                    break;
                case "1011":
                    _answer = "속상한 하루";

                    break;
                case "1010":
                    _answer = "내일이 얼른 오길";
                    break;
                case "0010":
                    _answer = "햇살 같아";
                    break;
                case "0001":
                    _answer = "홀가분한 날";

                    break;
                case "0101":
                    _answer = "기쁨 만땅";
                    break;
                case "0110":
                    _answer = "웃기고 즐거워";

                    break;
                case "1111":
                    _answer = "그리워";
                    break;
                default:
                    Debug.LogError("MorseImageContainer GetMorseTransmitData 잘못된 데이터 받음 " + _morseData);


                    break;
            }
        }

        if (_answer != "")
        {
            onMorseTransmitEnd?.Invoke(_morseData);
        }
        _morseQueue.Clear();

    }


    public void StartMorseCheck()
    {
        isInputCheck = true;


        ValueReset();

        /// 이걸 컨트롤러에서 실행해야함 
        if (GameManager.Instance.IsStarted == false)
            return;
    }
    public void StopMorseCheck()
    {
        OnReset = null;
        onMorseTransmitEnd = null;
        isInputCheck = false;

        onMorseInput = null;
    }

    void OnApplicationQuit()
    {
        ;
    }
}