using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

class MorseInputData
{
    public MorseInputData(bool isDash, float pressTime)
    {
        _isDash = isDash;
        _pressTime = pressTime;
    }

    public void SetMorseInputData(bool isDash, float pressTime)
    {
        _isDash = isDash;
        _pressTime = pressTime;
    }


    bool _isDash;
    float _pressTime;

    public bool IsDash
    {
        get { return _isDash; }
    }
    public float PressTime
    {
        get { return _pressTime; }
    }
}
public class Arduino_MorseKey : MonoBehaviour
{
    public Texture DotTexture;
    public Texture DashTexture;

    public AudioSource MorseDotSound;
    public AudioSource MorseDashSound;
    Queue<MorseInputData> _morseQueue = new Queue<MorseInputData>();


    MorseInputData[] _currentMorseInputData = new MorseInputData[4] { new MorseInputData(false, 0f), new MorseInputData(false, 0f), new MorseInputData(false, 0f), new MorseInputData(false, 0f) };

    Action<MorseType> onMorseInput;

    public Action<float> OnAccuracyCheckAction;



    bool _isAccuracyRateCheck = false;

    public bool IsAccuracyRateCheck
    {
        get { return _isAccuracyRateCheck; }
        set { _isAccuracyRateCheck = value; }
    }


    public Action OnMorseTransmitEnd;
    public CanvasGroup OverInputPopup;
    public Action OnReset;

    readonly float _overInputPopupTime = 3f;
    bool _isOverInputPopupOn = false;

    bool isInputCheck = false;

    bool isPress = false;


    float startTime = 0f;

    string _morseData = "";

    string _answer;

    bool resetRequest = false;

    public bool IsColoringDone = false;

    Coroutine _overInputCoroutine = null;

    bool _isGuide = false;
    public bool IsGuide
    {
        get { return _isGuide; }
        set { _isGuide = value; }
    }


    int _inputCount = 0;

    Coroutine _coloringWaitCoroutine = null;


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
        if (isInputCheck && _isOverInputPopupOn == false)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                startTime = Time.time;
                isPress = true;

