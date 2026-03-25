using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class WaitCheck : MonoBehaviour
{
    public SequenceScript Player1_Trigger;
    public SequenceScript Player2_Trigger;

    public UnityEvent ClearTrigger = new UnityEvent();

    public RawImage[] Player1_ColorBall;
    public Text[] Player1_NameText;
    public RawImage[] Player2_ColorBall;
    public Text[] Player2_NameText;

    bool _isPlayer1On = false;
    public bool IsPlayer1On
    {
        get { return _isPlayer1On; }
        set
        {
            if (value && !_isPlayer1On)
            {
                _isPlayer1On = true;
            }

        }
    }
    bool _isPlayer2On = false;
    public bool IsPlayer2On
    {
        get { return _isPlayer2On; }
        set
        {
            if (value && !_isPlayer2On)
            {
                _isPlayer2On = true;
            }
        }
    }


    Coroutine checkCoroutine = null;


    WaitForSeconds _checkWait = new WaitForSeconds(1f);

    Coroutine debugZ = null;
    Coroutine debugX = null;

    WaitForSeconds debugWait = new WaitForSeconds(0.2f);


    bool isTriggerTime = false;




    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (debugZ == null)
            {
                debugZ = StartCoroutine(ChangeZ());
            }


        }
        if (Input.GetKey(KeyCode.X))
        {
            if (debugX == null)
            {
                debugX = StartCoroutine(ChangeX());
            }

        }
    }

    IEnumerator ChangeZ()
    {
        yield return debugWait;
        Player1_Trigger.TriggerForceOn();
        IsPlayer1On = !_isPlayer1On;

        FadeManager.Instance.TargetFade(Player1_ColorBall, 1f);
        FadeManager.Instance.TargetFade(Player1_NameText, 1f);


        debugZ = null;

    }
    IEnumerator ChangeX()
    {
        yield return debugWait;

        Player2_Trigger.TriggerForceOn();
        IsPlayer2On = !_isPlayer2On;

        FadeManager.Instance.TargetFade(Player2_ColorBall, 1f);
        FadeManager.Instance.TargetFade(Player2_NameText, 1f);

        debugX = null;

    }
    IEnumerator WaitCoroutine()
    {
        bool isAllReady = false;
        while (isAllReady == false)
        {
            yield return _checkWait;

            if (isTriggerTime && IsPlayer1On && IsPlayer2On)
            {


                yield return CoroutineReturnManager.GetWaitForSeconds(0.1f);
                ClearTrigger.Invoke();
                isAllReady = true;

            }

        }
        isTriggerTime = false;
        checkCoroutine = null;
    }

    public void SetTriggerTime(bool isOn)
    {
        isTriggerTime = isOn;
    }



    // void OnEnable()
    // {
    //     if (GameManager.Instance.IsStarted == false)
    //     {
    //         return;
    //     }
    //     Reset();

    //     checkCoroutine = StartCoroutine(WaitCoroutine());
    // }

    public void StartCheck()
    {
        if (GameManager.Instance.IsStarted == false)
        {
            return;
        }
        Reset();
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }

        checkCoroutine = StartCoroutine(WaitCoroutine());
    }

    void OnDisable()
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null;
        }
    }

    void Reset()
    {
        _isPlayer1On = false;
        _isPlayer2On = false;



    }



    public void OnClear()
    {
        ClearTrigger.Invoke();
    }

}
