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


    public Action OnRedInputs;

    //Action onFailedInput;

    //Action<MorseType> onDashColoring;

    public Action<float> OnAccuracyCheckAction;

    bool _isOpenOverInputPopup = false;

    //bool morseInputDelay = false;

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

    bool isLastInputNShadow = false;

    float[] pressTimes = new float[4];

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

    public Action<float> OnUpdateDashVar;

    public Action<int> ShadowDash;

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
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                GameManager.Instance.GoToIdleCheck();

                startTime = Time.time;
                isPress = true;

                if (_answer != "")
                {
                    //게이지 바 컷인 떄문에 확 올라가는거 방지할려는 임시방편
                    //TODO 수정
                    if (ResetUIOn?.StartResetUIOn() == true)
                    {
                        SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseResetSound);
                        startTime += 0.5f;
                    }
                    else if (_answer != "")
                    {
                        SoundManager.Instance.PlayEffectSound(EffectSoundNum.MorseResetSound);

                    }
                }
            }

            else if (isPress && Input.GetKey(KeyCode.LeftControl))
            {

                if (_answer != "")
                {
                    if ((Time.time - startTime) / MorseTranslator.InputResetTime > 0)
                        ResetUIOn?.ResetBarUpdate((Time.time - startTime) / MorseTranslator.InputResetTime);
                }

                if (Time.time - startTime >= MorseTranslator.OverInputTime)
                {
                    OverInputProcess();
                }
                if (Time.time - startTime >= MorseTranslator.MaxDotTime && Time.time - startTime < MorseTranslator.MaxDotTime + 0.1f)
                {
                    if (_inputCount < 4)
                    {
                        OnUpdateDashVar?.Invoke(Time.time - startTime);
                        if (_inputCount == 3)
                            isLastInputNShadow = true;

                        ShadowDash?.Invoke(_inputCount);
                        if (_isGuide && PageController.Instance.CurrentPage == 3)
                        {
                            isPress = false;
                            MorseTransmit(MorseTranslator.DefaultDashTime);

                        }
                    }


                }




            }

            else if (isPress && Input.GetKey(KeyCode.LeftControl) == false)
            {
                NetworkManager.Instance.SendData($"M");

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
                if (PageController.Instance.CurrentPage == 4 || PageController.Instance.CurrentPage == 5 || PageController.Instance.CurrentPage == 6)
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
                if (_isGuide == false || (Time.time - startTime) < MorseTranslator.MaxDotTime)
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
        if (_inputCount < 4)
        {
            pressTimes[_inputCount] = pressTime;
        }

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
        else
        {
            Debug.Log($"DASH");
            _currentMorseInputData[_inputCount].SetMorseInputData(true, pressTime);
            onMorseInput?.Invoke(MorseType.Dash);
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

        if (_isOverInputPopupOn == false)
        {
            _isOverInputPopupOn = true;

            SoundManager.Instance.PlayEffectSound(EffectSoundNum.PopupSound);
            FadeManager.Instance.SetAlphaOne(OverInputPopup);
        }
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

        while (IsColoringDone == false && IsAccuracyRateCheck == false && isLastInputNShadow == false)
        {
            yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
        }

        _morseData = "";


        while (_morseQueue.Count > 0)
        {
            MorseInputData morseInputData = _morseQueue.Dequeue();
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


        _answer = MorseTranslator.Translate(_morseData);

        if (IsAccuracyRateCheck)
        {
            OnAccuracyCheckAction?.Invoke(MorseTranslator.Accuracy(UserDataManager.Instance.GetPlayer().PartnerPassCode, pressTimes));

        }
        else
        {
            if (_answer != "")
            {


                yield return CoroutineReturnManager.GetWaitForSeconds(0.3f); // 입력 사운드랑 겹쳐서 완성을 1초 딜레이 
                OnMorseTransmitEnd?.Invoke();
                _morseQueue.Clear();
            }
            else
            {
                //TODO 빨간색 변하기
                // onFailedInput?.Invoke();
                OnRedInputs?.Invoke();
                yield return CoroutineReturnManager.GetWaitForSeconds(1f);
                // yield return CoroutineReturnManager.GetWaitForSeconds(1f);
                Reset();
            }

        }

        _coloringWaitCoroutine = null;

    }


    public void StartMorseCheck()
    {
        isLastInputNShadow = false;
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

        ShadowDash = null;

        OnUpdateDashVar = null;

        _isAccuracyRateCheck = false;

        onMorseInput = null;
    }

    void OnApplicationQuit()
    {
        ;
    }
}