                if (_answer != "")
                {
                    //게이지 바 컷인 떄문에 확 올라가는거 방지할려는 임시방편
                    //TODO 수정
                    if (ResetUIOn?.StartResetUIOn() == true)
                        startTime += 0.5f;
                }
            }

            else if (isPress && Input.GetKey(KeyCode.C))
            {
                if (_answer != "")
                {
                    ResetUIOn?.ResetBarUpdate((Time.time - startTime) / MorseTranslator.InputResetTime);
                }
                if (Time.time - startTime >= MorseTranslator.OverInputTime)
                {
                    OverInputProcess();
                }


            }

            else if (isPress && Input.GetKey(KeyCode.C) == false)
            {
                if (_inputCount > 3)
                {
                    if ((Time.time - startTime) / MorseTranslator.InputResetTime >= 1f)
                    {
                        resetRequest = true;
                    }
                    else
                    {
                        ResetUIOn?.ResetBarUpdate(0f);
                    }
                }
                if (PageController.Instance.CurrentPage == 4 || PageController.Instance.CurrentPage == 5)
                {


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
    // IEnumerator EndInputCoroutine()
    // {


    //     yield return CoroutineReturnManager.GetWaitForSeconds(MorseTranslator.MaxDashTime + 0.5f);

    //     CheckMorse();
    // }

    public void MorseTransmit(float pressTime)
    {
        GameManager.Instance.GoToIdleCheck();

        if (_answer != "" || _inputCount >= 4)
        {
            return;
        }

        if (MorseTranslator.MaxDotTime >= pressTime)
        {
            Debug.Log($"DOT");

            _currentMorseInputData[_inputCount].SetMorseInputData(false, pressTime);
            onMorseInput?.Invoke(MorseType.Dot);
        }
        else if (MorseTranslator.MaxDashTime >= pressTime)
        {
            Debug.Log($"DASH");
            _currentMorseInputData[_inputCount].SetMorseInputData(true, pressTime);
            onMorseInput?.Invoke(MorseType.Dash);
        }
        else
        {
            return;
        }
        if (_isGuide == false)
        {

            _morseQueue.Enqueue(_currentMorseInputData[_inputCount]);
            _inputCount++;
        }


    }

    public void OverInputProcess()
    {
        if (_overInputCoroutine != null)
        {
            StopCoroutine(_overInputCoroutine);
        }
        _overInputCoroutine = StartCoroutine(OverInputCoroutine());
    }
    public IEnumerator OverInputCoroutine()
    {
        _isOverInputPopupOn = true;
        if (OverInputPopup.alpha < 0.9f)
            FadeManager.Instance.SetAlphaOne(OverInputPopup);
        float starttime = Time.time;

        while (Time.time - starttime < _overInputPopupTime)
        {
            yield return CoroutineReturnManager.WaitForFixedUpdate;
        }
        FadeManager.Instance.SetAlphaZero(OverInputPopup);
        _isOverInputPopupOn = false;

        _overInputCoroutine = null;
    }

    public void AddInputCount()
    {
        _morseQueue.Enqueue(_currentMorseInputData[_inputCount]);
        _inputCount++;

        Debug.Log($"InputCount : {_inputCount} ");

    }

    public void PlayMorseSound(MorseType morseType)
    {
        return;
        if (morseType == MorseType.Dash)
            MorseDashSound.PlayOneShot(MorseDashSound.clip, 1f);
        else
            MorseDotSound.PlayOneShot(MorseDotSound.clip, 1f);
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
        _overInputCoroutine = null;

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
        if (_coloringWaitCoroutine == null)
            _coloringWaitCoroutine = StartCoroutine(ColoringWaitCoroutine());


    }


    public IEnumerator ColoringWaitCoroutine()
    {
        IsColoringDone = false;

        while (IsColoringDone == false && IsAccuracyRateCheck == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        _morseData = "";


        float[] pressTimes = new float[4];
        while (_morseQueue.Count > 0)
        {
            MorseInputData morseInputData = _morseQueue.Dequeue();
            pressTimes[pressTimes.Length - _morseQueue.Count - 1] = morseInputData.PressTime;
            if (morseInputData.IsDash)
            {
                _morseData += '1';
                Debug.Log("1");
            }
            else
            {
                _morseData += '0';
                Debug.Log("0");

            }
            if (_morseData.Length > 4)
            {
                break;
            }
        }
        MorseTranslatorData morseTranslatorData = MorseTranslator.Translate(_morseData, pressTimes);
        _answer = morseTranslatorData.MorseData;

        if (IsAccuracyRateCheck)
        {
            OnAccuracyCheckAction?.Invoke(morseTranslatorData.PressTimes);

        }
        else
        {
            if (_answer != "")
            {
                if (PageController.Instance.CurrentPage == 5)
                    PlayerData.Instance.GetPlayer().PassCode = _answer;
                OnMorseTransmitEnd?.Invoke();
                _morseQueue.Clear();

            }
            else
            {
                Reset();
            }

        }

        _coloringWaitCoroutine = null;

    }


    public void StartMorseCheck()
    {
        if (_coloringWaitCoroutine != null)
        {
            StopCoroutine(_coloringWaitCoroutine);
            _coloringWaitCoroutine = null;
        }
        isInputCheck = true;


        ValueReset();

        /// 이걸 컨트롤러에서 실행해야함 
        if (GameManager.Instance.IsStarted == false)
            return;
    }
    public void StopMorseCheck()
    {
        _isGuide = false;
        OnReset = null;
        ResetUIOn = null;
        OnMorseTransmitEnd = null;
        isInputCheck = false;

        _isAccuracyRateCheck = false;

        onMorseInput = null;
    }

    void OnApplicationQuit()
    {
        ;
    }
}